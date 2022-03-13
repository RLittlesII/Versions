using Microsoft.Extensions.Configuration;
using Versions.Startup;

namespace Versions.iOS
{
    public class iOSInitializer : IPlatformInitializer
    {
        public iOSInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IServiceCollection Initialize(IServiceCollection serviceCollection) =>
            serviceCollection
                .ConfigureAppSettings(_configuration);

        private readonly IConfiguration _configuration;
    }
}