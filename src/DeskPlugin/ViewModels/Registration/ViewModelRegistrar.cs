using Microsoft.Extensions.DependencyInjection;

namespace ViewModels.Registration
{
    /// <summary>
    /// Статический класс для регистрации моделей представления плагина в контейнере
    /// сервисов.
    /// </summary>
    public static class ViewModelRegistrar
    {
        /// <summary>
        /// Метод, регистрирующий модели представления и определяющий их жизненный
        /// цикл в рамках работы плагина.
        /// </summary>
        /// <param name="services"> Указанный контейнер сервисов.</param>
        /// <returns> Контейнер сервисов плагина с добавленными моделями
        /// представления.</returns>
        public static IServiceCollection RegisterViewModels(
            this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>();
    }
}
