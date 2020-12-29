using AdbKeys.Dependencies;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(AdbKeys.Droid.Dependencies.SnackBar))]
namespace AdbKeys.Droid.Dependencies
{
    public class SnackBar : ISnackBar
    {
        public override void ShowSnackbar(string msg)
        {
            Toast.MakeText(Application.Context, msg, ToastLength.Long);
        }
    }
}