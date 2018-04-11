using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace Xandar.CustomControl
{
    public class XandarWebView : WebView
    {
        public string OriginalURL { get; set; }
        public string TitlePage { get; set; }

        public int ZoomInLevel
        {
            get { return (int)GetValue(ZoomInLevelProperty); }
            set { SetValue(ZoomInLevelProperty, value); }
        }

        public static readonly BindableProperty ZoomInLevelProperty =
            BindableProperty.Create(
                propertyName: "ZoomInLevel",
                returnType: typeof(int),
                declaringType: typeof(XandarWebView),
                defaultValue: 36,
                propertyChanged: OnZoomInLevelPropertyChanged);

        public bool IsMobileVersion
        {
            get { return (bool)GetValue(IsMobileVersionProperty); }
            set { SetValue(IsMobileVersionProperty, value); }
        }

        public static readonly BindableProperty IsMobileVersionProperty =
            BindableProperty.Create(
                propertyName: "IsMobileVersion",
                returnType: typeof(bool),
                declaringType: typeof(XandarWebView),
                defaultValue: true,
                propertyChanged: OnIsMobileVersionPropertyChanged);

        private static void OnZoomInLevelPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }
        private static void OnIsMobileVersionPropertyChanged(BindableObject bindable, object oldValue, object newValue) { }

    }
}
