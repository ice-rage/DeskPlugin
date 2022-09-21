using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DeskParameters.Enums;

namespace DeskParameters
{
    /// <summary>
    /// Класс для хранения параметров письменного стола.
    /// </summary>
    public class Parameters : ObservableObject
    {
        #region PrivateFields

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        private LegType _legType;

        #endregion

        #region PublicProperties

        /// <summary>
        /// Список параметров письменного стола.
        /// </summary>
        public List<ParameterGroup> ParameterGroups { get; } = new List<ParameterGroup>();

        /// <summary>
        /// Тип ножек письменного стола.
        /// </summary>
        public LegType LegType
        {
            get => _legType;

            set => SetProperty(ref _legType, value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр класса <see cref="Parameters"/>.
        /// </summary>
        public Parameters()
        {
            LegType = LegType.Round;
            ParameterGroups.Clear();
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Индексатор, позволяющий получить группу параметров письменного стола по ее наименованию.
        /// </summary>
        /// <param name="index"> Наименование группы параметров, определенный в перечислении
        /// <see cref="ParameterGroupType"/>.</param>
        /// <returns> Объект <see cref="ParameterGroup"/>, который представляет найденную группу
        /// параметров.</returns>
        public ParameterGroup this[ParameterGroupType index]
        {
            get => ParameterGroups.First(group => group.Name == index);

            set => ParameterGroups[ParameterGroups.IndexOf(ParameterGroups.First(group => 
                group.Name == index))] = value;
        }

        #endregion
    }
}
