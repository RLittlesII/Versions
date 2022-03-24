using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UIKit;

[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Main iOS application")]
namespace Versions.iOS
{
    /// <summary>
    /// iOS application.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "This is just iOS")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Main iOS application")]
    [SuppressMessage("Design", "RCS1102:Make class static.", Justification = "Main iOS application")]
    public class Application
    {
        private static void Main(string[] args)
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
            }
        }
    }
}