using System.Diagnostics.CodeAnalysis;
using Foundation;
using Microsoft.Extensions.Configuration;
using Rg.Plugins.Popup.Services;
using Splat;
using UIKit;

namespace Versions.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.

    /// <inheritdoc />
    [Register("AppDelegate")]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart", Justification = "Application Delegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.

        /// <inheritdoc/>
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            PreFormsInit();

            global::Xamarin.Forms.Forms.Init();

            PostFormsInit();
            var initializer =
                new iOSInitializer(new ConfigurationBuilder()
                    .AddJsonFile("ios.appsettings.json")
                    .AddJsonFile("appsettings.dev.json", true)
                    .Build());

            LoadApplication(new App(initializer));

            return base.FinishedLaunching(app, options);
        }

        private static void PreFormsInit()
        {
            Rg.Plugins.Popup.Popup.Init();
        }

        private static void PostFormsInit()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => PopupNavigation.Instance);
        }
    }
}