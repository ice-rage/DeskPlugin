using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Wrappers.Enums;
using Wrappers.Interfaces;
using Wrappers.SolidCreationInfo;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using Region = Autodesk.AutoCAD.DatabaseServices.Region;

namespace Wrappers
{
    /// <summary>
    /// Класс-обертка для обращения к свойствам и методам AutoCAD .NET API.
    /// </summary>
    public class AutoCadWrapper : ICadWrapper
    {
		#region Fileds

        private readonly Dictionary<Entity, ExtrusionInfo> _extrusionInfosByEntity = 
            new Dictionary<Entity, ExtrusionInfo>();

        private readonly Dictionary<Entity, RevolutionInfo> _revolutionInfosByEntity =
            new Dictionary<Entity, RevolutionInfo>();

        private readonly DBObjectCollection _modelParts = new DBObjectCollection();

        private readonly Dictionary<Entity, FilletEdgesInfo>
            _filletEdgesInfosByEntity = new Dictionary<Entity, FilletEdgesInfo>();

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
        /// <param name="blockName"> Название блока 3D-модели, которую необходимо
        /// построить.</param>
        public AutoCadWrapper(string blockName) => BlockName = blockName;

        #endregion

        #region Methods

        #region ICadWrapper Implementation
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object CreateRectangle(
            PlaneType planeType, 
            double x, 
            double y, 
            double width, 
            double height)
        {
            // Т.к. в AutoCAD .NET API отсутствует сущность Rectangle, создаем
            // прямоугольник с помощью ломаной линии.
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

            // Замыкаем ломаную линию, чтобы позднее над ней можно было выполнять
            // различные операции как над обычным 2D-объектом.
            rectangle.Closed = true;

            // Помещаем (поворачиваем) прямоугольник так, чтобы он оказался
            // в указанной координатной плоскости.
            var rotatedRectangle = PlaceInPlane(rectangle, planeType);

            return rotatedRectangle;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object CreateCircle(PlaneType planeType, int diameter, double x, 
            double y)
        {
            var circle = new Circle();
            circle.SetDatabaseDefaults();
            circle.Center = new Point3d(x, y, 0.0);
            circle.Diameter = diameter;

            // Помещаем (поворачиваем) окружность так, чтобы она оказалась
            // в указанной координатной плоскости.
            var rotatedCircle = PlaceInPlane(circle, planeType);

            return rotatedCircle;
        }

        public object CreatePolylineWithArcSegments(PlaneType planeType,
            Dictionary<Point, double> bulgesByVertex)
        {
            var polyline = new Polyline();
            polyline.SetDatabaseDefaults();

            var vertices = new List<Point2d>();

            foreach (var buldgeByVertex in bulgesByVertex)
            {
                var vertex = new Point2d(buldgeByVertex.Key.X, buldgeByVertex.Key.Y);
                vertices.Add(vertex);
                var arcAngle = buldgeByVertex.Value;

                if (arcAngle != 0.0)
                {
                    vertices.Add(new Point2d(
                            vertex.X + Math.Cos(arcAngle * Math.PI / 180),
                            vertex.Y + Math.Sin(arcAngle * Math.PI / 180)));
                }
            }

            for (var i = 0; i < vertices.Count; i++)
            {
                var buldge = 0.0;
                var currentPoint2d = vertices[i];

                if (i != 0 && i != vertices.Count)
                {
                    var previousPoint = new Point(vertices[i - 1].X, 
                        vertices[i - 1].Y);

                    if (bulgesByVertex.ContainsKey(previousPoint) && 
                        bulgesByVertex[previousPoint] != 0.0)
                    {
                        var startPoint2d = vertices[i - 1];
                        var endPoint2d = vertices[i + 1];

                        var angleBetweenStartAndBuldgePoint = startPoint2d
                            .GetVectorTo(currentPoint2d).Angle;
                        var angleBetweenBuldgeAndEndPoint = currentPoint2d
                            .GetVectorTo(endPoint2d).Angle;
                        buldge = Math.Tan((angleBetweenBuldgeAndEndPoint - 
                            angleBetweenStartAndBuldgePoint) / 2.0);
                    }
                }

                polyline.AddVertexAt(i, currentPoint2d, buldge, 0.0, 0.0);
            }

            polyline.Closed = true;
            var rotatedPolyline = PlaceInPlane(polyline, planeType);

            return rotatedPolyline;
        }

        public object Move(object obj, Point3D basePoint, Point3D displacementPoint)
        {
            var entity = (Entity)obj;
            var startPoint = new Point3d(basePoint.X, basePoint.Y, basePoint.Z);
            var displacementVector = startPoint.GetVectorTo(
                new Point3d(displacementPoint.X, displacementPoint.Y, 
                    displacementPoint.Z));

            entity.TransformBy(Matrix3d.Displacement(displacementVector));

            return entity;
        }

        public object Rotate(object obj, Vector3D rotationAxis, double rotationAngle, 
            Point3D rotationPoint)
        {
            var entity = (Entity)obj;
            var axis = new Vector3d(rotationAxis.X, rotationAxis.Y, rotationAxis.Z);
            var point = new Point3d(rotationPoint.X, rotationPoint.Y, rotationPoint.Z);

            entity.TransformBy(Matrix3d.Rotation(rotationAngle * Math.PI / 180,
                axis, point));

            return entity;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Extrude(
            object obj, 
            double height, 
            double taperAngle = 0.0,
            bool isExtrusionCuttingOut = false,
            bool isDirectionPositive = true) => 
            _extrusionInfosByEntity.Add((Entity)obj, new ExtrusionInfo(
                height, 
                taperAngle, 
                isExtrusionCuttingOut, 
                isDirectionPositive));

        public void Revolve(
            object obj, 
            Point3D startAxisPoint, 
            Point3D endAxisPoint, 
            double angle = 360) => _revolutionInfosByEntity.Add((Entity)obj, 
                new RevolutionInfo(startAxisPoint, endAxisPoint, angle));

        public void FilletEdges(
            object obj, 
            double radius, 
            double startSetback, 
            double endSetback) => _filletEdgesInfosByEntity.Add((Entity)obj, 
                new FilletEdgesInfo(radius, startSetback, endSetback));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Build()
        {
            // При пустом словаре сущностей построение 3D-модели не выполняется.
            if (_extrusionInfosByEntity.Count == 0)
            {
                return;
            }

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (var database = Application.DocumentManager.MdiActiveDocument
                           .Database)
                {
                    using (var transaction = database.TransactionManager
                               .StartTransaction())
                    {
                        var blockTableId = database.BlockTableId;

                        CreateBlock(blockTableId);

                        // Применяем операцию выдавливания (и, при необходимости, 
                        // операцию скругления ребер) ко всем созданным сущностям.
                        _extrusionInfosByEntity.Keys
                            .ToList()
                            .ForEach(CreateSolid);

                        _revolutionInfosByEntity.Keys
                            .ToList()
                            .ForEach(CreateSolid);

                        // Перед добавлением 3D-объектов в документ AutoCAD
                        // проверяем, нужно ли применять операцию вычитания
                        // к некоторым объектам.
                        //
                        for (var i = 0; i < _modelParts.Count; i++)
                        {
                            var modelPart = _modelParts[i];

                            var modelPartExtents = ((Solid3d)modelPart)
                                .GeometricExtents;
                            var minPoint = modelPartExtents.MinPoint;
                            var maxPoint = modelPartExtents.MaxPoint;

                            // Выбираем минимальные и максимальные координаты тела.
                            //
                            var xMin = (int)minPoint.X;
                            var xMax = (int)maxPoint.X;

                            var yMin = (int)minPoint.Y;
                            var yMax = (int)maxPoint.Y;

                            var zMin = (int)minPoint.Z;
                            var zMax = (int)maxPoint.Z;

                            // Проверяем остальные тела на содержание внутри
                            // текущего тела и в случае успеха применяем операцию
                            // вычитания к этим телам, при этом удаляя тело,
                            // содержащееся в "главном", из общего массива тел,
                            // подлежащих добавлению в документ AutoCAD.
                            // 
                            foreach (DBObject otherPart in _modelParts)
                            {
                                var points = new List<Point3d>();

                                using (var brep = new Brep((Entity)otherPart))
                                {
                                    points.AddRange(brep.Vertices
                                        .Select(vertex => vertex.Point));
                                }

                                if (otherPart == modelPart ||
                                    !points.All(point => xMin <= (int)point.X
                                        && (int)point.X <= xMax) ||
                                    !points.All(point => yMin <= (int)point.Y
                                        && (int)point.Y <= yMax) ||
                                    !points.All(point => zMin <= (int)point.Z
                                        && (int)point.Z <= zMax))
                                {
                                    continue;
                                }

                                ((Solid3d)modelPart).BooleanOperation(
                                    BooleanOperationType.BoolSubtract, 
                                    (Solid3d)otherPart);
                                _modelParts.Remove(otherPart);
                            }
                        }

                        _extrusionInfosByEntity.Clear();
                        _revolutionInfosByEntity.Clear();
                        _filletEdgesInfosByEntity.Clear();
                        _modelParts.Clear();

                        transaction.Commit();
                    }
                }
            }
        }

        #endregion

        private void CreateSolid(Entity entity)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (var database = Application.DocumentManager.MdiActiveDocument
                           .Database)
                {
                    using (var transaction = database.TransactionManager
                               .StartTransaction())
                    {
                        var solid = new Solid3d();
                        var curves = new DBObjectCollection();
                        curves.Add(entity);

                        // Для выполнения операции выдавливания 2D-объект должен представлять
                        // собой замкнутую область, поэтому предварительно получаем область
                        // (region) из кривых, образующих объект.
                        var regions = Region.CreateFromCurves(curves);

                        using (var region = (Region)regions[0])
                        {
                            if (_extrusionInfosByEntity.ContainsKey(entity))
                            {
                                var extrusionInfo = _extrusionInfosByEntity[entity];
                                solid.Extrude(
                                    region,
                                    extrusionInfo.IsDirectionPositive
                                        ? extrusionInfo.Height
                                        : -extrusionInfo.Height,
                                    extrusionInfo.TaperAngle);
                            }
                            else if (_revolutionInfosByEntity.ContainsKey(entity))
                            {
                                var revolutionInfo = _revolutionInfosByEntity[entity];
                                var axisPoint = new Point3d(
                                    revolutionInfo.StartAxisPoint.X,
                                    revolutionInfo.StartAxisPoint.Y,
                                    revolutionInfo.StartAxisPoint.Z);
                                var axisDirection = axisPoint
                                    .GetVectorTo(new Point3d(
                                        revolutionInfo.EndAxisPoint.X, 
                                        revolutionInfo.EndAxisPoint.Y, 
                                        revolutionInfo.EndAxisPoint.Z));

                                solid.Revolve(
                                    region, 
                                    axisPoint, 
                                    axisDirection, 
                                    revolutionInfo.Angle * Math.PI / 180);
                            }
                            else
                            {
                                return;
                            }

                            AddObjectInBlock(solid, BlockName);

                            if (_filletEdgesInfosByEntity.ContainsKey(entity))
                            {
                                FilletEdges(
                                    solid, 
                                    _filletEdgesInfosByEntity[entity].Radius, 
                                    _filletEdgesInfosByEntity[entity].StartSetback, 
                                    _filletEdgesInfosByEntity[entity].EndSetback);
                            }

                            _modelParts.Add(solid);
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Метод для размещения некоторой сущности в указанной координатной
        /// плоскости.
        /// </summary>
        /// <param name="entity"> Сущность, которую необходимо поместить в указанную
        /// плоскость.</param>
        /// <param name="planeType"> Координатная плоскость из перечисления
        /// <see cref="PlaneType"/>.</param>
        /// <returns> Сущность, помещенная в указанную плоскость.</returns>
        private object PlaceInPlane(Entity entity, PlaneType planeType)
        {
            Vector3D axis;
            var rotationAngle = 0.0;
            var rotationPoint = new Point3D(Point3d.Origin.X, Point3d.Origin.Y, 
                Point3d.Origin.Z);

            // В зависимости от указанной координатной плоскости определяем, вокруг
            // какой оси необходимо выполнить поворот прямоугольника, чтобы
            // поместить его в эту плоскость (по умолчанию текущей плоскостью
            // является xOy).
            switch (planeType)
            {
                case PlaneType.XoZ:
                {
                    axis = new Vector3D(1, 0, 0);
                    rotationAngle = -90;
                    break;
                }
                case PlaneType.YoZ:
                {
                    axis = new Vector3D(0, 1, 0);
                    rotationAngle = -90;
                    break;
                }
                default:
                {
                    axis = new Vector3D(0, 0, 0);
                    break;
                }
            }

            var rotatedEntity = Rotate(entity, axis, rotationAngle, 
                rotationPoint);

            return rotatedEntity;
        }

        /// <summary>
        /// Метод для создания блока, содержащего 3D-модель письменного стола.
        /// </summary>
        /// <param name="blockTableId"> Идентификатор таблицы блоков из базы данных
        /// активного документа AutoCAD.</param>
        private void CreateBlock(ObjectId blockTableId)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (var database = Application.DocumentManager.MdiActiveDocument
                           .Database)
                {
                    using (var transaction = database.TransactionManager
                               .StartTransaction())
                    {
                        // Если 3D-модель письменного стола уже была создана,
                        // удаляем ее.
                        EraseExisting3dModel(transaction, blockTableId);

                        var blockTable = (BlockTable)transaction
                            .GetObject(blockTableId, OpenMode.ForWrite);

                        // Создаем в таблице блоков определение нового блока,
                        // присваиваем ему имя.
                        var blockTableRecord = new BlockTableRecord
                        {
                            Name = BlockName
                        };

                        // Добавляем созданное определение блока в таблицу блоков
                        // и в транзакцию, запоминаем идентификатор этого
                        // определения.
                        //
                        var deskBlockTableRecordId = blockTable
                            .Add(blockTableRecord);
                        transaction.AddNewlyCreatedDBObject(blockTableRecord, true);

                        // Открываем пространство моделей для записи и создаем в нем
                        // новое вхождение блока, используя идентификатор
                        // определения этого блока.
                        //
                        var deskBlockReference = new BlockReference(Point3d.Origin,
                            deskBlockTableRecordId);

                        AddObjectInBlock(deskBlockReference, 
                            BlockTableRecord.ModelSpace);

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Метод для удаления существующей 3D-модели.
        /// </summary>
        /// <param name="transaction"> Текущая транзакция.</param>
        /// <param name="blockTableId"> Идентификатор таблицы блоков из базы данных
        /// активного документа AutoCAD.</param>
        private void EraseExisting3dModel(Transaction transaction, 
            ObjectId blockTableId)
        {
            var deskBlockId = ObjectId.Null;

            // Если ранее уже был создан блок, содержащий 3D-модель, получаем
            // идентификатор этого блока.
            //
            var blockTable = transaction.GetObject(blockTableId, OpenMode.ForRead) 
                as BlockTable;

            if (blockTable != null && blockTable.Has(BlockName))
            {
                deskBlockId = blockTable[BlockName];
            }

            if (deskBlockId.IsNull)
            {
                return;
            }

            // Прежде чем удалять определение блока из таблицы блоков, удаляем все
            // его вхождения, чтобы предотвратить повреждение чертежа.
            //
            var deskBlockTableRecord = (BlockTableRecord)transaction
                .GetObject(deskBlockId, OpenMode.ForRead);

            var deskBlockReferenceIds = deskBlockTableRecord
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

            // После удаления всех вхождений блока из поля чертежа удаляем его
            // определение.
            //
            deskBlockReferenceIds = deskBlockTableRecord
                .GetBlockReferenceIds(true, false);

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
        /// <param name="blockName"> Имя блока, в который нужно добавить объект.
        /// </param>
        private static void AddObjectInBlock(DBObject databaseObject, 
            string blockName)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (var database = Application.DocumentManager.MdiActiveDocument
                    .Database)
                {
                    using (var transaction = database.TransactionManager
                        .StartTransaction())
                    {
                        // Открываем для записи определенный блок из таблицы блоков.
                        //
                        var blockTable = (BlockTable)transaction.GetObject(database
                                .BlockTableId, OpenMode.ForRead);
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

        private void FilletEdges(Solid3d solid, double radius, double startSetback,
            double EndSetback)
        {
            // Выделяем из твердого тела идентификаторы всех его подсущностей.
            var solidIds = new[]
            {
                solid.ObjectId
            };

            // Определяем идентификатор основной подсущности твердого тела.
            var solidSubentityId = new SubentityId(SubentityType.Null, IntPtr.Zero);

            // Получаем полный путь к основной подсущности твердого тела из массива
            // идентификаторов его подсущностей.
            var solidSubentityPath = new FullSubentityPath(solidIds, solidSubentityId);

            // Определяем список идентификаторов подсущностей определенного типа
            // (в данном случае - ребер).
            var solidEdgeIds = new List<SubentityId>();

            // Определяем числовые коллекции радиусов скругления ребер твердого тела,
            // а также начальной и конечной точки отступа.
            var filletRadius = new DoubleCollection();
            var filletStartSetback = new DoubleCollection();
            var filletEndSetback = new DoubleCollection();

            using (var brep = new Brep(solidSubentityPath))
            {
                // Выбираем из всех ребер твердого тела нужные для скругления (только
                // те, которые относятся к передней грани тела относительно оси xOz).
                brep.Edges.Where((edge, index) => index != 0 && index % 2 == 0)
                    .Select(edge => edge.SubentityPath.SubentId)
                    .ToList()
                    .ForEach(subentityId =>
                    {
                        solidEdgeIds.Add(subentityId);

                        // Добавляем в соответствующие числовые коллекции радиус
                        // скругления данного ребра, начало и конец отступа скругления.
                        //
                        filletRadius.Add(radius);
                        filletStartSetback.Add(startSetback);
                        filletEndSetback.Add(EndSetback);
                    });
            }

            solid.FilletEdges(solidEdgeIds.ToArray(), filletRadius,
                filletStartSetback, filletEndSetback);
        }

        #endregion
    }
}