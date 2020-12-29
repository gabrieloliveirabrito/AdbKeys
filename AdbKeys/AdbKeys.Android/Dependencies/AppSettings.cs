using AdbKeys.Dependencies;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(AdbKeys.Droid.Dependencies.AppSettings))]
namespace AdbKeys.Droid.Dependencies
{
    public class AppSettings : IAppSettings
    {
        public override void OpenAppSettings()
        {
            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.AddFlags(ActivityFlags.NewTask);

            var package = AppInfo.PackageName;
            var uri = Android.Net.Uri.FromParts("package", package, null);
            intent.SetData(uri);

            Application.Context.StartActivity(intent);
        }
    }
}