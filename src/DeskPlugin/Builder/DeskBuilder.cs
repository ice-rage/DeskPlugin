using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Parameters;
using Parameters.Enums;
using Parameters.Enums.Extensions;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Builder
{
    /// <summary>
    /// Класс для построения 3D-модели письменного стола.
    /// </summary>
    public class DeskBuilder
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
        public void BuildDesk(DeskParameters parameters)
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
                        int worktopLength = parameters[DeskParameterGroupType.Worktop,
                            DeskParameterType.WorktopLength].Value;
                        int worktopWidth = parameters[DeskParameterGroupType.Worktop,
                            DeskParameterType.WorktopWidth].Value;
                        int worktopHeight = parameters[DeskParameterGroupType.Worktop,
                            DeskParameterType.WorktopHeight].Value;
                        int legHeight = parameters[DeskParameterGroupType.Legs,
                            DeskParameterType.LegHeight].Value;
                        int drawerNumber = parameters[DeskParameterGroupType.Drawers,
                            DeskParameterType.DrawerNumber].Value;
                        int drawerLength = parameters[DeskParameterGroupType.Drawers,
                            DeskParameterType.DrawerLength].Value;
                        double drawerHeight = (double)legHeight / drawerNumber;

                        LegType legType = parameters.LegType;
                        int legBaseValue = parameters[DeskParameterGroupType.Legs,
                            parameters.LegType.GetLegBaseType()].Value;

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
                .GetBlockReferenceIds(true, false);

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
            deskBlockReferenceIds = deskBlockTableRecord.GetBlockReferenceIds(true, false);

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
            Solid3d worktop = CreateAndDisplaceBox(
                worktopLength,
                worktopWidth,
                worktopHeight,
                new Point3d(worktopLength, worktopWidth, worktopHeight));
            AddObjectInBlock(worktop, DeskBlockName);
        }

        /// <summary>
        /// Метод для создания 3D-примитива "ящик" и его перемещения в указанную точку чертежа.
        /// </summary>
        /// <param name="boxLength"> Длина ящика.</param>
        /// <param name="boxWidth"> Ширина ящика.</param>
        /// <param name="boxHeight"> Высота ящика.</param>
        /// <param name="displacementEndPoint"> Точка чертежа, в которую необходимо переместить
        /// ящик.</param>
        /// <returns> Созданный ящик.</returns>
        private static Solid3d CreateAndDisplaceBox(
            double boxLength,
            double boxWidth,
            double boxHeight,
            Point3d displacementEndPoint)
        {
            var box = new Solid3d();
            box.CreateBox(boxLength, boxWidth, boxHeight);

            Extents3d boxExtents = box.Bounds.GetValueOrDefault();
            var displacementStartPoint = new Point3d(boxExtents.MaxPoint.X, boxExtents.MaxPoint.Y,
                boxExtents.MaxPoint.Z);
            Vector3d displacementVector = displacementStartPoint.GetVectorTo(displacementEndPoint);
            box.TransformBy(Matrix3d.Displacement(displacementVector));

            return box;
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
            // Создаем словари, содержащие в качестве ключа порядковый номер ножки, а в качестве
            // значения - одну из координат ее основания.
            //
            var x = new Dictionary<int, double>();
            var y = new Dictionary<int, double>();

            // Строим основания ножек.
            DBObjectCollection legBases = legType == LegType.Round
                ? CreateRoundLegBases(legBase, worktopWidth, x, y)
                : CreateSquareLegBases(legBase, worktopWidth, x, y);

            // Создаем области из каждого замкнутого цикла, образованного массивом оснований ножек.
            DBObjectCollection legBasesRegions = Region.CreateFromCurves(legBases);

            // Выдавливаем каждую полученную область на заданную высоту и добавляем полученный
            // объект в блок 3D-модели письменного стола.
            foreach (Region region in legBasesRegions)
            {
                var leg = new Solid3d();
                leg.Extrude(region, -legHeight, 0);
                AddObjectInBlock(leg, DeskBlockName);

                region.Dispose();
            }
        }

        /// <summary>
        /// Метод для создания круглых оснований ножек письменного стола (если выбран тип ножек
        /// <see cref="LegType.Round"/>).
        /// </summary>
        /// <param name="baseDiameter"> Диаметр основания ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета положения ножек
        /// относительно краев стола.</param>
        /// <param name="x"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - x-координату центра окружности основания ножки.</param>
        /// <param name="y"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - y-координату центра окружности основания ножки.</param>
        /// <returns> Массив круглых оснований ножек.</returns>
        private static DBObjectCollection CreateRoundLegBases(int baseDiameter, int worktopWidth,
            IDictionary<int, double> x, IDictionary<int, double> y)
        {
            var roundLegBases = new DBObjectCollection();

            // В каждый словарь оснований добавляем координату центра окружности основания
            // соответствующей ножки.
            //
            int baseCenterCoordinate = DeskParameters.DistanceFromWorktopCorner + 
                (baseDiameter / 2);
            x.Add(0, baseCenterCoordinate);
            y.Add(0, baseCenterCoordinate);

            x.Add(1, baseCenterCoordinate);
            y.Add(1, worktopWidth - baseCenterCoordinate);

            // Создаем окружности основания ножек и добавляем их в массив.
            for (var i = 0; i < x.Count; i++)
            {
                var baseCircle = new Circle();
                baseCircle.SetDatabaseDefaults();
                baseCircle.Center = new Point3d(x[i], y[i], 0);
                baseCircle.Diameter = baseDiameter;
                roundLegBases.Add(baseCircle);
            }

            return roundLegBases;
        }

        /// <summary>
        /// Метод для создания квадратных оснований ножек письменного стола (если выбран тип ножек
        /// <see cref="LegType.Square"/>).
        /// </summary>
        /// <param name="baseLength"> Длина основания ножек.</param>
        /// <param name="worktopWidth"> Ширина столешницы, необходимая для расчета положения ножек
        /// относительно краев стола.</param>
        /// <param name="x"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - x-координату левого нижнего угла квадрата основания ножки.</param>
        /// <param name="y"> Словарь, содержащий в качестве ключа порядковый номер ножки, а
        /// в качестве значения - y-координату левого нижнего угла квадрата основания ножки.</param>
        /// <returns> Массив квадратных оснований ножек.</returns>
        private static DBObjectCollection CreateSquareLegBases(int baseLength, int worktopWidth,
            IDictionary<int, double> x, IDictionary<int, double> y)
        {
            var squareLegBases = new DBObjectCollection();

            // В каждый словарь оснований добавляем координату левого нижнего угла квадрата
            // основания соответствующей ножки.
            //
            x.Add(0, DeskParameters.DistanceFromWorktopCorner);
            y.Add(0, DeskParameters.DistanceFromWorktopCorner);

            x.Add(1, DeskParameters.DistanceFromWorktopCorner);
            y.Add(1, worktopWidth - DeskParameters.DistanceFromWorktopCorner - baseLength);

            // Создаем квадраты оснований ножек и добавляем их в массив.
            for (var i = 0; i < x.Count; i++)
            {
                var baseSquare = new Polyline();
                baseSquare.SetDatabaseDefaults();

                var baseSquareVertices = new[]
                {
                    new Point2d(x[i], y[i]),
                    new Point2d(x[i], y[i] + baseLength),
                    new Point2d(x[i] + baseLength, y[i] + baseLength),
                    new Point2d(x[i] + baseLength, y[i])
                };

                for (var j = 0; j < baseSquareVertices.Length; j++)
                {
                    baseSquare.AddVertexAt(j, baseSquareVertices[j], 0, 0, 0);
                }

                baseSquare.Closed = true;
                squareLegBases.Add(baseSquare);
            }

            return squareLegBases;
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
            for (var i = 0; i < drawerNumber; i++)
            {
                // Строим ящик для канцелярии.
                BuildCompositeBox(
                    drawerLength,
                    worktopWidth - DeskParameters.WorktopDrawerWidthDifference,
                    drawerHeight,
                    new Point3d(
                        worktopLength - DeskParameters.DistanceFromWorktopCorner,
                        worktopWidth - DeskParameters.DistanceFromWorktopCorner,
                        -drawerHeight * i),
                    drawerLength - DeskParameters.OuterInnerDrawerLengthDifference,
                    worktopWidth - DeskParameters.WorktopDrawerWidthDifference -
                    DeskParameters.OuterInnerDrawerWidthDifference,
                    drawerHeight - DeskParameters.OuterInnerDrawerHeightDifference,
                    new Point3d(
                        worktopLength - DeskParameters.OuterInnerDrawerLengthDifference,
                        worktopWidth - DeskParameters.OuterInnerDrawerWidthDifference,
                        -drawerHeight * i));

                // Строим дверцу ящика.
                Solid3d door = CreateAndDisplaceBox(
                    drawerLength - DeskParameters.DrawerDoorLengthDifference,
                    DeskParameters.DoorWidth,
                    drawerHeight - DeskParameters.DrawerDoorHeightDifference,
                    new Point3d(
                        worktopLength - DeskParameters.DrawerDoorLengthDifference,
                        DeskParameters.DoorWidth + DeskParameters.DistanceFromWorktopCorner,
                        -drawerHeight * i));
                AddObjectInBlock(door, DeskBlockName);

                // Строим ручку ящика.
                BuildCompositeBox(
                    (double)drawerLength / 2,
                    DeskParameters.OuterHandleWidth,
                    DeskParameters.HandleHeight,
                    new Point3d(
                        worktopLength - DeskParameters.DistanceFromWorktopCorner - 
                        drawerLength / 4,
                        DeskParameters.DistanceFromWorktopCorner,
                        -drawerHeight * i - drawerHeight / 2 + 
                        (double)DeskParameters.HandleHeight / 2),
                    drawerLength / 2 - 
                    DeskParameters.OuterInnerHandleLengthDifference,
                    DeskParameters.InnerHandleWidth,
                    DeskParameters.HandleHeight,
                    new Point3d(
                        worktopLength - DeskParameters.DistanceFromWorktopCorner - 
                        drawerLength / 4 - DeskParameters.OuterInnerHandleLengthDifference / 2,
                        DeskParameters.DistanceFromWorktopCorner,
                        -drawerHeight * i - drawerHeight / 2 + 
                        (double)DeskParameters.HandleHeight / 2));
            }
        }

        /// <summary>
        /// Метод, который создает два 3D-примитива типа "ящик", вычитает второй ящик из первого,
        /// а затем добавляет результат вычитания в блок 3D-модели письменного стола.
        /// </summary>
        /// <param name="firstBoxLength"> Длина первого ящика.</param>
        /// <param name="firstBoxWidth"> Ширина первого ящика.</param>
        /// <param name="firstBoxHeight"> Высота первого ящика.</param>
        /// <param name="firstBoxDisplacementEndpoint"> Точка чертежа, в которую необходимо
        /// переместить первый ящик.</param>
        /// <param name="secondBoxLength"> Длина второго ящика.</param>
        /// <param name="secondBoxWidth"> Ширина второго ящика.</param>
        /// <param name="secondBoxHeight"> Высота второго ящика.</param>
        /// <param name="secondBoxDisplacementEndpoint"> Точка чертежа, в которую необходимо
        /// переместить второй ящик.</param>
        private static void BuildCompositeBox(
            double firstBoxLength,
            double firstBoxWidth,
            double firstBoxHeight,
            Point3d firstBoxDisplacementEndpoint,
            double secondBoxLength,
            double secondBoxWidth,
            double secondBoxHeight,
            Point3d secondBoxDisplacementEndpoint)
        {
            // Строим внешний объект типа "ящик".
            Solid3d objectToSubstructFrom = CreateAndDisplaceBox(firstBoxLength, firstBoxWidth,
                firstBoxHeight, firstBoxDisplacementEndpoint);

            // Внутри внешнего объекта строим еще один объект того же типа.
            Solid3d substructedObject = CreateAndDisplaceBox(secondBoxLength, secondBoxWidth,
                secondBoxHeight, secondBoxDisplacementEndpoint);

            // Вычитаем внутренний объект из внешнего, добавляем результат в блок 3D-модели
            // письменного стола.
            //
            objectToSubstructFrom.BooleanOperation(BooleanOperationType.BoolSubtract,
                substructedObject);
            AddObjectInBlock(objectToSubstructFrom, DeskBlockName);
        }

        #endregion
    }
}
