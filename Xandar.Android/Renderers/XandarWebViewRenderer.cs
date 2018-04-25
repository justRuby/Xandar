using System.ComponentModel;

using Android.Content;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Xandar.CustomControl;
using Xandar.Droid.Renderers;

[assembly: ExportRenderer(typeof(XandarWebView), typeof(XandarWebViewRenderer))]
namespace Xandar.Droid.Renderers
{
    public class XandarWebViewRenderer : WebViewRenderer
    {
        private int _zoom = 0;
        private string _userAgentString = "";
        private bool isDataTaken = false;
        private bool isModeTurn = false;

        public XandarWebViewRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            if (!isDataTaken)
            {
                isDataTaken = true;
                _zoom = Control.Settings.TextZoom;
                _userAgentString = Control.Settings.UserAgentString;
            }

            if (Control != null)
            {
                Control.Settings.BuiltInZoomControls = true;
                Control.Settings.DisplayZoomControls = false;
            }

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var element = Element as XandarWebView;

            if (element.IsMobileVersion)
            {
                Control.Settings.UserAgentString = _userAgentString;
                Control.Settings.TextZoom = _zoom;
            }
            else
            {
                Control.Settings.UserAgentString = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
                Control.Settings.TextZoom = element.ZoomInLevel;
            }

            if (Control.OriginalUrl != null)
            {
                element.OriginalURL = Control.OriginalUrl;
            }

            element.TitlePage = Control.Title;
        }
    }
}