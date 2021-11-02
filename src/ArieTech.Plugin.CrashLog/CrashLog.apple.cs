// ------------------------------------------------------------------------
//   Copyright (c) ArieTech GmbH.  All rights reserved.
// ------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace ArieTech.Plugin.CrashLog
{
    /// <summary>
    /// Interface for CrashLog
    /// </summary>
    public class CrashLogImplementation : ICrashLog
    {
        public string Filename { get; set; } = "Fatal";

        public void Init(object context = null)
        {
            // global exception handling
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            DisplayCrashReport();
        }

        void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        [Conditional("DEBUG")]
        private void DisplayCrashReport()
        {
            //const string errorFilename = "Fatal.log";
            var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
            var files = Directory.EnumerateFiles(libraryPath, $"{Filename}?.log").ToList();

            if (files.Count() == 0)
            {
                return;
            }

            var errorFilePath = Path.Combine(libraryPath, files.Last());

            if (!File.Exists(errorFilePath))
            {
                return;
            }

            var errorText = File.ReadAllText(errorFilePath);
#pragma warning disable CS0618 // Type or member is obsolete
            var alertView = new UIAlertView("Crash Report", errorText, null, "Close", "Clear") { UserInteractionEnabled = true };
#pragma warning restore CS0618 // Type or member is obsolete
            alertView.Clicked += (sender, args) =>
            {
                if (args.ButtonIndex != 0)
                {
                    File.Delete(errorFilePath);
                }
            };
            alertView.Show();
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {

                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                var files = Directory.GetFiles(libraryPath, $"{CrossCrashLog.Current.Filename}?.log").ToList();
                string errorFileName = $"{CrossCrashLog.Current.Filename}{files.Count + 1}.log";

                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = string.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}\r\nCallStack:\r\n{2}",
                DateTime.Now, exception.ToString(), exception.StackTrace);
                File.WriteAllText(errorFilePath, errorMessage);

                //ToDO: Log to iOS Device Logging.
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }
    }
}
