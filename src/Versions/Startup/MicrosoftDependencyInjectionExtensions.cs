using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReactiveMarbles.Locator;
using ReactiveMarbles.Mvvm;
using ReactiveUI;
using Rg.Plugins.Popup.Services;
using Serilog;
using Sextant.Plugins.Popup;
using Sextant.XamForms;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using Splat.Serilog;
using Versions.Configuration;
using ILogger = Serilog.ILogger;
using IView = Sextant.IView;
using IViewModelFactory = Sextant.IViewModelFactory;

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
        public static IServiceCollection RegisterForNavigation<TView, TViewModel>(
            this IServiceCollection serviceCollection)
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
        /// Registers <see cref="Serilog"/> to the container.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="factory">The logger factory.</param>
        /// <returns>The container collection.</returns>
        public static IServiceCollection AddSerilog(
            this IServiceCollection serviceCollection,
            Func<LoggerConfiguration> factory)
        {
            Serilog.Log.Logger = factory().CreateLogger();

            Locator.CurrentMutable.UseSerilogFullLogger(Serilog.Log.Logger);

            return serviceCollection.AddSingleton<Serilog.ILogger>(Serilog.Log.Logger);
        }

        /// <summary>
        /// Registers <see cref="Serilog"/> to the container.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The container collection.</returns>
        public static IServiceCollection AddSerilog(this IServiceCollection serviceCollection)
        {
            ILogger GenerateLogger(IServiceProvider provider)
            {
                var appCenterKey = provider.GetService<IOptions<FormsSettings>>() !.Value.AppCenterKey;

                var appCenterCrashes =
                    provider.GetService<LoggerConfiguration>() !.WriteTo.AppCenterCrashes(appCenterKey);
                var logger = appCenterCrashes.CreateLogger();

                Locator.CurrentMutable.UseSerilogFullLogger(logger);
                Log.Logger = logger;
                return Log.Logger;
            }

            return serviceCollection.AddSingleton<Serilog.ILogger>(GenerateLogger);
        }

        /// <summary>
        /// Registers <see cref="Serilog"/> to the container.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The logger factory.</param>
        /// <returns>The container collection.</returns>
        public static IServiceCollection AddSerilogConfiguration(
            this IServiceCollection serviceCollection,
            LoggerConfiguration configuration) => serviceCollection.AddSingleton<LoggerConfiguration>(configuration);

        /// <summary>
        /// Registers <see cref="Sextant"/> to the container.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The container collection.</returns>
        public static IServiceCollection AddSextant(this IServiceCollection serviceCollection) =>
            serviceCollection
                .AddSingleton(_ => ViewLocator.Current)
                .AddSingleton<IViewModelFactory, ViewModelFactory>()
                .AddSingleton<IView>(provider =>
                    new NavigationView(RxApp.MainThreadScheduler, RxApp.TaskpoolScheduler, provider.GetService<IViewLocator>() !))
                .AddSingleton(PopupNavigation.Instance)
                .AddSingleton<IPopupViewStackService, PopupViewStackService>();

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
        public static IServiceCollection ConfigureAppSettings(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
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
        public static IServiceCollection AddPlatform(
            this IServiceCollection serviceCollection,
            IPlatformInitializer platformInitializer) => platformInitializer.Initialize(serviceCollection);
    }
}