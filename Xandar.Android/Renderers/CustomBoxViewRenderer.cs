using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Xandar.CustomControl;
using Xandar.Droid.Renderers;

[assembly: ExportRenderer(handler: typeof(CustomBoxView), target: typeof(CustomBoxViewRenderer))]
namespace Xandar.Droid.Renderers
{
    public class CustomBoxViewRenderer : BoxRenderer
    {
        public CustomBoxViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement as CustomBoxView;

            if (element == null) return;

            if(element.HasShadow)
            {
                ViewGroup.Elevation = 4f;
                ViewGroup.TranslationZ = 20f;
            }
        }
    }
}