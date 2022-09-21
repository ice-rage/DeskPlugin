using Autodesk.AutoCAD.DatabaseServices;
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
