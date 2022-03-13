using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace Versions
{
    public interface IStartup
    {
        /// <summary>
        /// Configure services in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>A service provider.</returns>
        IServiceProvider ConfigureServices(IServiceCollection serviceCollection);
        Page NavigateToStart<T>();
    }
}