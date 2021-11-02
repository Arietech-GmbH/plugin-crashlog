// ------------------------------------------------------------------------
//   Copyright (c) ArieTech GmbH.  All rights reserved.
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ArieTech.Plugin.CrashLog.Samples
{
    public class MainViewModel : ObservableObject
    {
        readonly Page view;
        const string CrashReport = "CrashReport";
        object selectedItem;

        public MainViewModel(Page view)
        {
            this.view = view;
            LoadCrashReports();
        }

        public List<DebugItem> CrashReports { get; set; }

        public object SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value, onChanged: () => OnPropertyChanged(nameof(IsDeletable)));
        }

        public bool IsDeletable => SelectedItem != null && (SelectedItem as DebugItem).Description == CrashReport;

        public ICommand DeleteCommand
        {
            get => new Command(() =>
            {
                if (SelectedItem is DebugItem debugItem && File.Exists(debugItem.Path))
                {
                    File.Delete(debugItem.Path);
                    LoadCrashReports();
                }
            });
        }

        public ICommand ThrowExceptionCommand
        {
            get
            {
                return new Command(() => throw new Exception("This exception was thrown by user."));
            }
        }

        public ICommand ViewCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (SelectedItem is DebugItem debugItem && File.Exists(debugItem.Path))
                    {
                        var errorText = File.ReadAllText(debugItem.Path);
                        var yes = await view.DisplayAlert(debugItem.File, errorText, "CLEAR", "CLOSE");
                        if (yes)
                        {
                            if (debugItem.Description == CrashReport)
                            {
                                File.Delete(debugItem.Path);
                                LoadCrashReports();
                            }
                            else
                            {
                                await view.DisplayAlert("Sorry, can't delete that", "Not a crash-report file.", "OK");
                            }
                        }
                    }
                });
            }
        }

        void LoadCrashReports()
        {
            string libraryPath = null;

            if (Device.RuntimePlatform == Device.Android) libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (Device.RuntimePlatform == Device.iOS) libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);

            if (string.IsNullOrEmpty(libraryPath)) throw new PlatformNotSupportedException("Must be iOS or Android");

            var crashReports = new List<DebugItem>();
            Directory.GetFiles(libraryPath).ForEach(f => crashReports.Add(new DebugItem(f)));
            CrashReports = crashReports;
            OnPropertyChanged(nameof(CrashReports));
        }
    }
}
