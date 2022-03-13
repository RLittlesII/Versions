using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Splat.Microsoft.Extensions.Logging;
using UIKit;
using Xamarin.Forms.Internals;

namespace Versions.iOS
{
    public class Application
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += HandledUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Splat.LogHost.Default.Error(e.Exception, "TaskScheduler unobserved task exception {@Args}");
        }

        private static void HandledUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;

            if (!e.IsTerminating)
            {
                Splat.LogHost.Default.Error(exception!, "Unhandled Exception {@Args}");

                return;
            }
        }
    }
}