using System;
using Microsoft.Extensions.DependencyInjection;
using Sextant;
using Xamarin.Forms;

namespace Versions.Startup
{
    /// <summary>
    /// Interface that represents application service configuration.
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Configure services in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>A service provider.</returns>
        IServiceProvider ConfigureServices(IServiceCollection serviceCollection);

        /// <summary>
        /// Navigate to the start page.
        /// </summary>
        /// <typeparam name="T">The view model.</typeparam>
        /// <returns>The start page.</returns>
        Page NavigateToStart<T>()
            where T : IViewModel;
    }
}