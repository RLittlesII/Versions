using System;
using Microsoft.Extensions.DependencyInjection;
using Versions.Configuration;
using Xamarin.Forms;

namespace Versions.Startup
{
    /// <inheritdoc />
    public class VersionsStartup : IStartup
    {
        private readonly IPlatformInitializer _platformInitializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionsStartup"/> class.
        /// </summary>
        /// <param name="platformInitializer">The platform initializer.</param>
        public VersionsStartup(IPlatformInitializer platformInitializer) => _platformInitializer = platformInitializer;

        /// <inheritdoc />
        public IServiceProvider ConfigureServices(IServiceCollection serviceCollection) =>
            serviceCollection
                .AddPlatform(_platformInitializer)
                .ConfigureOptions<FormsSettings>()
                .ConfigureOptions<AppSettings>()
                .AddMarbles()
                .AddReactiveUI()
                .UseServiceProviderAsLocator()
                .BuildServiceProvider();

        /// <inheritdoc />
        public Page NavigateToStart<T>() => new MainPage();
    }
}