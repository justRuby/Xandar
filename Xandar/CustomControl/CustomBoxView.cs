using Xamarin.Forms;

namespace Xandar.CustomControl
{
    public class CustomBoxView : BoxView
    {
        public static readonly BindableProperty HasShadowProperty =
            BindableProperty.Create("HasShadow", typeof(bool), typeof(CustomBoxView), false);

        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

    }
}
