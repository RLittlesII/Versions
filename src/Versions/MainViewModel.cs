using System.Reflection;
using Microsoft.Extensions.Options;
using ReactiveMarbles.Mvvm;
using Sextant;
using Versions.Configuration;

namespace Versions
{
    /// <summary>
    /// Represents the View Model for the main view.
    /// </summary>
    public class MainViewModel : RxObject, IViewModel
    {
        private readonly IOptions<FormsSettings> _appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        public MainViewModel(IOptions<FormsSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        /// <inheritdoc />
        public string Id { get; } = nameof(MainViewModel);
    }
}