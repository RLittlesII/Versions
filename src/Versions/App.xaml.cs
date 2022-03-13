using Versions.Startup;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Versions
{
    public partial class App : Application
    {
        public App(IPlatformInitializer initializer)
        {
            InitializeComponent();

            var _ = new VersionsStartup(initializer);

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}