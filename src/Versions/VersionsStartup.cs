using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace Versions
{
    public class VersionsStartup : IStartup
    {
        public VersionsStartup(IPlatformInitializer platformInitializer) => _platformInitializer = platformInitializer;

        /// <inheritdoc />
        public IServiceProvider ConfigureServices(IServiceCollection serviceCollection) =>
            serviceCollection
                .AddPlatform(_platformInitializer)
                .BuildServiceProvider();

        /// <inheritdoc />
        public Page NavigateToStart<T>()
        {
            return null;
        }

        private readonly IPlatformInitializer _platformInitializer;
    }
}