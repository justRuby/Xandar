using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Xandar.Model
{
    public class XWebView : WebView
    {
        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


    }
}
