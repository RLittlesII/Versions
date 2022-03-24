using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReactiveMarbles.Locator;
using ReactiveMarbles.Mvvm;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

namespace Versions.Startup
{
    /// <summary>
    /// Extensions methods for <see cref="Microsoft.Extensions.DependencyInjection"/>.
    /// </summary>
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
        /// Adds Reactive Marbles to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection with ReactiveUI dependencies registered.</returns>
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

            return serviceCollection.AddSingleton(_ => coreRegistration);
        }

        /// <summary>
        /// Adds ReactiveUI to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection with ReactiveUI dependencies registered.</returns>
        public static IServiceCollection AddReactiveUI(this IServiceCollection serviceCollection)
        {
            Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.XamForms);
            Microsoft.Extensions.Logging.LoggerFactory.Create(cfg => cfg.AddSplat());
            return serviceCollection;
        }

        /// <summary>
        /// Sets the <see cref="Locator"/> as the pass through for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The service collection with the locator dependencies registered.</returns>
        public static IServiceCollection UseServiceProviderAsLocator(this IServiceCollection serviceCollection)
        {
            serviceCollection.UseMicrosoftDependencyResolver();

            return serviceCollection;
        }

        /// <summary>
        /// Configures the <see cref="IOptions{TOptions}"/> for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="T">The option type.</typeparam>
        /// <returns>The service collection with options registered.</returns>
        public static IServiceCollection ConfigureOptions<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            serviceCollection
                .AddOptions<T>()
                .Configure((T settings, IConfiguration config) => config.Bind(settings));

            return serviceCollection;
        }

        /// <summary>
        /// Configures the app settings for the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection with ReactiveUI dependencies registered.</returns>
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
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="platformInitializer">The platform initializer.</param>
        /// <returns>The service collection with platform dependencies registered.</returns>
        public static IServiceCollection AddPlatform(this IServiceCollection serviceCollection, IPlatformInitializer platformInitializer) => platformInitializer.Initialize(serviceCollection);
    }
}