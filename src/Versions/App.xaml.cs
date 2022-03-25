using Versions.Startup;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Versions
{
    /// <inheritdoc />
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <param name="initializer">The platform initializer.</param>
        public App(IPlatformInitializer initializer)
        {
            InitializeComponent();

            var startup = new VersionsStartup(initializer);

            MainPage = startup.NavigateToStart<MainViewModel>();
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        /// <inheritdoc />
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <inheritdoc />
        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}