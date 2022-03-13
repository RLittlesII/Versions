using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveMarbles.Locator;
using ReactiveMarbles.Mvvm;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

namespace Versions
{
    public static class MicrosoftDependencyInjectionExtensions
    {
        /// <summary>
        /// Register a view model and a view for navigation.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="TView">The view type.</typeparam>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>The container collection.</returns>
        public static IServiceCollection RegisterForNavigation<TView, TViewModel>(this IServiceCollection serviceCollection)
            where TView : class, IViewFor<TViewModel>
            where TViewModel : class
        {
            serviceCollection.AddTransient<IViewFor<TViewModel>, TView>();
            serviceCollection.AddTransient<TViewModel>();
            return serviceCollection;
        }

        /// <summary>
        /// Registers <see cref="Sextant"/> to the container.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The container collection.</returns>
        public static IServiceCollection AddSextant(this IServiceCollection serviceCollection) =>
            serviceCollection;
                // .AddSingleton<IViewLocator>(_ => ViewLocator.Current)
                // .AddSingleton<IViewModelFactory, PuppyViewModelFactory>()
                // .AddSingleton<IView>(provider => new NavigationView(RxApp.MainThreadScheduler, RxApp.TaskpoolScheduler, provider.GetService<IViewLocator>()!))
                // .AddSingleton<IPopupNavigation>(PopupNavigation.Instance)
                // .AddSingleton<IPopupViewStackService, PopupViewStackService>();

        public static IServiceCollection AddMarbles(this IServiceCollection serviceCollection)
        {
            var coreRegistration = CoreRegistrationBuilder
                .Create()
                .WithMainThreadScheduler(RxApp.MainThreadScheduler)
                .WithTaskPoolScheduler(RxApp.TaskpoolScheduler)
                .Build();

            ServiceLocator
                .Current()
                .AddCoreRegistrations(() => coreRegistration);

            return serviceCollection.AddSingleton<ICoreRegistration>(_ => coreRegistration);
        }

        public static IServiceCollection AddReactiveUI(this IServiceCollection serviceCollection)
        {
            Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.XamForms);
            Microsoft.Extensions.Logging.LoggerFactory.Create(cfg => cfg.AddSplat());
            return serviceCollection;
        }


        public static IServiceCollection UseServiceProviderAsLocator(this IServiceCollection serviceCollection)
        {
            serviceCollection.UseMicrosoftDependencyResolver();

            return serviceCollection;
        }

        public static IServiceCollection ConfigureOptions<T>(this IServiceCollection serviceCollection) where T : class
        {
            serviceCollection
                .AddOptions<T>()
                .Configure((T settings, IConfiguration config) => config.Bind(settings));

            return serviceCollection;
        }

        public static IServiceCollection ConfigureAppSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.dev.json", optional: true);

            serviceCollection.AddSingleton<IConfiguration>(_ => builder.AddConfiguration(configuration).Build());

            return serviceCollection;
        }

        /// <summary>
        /// Adds the <see cref="IPlatformInitializer"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="platformInitializer"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlatform(this IServiceCollection serviceCollection, IPlatformInitializer platformInitializer) => platformInitializer.Initialize(serviceCollection);
    }
}