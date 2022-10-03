using System.Windows.Media.Media3D;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Services.Interfaces;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Services
{
    public class AutoCadWrapper : ICadWrapper
    {
        #region Fileds

        /// <summary>
        /// Массив объектов базы данных документа AutoCAD, хранящий части (детали) 3D-модели.
        /// </summary>
        private readonly DBObjectCollection _3dModelParts = new DBObjectCollection();

        #endregion

        #region Properties

        /// <summary>
        /// Название блока, содержащего 3D-модель.
        /// </summary>
        public string BlockName { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр <see cref="AutoCadWrapper"/>.
        /// </summary>
        /// <param name="blockName"> Название блока 3D-модели, которую необходимо построить.</param>
        public AutoCadWrapper(string blockName)
        {
            BlockName = blockName;
        }

        #endregion

        #region Methods

        #region Interface Implementation

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void BuildSimpleBox(double boxLength, double boxWidth, double boxHeight,
            Point3D displacementEndPoint)
        {
            Solid3d box = CreateAndDisplaceBox(boxLength, boxWidth, boxHeight, 
                new Point3d(displacementEndPoint.X, displacementEndPoint.Y, 
                    displacementEndPoint.Z));
            _3dModelParts.Add(box);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void BuildCompositeBox(
            double firstBoxLength,
            double firstBoxWidth,
            double firstBoxHeight,
            Point3D firstBoxDisplacementEndpoint,
            double secondBoxLength,
            double secondBoxWidth,
            double secondBoxHeight,
            Point3D secondBoxDisplacementEndpoint)
        {
            // Строим внешний объект типа "ящик".
            Solid3d objectToSubstructFrom = CreateAndDisplaceBox(firstBoxLength, firstBoxWidth,
                firstBoxHeight, new Point3d(firstBoxDisplacementEndpoint.X,
                    firstBoxDisplacementEndpoint.Y, firstBoxDisplacementEndpoint.Z));

            // Внутри внешнего объекта строим еще один объект того же типа.
            Solid3d substructedObject = CreateAndDisplaceBox(secondBoxLength, secondBoxWidth,
                secondBoxHeight, new Point3d(secondBoxDisplacementEndpoint.X,
                    secondBoxDisplacementEndpoint.Y, secondBoxDisplacementEndpoint.Z));

            // Вычитаем внутренний объект из внешнего, добавляем результат в блок 3D-модели
            // письменного стола.
            //
            objectToSubstructFrom.BooleanOperation(BooleanOperationType.BoolSubtract,
                substructedObject);
            _3dModelParts.Add(objectToSubstructFrom);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object CreateCircle(int diameter, double x, double y)
        {
            var circle = new Circle();
            circle.SetDatabaseDefaults();
            circle.Center = new Point3d(x, y, 0);
            circle.Diameter = diameter;

            return circle;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object CreateSquare(int sideLength, double x, double y)
        {
            var square = new Polyline();
            square.SetDatabaseDefaults();

            var baseSquareVertices = new[]
            {
                new Point2d(x, y),
                new Point2d(x, y + sideLength),
                new Point2d(x + sideLength, y + sideLength),
                new Point2d(x + sideLength, y)
            };

            for (var j = 0; j < baseSquareVertices.Length; j++)
            {
                square.AddVertexAt(j, baseSquareVertices[j], 0, 0, 0);
            }

            square.Closed = true;

            return square;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Extrude(object obj, double height)
        {
            var curves = new DBObjectCollection();
            curves.Add((DBObject)obj);

            DBObjectCollection regions = Region.CreateFromCurves(curves);

            using (var region = (Region)regions[0])
            {
                var solid = new Solid3d();
                solid.Extrude(region, height, 0);
                _3dModelParts.Add(solid);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Build()
        {
            if (_3dModelParts.Count == 0)
            {
                return;
            }

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Database database = Application.DocumentManager.MdiActiveDocument.Database)
                {
                    using (Transaction transaction = database.TransactionManager.StartTransaction())
                    {
                        ObjectId blockTableId = database.BlockTableId;

                        CreateBlock(blockTableId);

                        foreach (DBObject modelPart in _3dModelParts)
                        {
                            AddObjectInBlock(modelPart, BlockName);
                        }

                        // Очищаем массив частей (деталей) 3D-модели перед последующим возможным
                        // построением.
                        _3dModelParts.Clear();

                        transaction.Commit();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Метод для создания блока, содержащего 3D-модель письменного стола.
        /// </summary>
        /// <param name="blockTableId"> Идентификатор таблицы блоков из базы данных активного
        /// документа AutoCAD.</param>
        private void CreateBlock(ObjectId blockTableId)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Database database = Application.DocumentManager.MdiActiveDocument.Database)
                {
                    using (Transaction transaction = database.TransactionManager.StartTransaction())
                    {
                        // Если 3D-модель письменного стола уже была создана, удаляем ее.
                        EraseExisting3dModel(transaction, blockTableId);

                        var blockTable = (BlockTable)transaction.GetObject(blockTableId,
                            OpenMode.ForWrite);

                        // Создаем в таблице блоков определение нового блока, присваиваем ему имя.
                        var blockTableRecord = new BlockTableRecord
                        {
                            Name = BlockName
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
        /// Метод для удаления существующей 3D-модели.
        /// </summary>
        /// <param name="transaction"> Текущая транзакция.</param>
        /// <param name="blockTableId"> Идентификатор таблицы блоков из базы данных активного
        /// документа AutoCAD.</param>
        private void EraseExisting3dModel(Transaction transaction, ObjectId blockTableId)
        {
            ObjectId deskBlockId = ObjectId.Null;

            // Если ранее уже был создан блок, содержащий 3D-модель, получаем идентификатор этого
            // блока.
            //
            var blockTable = transaction.GetObject(blockTableId, OpenMode.ForRead) as BlockTable;

            if (blockTable != null && blockTable.Has(BlockName))
            {
                deskBlockId = blockTable[BlockName];
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

        #endregion
    }
}
