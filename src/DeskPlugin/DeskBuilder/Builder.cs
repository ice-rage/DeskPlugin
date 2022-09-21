using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DeskParameters;
using DeskParameters.Enums;
using DeskParameters.Enums.Extensions;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace DeskBuilder
{
    /// <summary>
    /// Класс для построения 3D-модели письменного стола.
    /// </summary>
    public class Builder
    {
        #region Fields

        /// <summary>
        /// Имя блока, содержащего 3D-модель письменного стола.              
        /// </summary>
        private const string DeskBlockName = "Desk";

        #endregion

        #region Methods

        /// <summary>
        /// Метод для построения 3D-модели письменного стола.
        /// </summary>
        /// <param name="parameters"> Параметры, необходимые для построения 3D-модели.</param>
        public void BuildDesk(Parameters parameters)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Database database = Application.DocumentManager.MdiActiveDocument.Database)
                {
                    using (Transaction transaction = database.TransactionManager.StartTransaction())
                    {
                        ObjectId blockTableId = database.BlockTableId;

                        // Создаем новый блок для 3D-модели письменного стола.
                        CreateDeskBlock(blockTableId);

                        // Получаем параметры стола.
                        //
                        int worktopLength = parameters[ParameterGroupType.Worktop]
                            [ParameterType.WorktopLength].Value;
                        int worktopWidth = parameters[ParameterGroupType.Worktop]
                            [ParameterType.WorktopWidth].Value;
                        int worktopHeight = parameters[ParameterGroupType.Worktop]
                            [ParameterType.WorktopHeight].Value;
                        int legHeight = parameters[ParameterGroupType.Legs]
                            [ParameterType.LegHeight].Value;
                        int drawerNumber = parameters[ParameterGroupType.Drawers]
                            [ParameterType.DrawerNumber].Value;
                        int drawerLength = parameters[ParameterGroupType.Drawers]
                            [ParameterType.DrawerLength].Value;
                        double drawerHeight = (double)legHeight / drawerNumber;

                        LegType legType = parameters.LegType;
                        int legBaseValue = parameters[ParameterGroupType.Legs]
                            [parameters.LegType.GetLegBaseType()].Value;

                        // Строим столешницу.
                        BuildWorktop(worktopLength, worktopWidth, worktopHeight);

                        // Строим ножки стола.
                        BuildLegs(legType, legBaseValue, legHeight, worktopWidth);

                        // Строим ящики для канцелярии.
                        BuildDrawers(drawerNumber, drawerLength, drawerHeight, worktopLength,
                            worktopWidth);

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Метод для создания блока, содержащего 3D-модель письменного стола.
        /// </summary>
        /// <param name="blockTableId"> Идентификатор таблицы блоков из базы данных активного
        /// документа AutoCAD.</param>
        private static void CreateDeskBlock(ObjectId blockTableId)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Database database = Application.DocumentManager.MdiActiveDocument.Database)
                {
                    using (Transaction transaction = database.TransactionManager.StartTransaction())
                    {
                        // Если 3D-модель письменного стола уже была создана, удаляем ее.
                        EraseExistingDeskModel(transaction, blockTableId);

                        var blockTable = (BlockTable)transaction.GetObject(blockTableId,
                            OpenMode.ForWrite);

                        // Создаем в таблице блоков определение нового блока, присваиваем ему имя.
                        var blockTableRecord = new BlockTableRecord
                        {
                            Name = DeskBlockName
                        };

                        // Добавляем созданное определение блока в таблицу блоков и в транзакцию,
                        // запоминаем идентификатор этого определения.
                        //
                        ObjectId deskBlockTableRecordId = blockTable.Add(blockTableRecord);
                        transaction.AddNewlyCreatedDBObject(blockTableRecord, true);

                        // Открываем пространство моделей для записи и создаем в нем новое вхождение
                        // блока, используя сохраненный идентификатор определения этого блока.
                        //
                        var deskBlockReference = new BlockReference(Point3d.Origin,
                            deskBlockTableRecordId);
                        AddObjectInBlock(deskBlockReference, BlockTableRecord.ModelSpace);

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Метод для удаления существующей 3D-модели письменного стола.
        /// </summary>
        /// <param name="transaction"> Текущая транзакция.</param>
        /// <param name="blockTableId"> Идентификатор таблицы блоков из базы данных активного
        /// документа AutoCAD.</param>
        private static void EraseExistingDeskModel(Transaction transaction, ObjectId blockTableId)
        {
            ObjectId deskBlockId = ObjectId.Null;

            // Если ранее уже был создан блок, содержащий 3D-модель письменного стола, получаем
            // идентификатор этого блока.
            //
            var blockTable = transaction.GetObject(blockTableId, OpenMode.ForRead) as BlockTable;

            if (blockTable != null && blockTable.Has(DeskBlockName))
            {
                deskBlockId = blockTable[DeskBlockName];
            }

            if (deskBlockId.IsNull)
            {
                return;
            }

            // Прежде чем удалять определение блока из таблицы блоков, удаляем все его вхождения,
            // чтобы предотвратить повреждение чертежа.
            //
            var deskBlockTableRecord = (BlockTableRecord)transaction.GetObject(deskBlockId,
                OpenMode.ForRead);

            ObjectIdCollection deskBlockReferenceIds = deskBlockTableRecord
                .GetBlockReferenceIds(true, true);

            if (deskBlockReferenceIds != null && deskBlockReferenceIds.Count > 0)
            {
                foreach (ObjectId deskBlockReferenceId in deskBlockReferenceIds)
                {
                    var deskBlockReference = (BlockReference)transaction
                        .GetObject(deskBlockReferenceId, OpenMode.ForWrite);
                    deskBlockReference.Erase();
                }
            }

            // После удаления всех вхождений блока из поля чертежа удаляем его определение.
            //
            deskBlockReferenceIds = deskBlockTableRecord.GetBlockReferenceIds(true, true);

            if (deskBlockReferenceIds != null && deskBlockReferenceIds.Count != 0)
            {
                return;
            }

            deskBlockTableRecord.UpgradeOpen();
            deskBlockTableRecord.Erase();
        }

        /// <summary>
        /// Метод для добавления нового объекта в определенный блок.
        /// </summary>
        /// <param name="databaseObject"> Объект для добавления в блок.</param>
        /// <param name="blockName"> Имя блока, в который нужно добавить объект.</param>
        private static void AddObjectInBlock(DBObject databaseObject, string blockName)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Database database = Application.DocumentManager.MdiActiveDocument.Database)
                {
                    using (Transaction transaction = database.TransactionManager.StartTransaction())
                    {
                        // Открываем для записи определенный блок из таблицы блоков.
                        //
                        var blockTable = (BlockTable)transaction.GetObject(database.BlockTableId,
                            OpenMode.ForRead);
                        var blockTableRecord = (BlockTableRecord)transaction
                            .GetObject(blockTable[blockName], OpenMode.ForWrite);

                        // Добавляем новый объект в данный блок и в транзакцию.
                        //
                        blockTableRecord.AppendEntity((Entity)databaseObject);
                        transaction.AddNewlyCreatedDBObject(databaseObject, true);

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Метод для построения столешницы.
        /// </summary>
        /// <param name="worktopLength"> Длина столешницы.</param>
        /// <param name="worktopWidth"> Ширина столешницы.</param>
        /// <param name="worktopHeight"> Высота столешницы.</param>
        private static void BuildWorktop(int worktopLength, int worktopWidth, int worktopHeight)
        {
            
        }

        /// <summary>
        /// Метод для построения ножек письменного стола.
        /// </summary>
        /// <param name="legType"> Тип ножек, описанный в перечислении <see cref="ParameterType"/>.
        /// </param>
        /// <param name="legBase"> Значение основания ножек.</param>
        /// <param name="legHeight"> Высота ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета положения ножек
        /// относительно краев стола.</param>
        private static void BuildLegs(LegType legType, int legBase, int legHeight, int worktopWidth)
        {
            
        }

        /// <summary>
        /// Метод, который строит ящики для канцелярии письменного стола.
        /// </summary>
        /// <param name="drawerNumber"> Количество ящиков для канцелярии.</param>
        /// <param name="drawerLength"> Длина ящиков для канцелярии.</param>
        /// <param name="drawerHeight"> Высота ящиков для канцелярии.</param>
        /// <param name="worktopLength"> Длина столешницы, необходимая для расчета размеров
        /// и положения ящиков для канцелярии, их дверц и ручек в пространстве чертежа.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета размеров
        /// и положения ящиков для канцелярии, их дверц и ручек в пространстве чертежа.</param>
        private static void BuildDrawers(
            int drawerNumber,
            int drawerLength,
            double drawerHeight,
            int worktopLength,
            int worktopWidth)
        {
            
        }

        #endregion
    }
}
