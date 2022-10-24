using Microsoft.Extensions.DependencyInjection;
using Wrappers.Interfaces;

namespace Wrappers.Registration
{
    /// <summary>
    /// Статический класс для регистрации сервисов, используемых плагином,
    /// в контейнере сервисов.
    /// </summary>
    public static class WrapperRegistrar
    {
        /// <summary>
        /// Метод, регистрирующий классы-обертки для используемых САПР и определяющий
        /// их жизненный цикл в рамках работы плагина.
        /// </summary>
        /// <param name="services"> Указанный контейнер сервисов плагина.</param>
        /// <returns> Контейнер сервисов плагина с добавленными классами-обертками.
        /// </returns>
        public static IServiceCollection RegisterWrappers(
            this IServiceCollection services) => services
            .AddSingleton<ICadWrapper>(_ => new AutoCadWrapper("Desk"));
    }
}
