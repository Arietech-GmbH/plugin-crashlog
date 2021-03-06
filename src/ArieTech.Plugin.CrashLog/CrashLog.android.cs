// ------------------------------------------------------------------------
//   Copyright (c) ArieTech GmbH.  All rights reserved.
// ------------------------------------------------------------------------

using Android.App;
using Android.Content;
using Android.Runtime;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArieTech.Plugin.CrashLog
{
 
    /// <summary>
    /// Interface for CrashLog
    /// </summary>
    public class CrashLogImplementation : ICrashLog
    {
        Context context;

        public string Filename { get; set; } = "Fatal";

        public void Init(object context = null)
        {
            this.context = context as Context;

            // global exception handling
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentOnUnhandledExceptionRaiser;

            DisplayCrashReport();
        }

        void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception(nameof(TaskSchedulerOnUnobservedTaskException), unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        void AndroidEnvironmentOnUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs raiseThrowableEventArgs)
        {
            var newExc = new Exception(nameof(AndroidEnvironmentOnUnhandledExceptionRaiser), raiseThrowableEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var files = Directory.GetFiles(libraryPath, $"{CrossCrashLog.Current.Filename}?.log").ToList();
                string errorFileName = $"{CrossCrashLog.Current.Filename}{files.Count + 1}.log";

                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = string.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}\r\nCallStack:\r\n{2}",
                DateTime.Now, exception.ToString(), exception.StackTrace);
                File.WriteAllText(errorFilePath, errorMessage);

                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        [Conditional("DEBUG")]
        private void DisplayCrashReport()
        {

            if (context != null)
            {
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var files = Directory.EnumerateFiles(libraryPath, $"{CrossCrashLog.Current.Filename}?.log").ToList();

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
                new AlertDialog.Builder(context)
                    .SetPositiveButton("Clear", (sender, args) =>
                    {
                        File.Delete(errorFilePath);
                    })
                    .SetNegativeButton("Close", (sender, args) =>
                    {
                        // User pressed Close.
                    })
                    .SetMessage(errorText)
                    .SetTitle("Crash Report")
                    .Show();
            }
        }
    }
}
