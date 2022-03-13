using System;
using Microsoft.Extensions.DependencyInjection;
using Versions.Configuration;
using Xamarin.Forms;

namespace Versions.Startup
{
    public class VersionsStartup : IStartup
    {
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
        public Page NavigateToStart<T>()
        {
            return null;
        }

        private readonly IPlatformInitializer _platformInitializer;
    }
}