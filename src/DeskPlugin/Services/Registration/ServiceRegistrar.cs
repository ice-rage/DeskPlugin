using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;

namespace Services.Registration
{
    /// <summary>
    /// Статический класс для регистрации сервисов, используемых плагином,
    /// в контейнере сервисов.
    /// </summary>
    public static class ServiceRegistrar
    {
        /// <summary>
        /// Метод, регистрирующий сервисы и определяющий их жизненный цикл в рамках
        /// выполнения плагина.
        /// </summary>
        /// <param name="services"> Указанный контейнер сервисов.</param>
        /// <returns> Коллекция сервисов плагина с добавленными сервисами.</returns>
        public static IServiceCollection RegisterServices(
            this IServiceCollection services) => services
            .AddSingleton<ICadWrapper>(_ => new AutoCadWrapper("Desk"));
    }
}
