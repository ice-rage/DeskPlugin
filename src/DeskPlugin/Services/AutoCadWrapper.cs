using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Services.Enums;
using Services.Interfaces;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Services
{
    /// <summary>
    /// Класс-обертка для обращения к свойствам и методам AutoCAD .NET API.
    /// </summary>
    public class AutoCadWrapper : ICadWrapper
    {
		#region Fileds

		/// <summary>
		/// Массив объектов базы данных документа AutoCAD, хранящий части (детали) 3D-модели.
		/// </summary>
		// TODO: просто _modelParts или _modelParts3D
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

        #region ICadWrapper Implementation

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object CreateRectangle(PlaneType planeType, double x, double y, double width, 
            double height)
        {
            // Т.к. в AutoCAD .NET API отсутствует сущность Rectangle, создаем прямоугольник 
            // с помощью ломаной линии.
            //
            var rectangle = new Polyline();
            rectangle.SetDatabaseDefaults();

            var baseSquareVertices = new[]
            {
                new Point2d(x, y),
                new Point2d(x, y + height),
                new Point2d(x + width, y + height),
                new Point2d(x + width, y)
            };

            for (var j = 0; j < baseSquareVertices.Length; j++)
            {
                rectangle.AddVertexAt(j, baseSquareVertices[j], 0, 0, 0);
            }

            // Замыкаем ломаную линию, чтобы позднее над прямоугольником можно было выполнять
            // различные операции как над обычным 2D-объектом.
            rectangle.Closed = true;

            // Помещаем (поворачиваем) прямоугольник так, чтобы он оказался в указанной
            // координатной плоскости.
            Entity rotatedRectangle = PlaceInPlane(rectangle, planeType);

            return rotatedRectangle;
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
        public void Extrude(object obj, double height, bool cuttingByExtrusion = false,
            bool isPositiveDirection = true)
        {
            var curves = new DBObjectCollection();
            curves.Add((DBObject)obj);

			// Для выполнения операции выдавливания 2D-объект должен представлять собой замкнутую
			// область, поэтому предварительно получаем область (region) из кривых, образующих
			// объект.
			// TODO: Почему не var?
			DBObjectCollection regions = Region.CreateFromCurves(curves);

            using (var region = (Region)regions[0])
            {
                var solid = new Solid3d();
                solid.Extrude(region, isPositiveDirection ? height : -height, 0);
                _3dModelParts.Add(solid);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Build()
        {
            // При пустом массиве деталей построение 3D-модели не выполняется.
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

                        // Перед добавлением 3D-объектов в документ AutoCAD проверяем, нужно ли
                        // применять операцию вычитания к некоторым объектам
                        //
                        for (var i = 0; i < _3dModelParts.Count; i++)
                        {
                            DBObject modelPart = _3dModelParts[i];

                            Extents3d modelPartExtents = modelPart.Bounds.GetValueOrDefault();
                            Point3d minPoint = modelPartExtents.MinPoint;
                            Point3d maxPoint = modelPartExtents.MaxPoint;

                            // Выбираем минимальные и максимальные координаты тела
                            //
                            var xMin = (int)minPoint.X;
                            var xMax = (int)maxPoint.X;

                            var yMin = (int)minPoint.Y;
                            var yMax = (int)maxPoint.Y;

                            var zMin = (int)minPoint.Z;
                            var zMax = (int)maxPoint.Z;

                            // Проверяем остальные тела на содержание внутри текущего тела, и 
                            // в случае успеха применяем операцию вычитания к этим телам, при этом
                            // удаляя тело, содержащееся в "главном", из общего массива тел,
                            // подлежащих добавлению в документ AutoCAD
                            // 
                            foreach (DBObject otherPart in _3dModelParts)
                            {
                                var points = new List<Point3d>();

                                using (var brep = new Brep((Entity)otherPart))
                                {
                                    points.AddRange(brep.Vertices.Select(vertex => vertex.Point));
                                }

                                if (otherPart == modelPart ||
                                    !points.All(point => xMin <= (int)point.X && (int)point.X <= xMax) ||
                                    !points.All(point => yMin <= (int)point.Y && (int)point.Y <= yMax) ||
                                    !points.All(point => zMin <= (int)point.Z && (int)point.Z <= zMax))
                                {
                                    continue;
                                }

                                ((Solid3d)modelPart).BooleanOperation(BooleanOperationType
                                    .BoolSubtract, (Solid3d)otherPart);
                                _3dModelParts.Remove(otherPart);
                            }

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
        /// Метод для размещения некоторой сущности в указанной координатной плоскости.
        /// </summary>
        /// <param name="entity"> Сущность, которую необходимо поместить в указанную плоскость.
        /// </param>
        /// <param name="planeType"> Координатная плоскость из перечисления <see cref="PlaneType"/>.
        /// </param>
        /// <returns> Сущность, помещенная в указанную плоскость.</returns>
        private static Entity PlaceInPlane(Entity entity, PlaneType planeType)
        {
            // Получаем текущую систему координат активного документа AutoCAD.
            Document activeDocument = Application.DocumentManager.MdiActiveDocument;
            Matrix3d currentUcs = activeDocument.Editor.CurrentUserCoordinateSystem;
            CoordinateSystem3d coordinateSystem3d = currentUcs.CoordinateSystem3d;

            Vector3d axis;
            var rotationDegree = 0.0;

            // В зависимости от указанной координатной плоскости определяем, вокруг какой оси 
            // необходимо выполнить поворот прямоугольника, чтобы поместить его в эту плоскость
            // (по умолчанию текущей плоскостью является xOy).
            switch (planeType)
            {
                case PlaneType.XoZ:
                {
                    axis = coordinateSystem3d.Xaxis;
                    rotationDegree = -90;
                    break;
                }

                case PlaneType.YoZ:
                {
                    axis = coordinateSystem3d.Yaxis;
                    break;
                }

                default:
                {
                    axis = new Vector3d(0, 0, 0);
                    break;
                }
            }

            Matrix3d rotationMatrix = Matrix3d.Rotation(rotationDegree * (Math.PI / 180), axis,
                Point3d.Origin);
            entity.TransformBy(rotationMatrix);

            return entity;
        }

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

        #endregion
    }
}
