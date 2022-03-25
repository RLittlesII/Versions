using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Versions.Startup;

[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Main iOS application")]
namespace Versions.iOS
{
    /// <inheritdoc />
    public class iOSInitializer : IPlatformInitializer
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="iOSInitializer"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public iOSInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public IServiceCollection Initialize(IServiceCollection serviceCollection) =>
            serviceCollection
                .AddSerilogConfiguration(new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.NSLog())
                .AddSerilog(() => new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.NSLog().WriteTo.AppCenterCrashes())
                .ConfigureAppSettings(_configuration);
    }
}