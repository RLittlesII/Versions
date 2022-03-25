using System;
using Sextant;

namespace Versions.Startup
{
    /// <summary>
    /// Represents a <see cref="IViewModelFactory"/> instance.
    /// </summary>
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ViewModelFactory(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        /// <inheritdoc />
        public TViewModel Create<TViewModel>(string? contract = null)
            where TViewModel : IViewModel =>
            Create<TViewModel>(typeof(TViewModel));

        private TViewModel Create<TViewModel>(Type type) =>
            (TViewModel)_serviceProvider.GetService(type);
    }
}