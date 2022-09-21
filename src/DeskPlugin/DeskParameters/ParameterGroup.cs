using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DeskParameters.Enums;

namespace DeskParameters
{
    /// <summary>
    /// Класс для хранения группы параметров письменного стола.
    /// </summary>
    public class ParameterGroup
    {
        #region PublicProperties

        /// <summary>
        /// Название группы параметров.
        /// </summary>
        public ParameterGroupType Name { get; }

        /// <summary>
        /// Параметры, входящие в данную группу.
        /// </summary>
        public ObservableCollection<Parameter> Parameters { get; } =
            new ObservableCollection<Parameter>();

        #endregion

        #region Constructors

        /// <summary>
        /// Создает экземпляр класса <see cref="ParameterGroup"/>.
        /// </summary>
        /// <param name="group"> Название группы параметров.</param>
        /// <param name="parameters"> Перечислимая коллекция параметров, входящих в данную группу.
        /// </param>
        public ParameterGroup(ParameterGroupType group, IEnumerable<Parameter> parameters)
        {
            Name = group;
            parameters
                .ToList()
                .ForEach(parameter => Parameters.Add(parameter));
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Индексатор, позволяющий получить параметр письменного стола по его типу.
        /// </summary>
        /// <param name="index"> Тип параметра, определенный в перечислении
        /// <see cref="ParameterType"/>.
        /// </param>
        /// <returns> Объект класса <see cref="Parameter"/>, который представляет найденный
        /// параметр.
        /// </returns>
        public Parameter this[ParameterType index]
        {
            get => Parameters.First(parameter => parameter.Name == index);

            set => Parameters[Parameters.IndexOf(Parameters.First(parameter => 
                parameter.Name == index))] = value;
        }

        #endregion
    }
}
