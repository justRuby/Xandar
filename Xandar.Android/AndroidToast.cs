using Android.Widget;
using Xandar.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidToast))]
namespace Xandar.Droid
{
    public class AndroidToast : Xandar.Service.IToast
    {
        public void ShowLong(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }

        public void ShowShort(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
    }
}