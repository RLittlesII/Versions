using Microsoft.Extensions.DependencyInjection;

namespace Versions.Startup
{
    /// <summary>
    /// Represents an initializer for a specific platform.
    /// </summary>
    public interface IPlatformInitializer
    {
        /// <summary>
        /// Initialized the platform.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public IServiceCollection Initialize(IServiceCollection serviceCollection);
    }
}