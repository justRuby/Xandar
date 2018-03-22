using System;
using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Serialization;

namespace Xandar.CustomView
{
    public class XandarWebView : HybridWebView
    {
        private readonly IJsonSerializer jsonSerializer;
        private readonly Dictionary<string, Action<string>> registeredActions;
        private readonly Dictionary<string, Func<string, object[]>> registeredFunctions;

        public XandarWebView(IJsonSerializer jsonSerializer) : base(jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
            this.registeredActions = new Dictionary<string, Action<string>>();
            this.registeredFunctions = new Dictionary<string, Func<string, object[]>>();
        }

        public event EventHandler<ClickEventArgs> Clicked;

        public void Initialize()
        {
            LoadFinished += (sender, e) =>
                InjectJavaScript(@"
                document.body.addEventListener('click', function(e) {
                    e = e || window.event;
                    var target = e.target || e.srcElement;
                    Native('invokeClick', 'tag='+target.tagName+' id='+target.id+' name='+target.name);
                }, true /* to ensure we capture it first*/);
            ");

            this.RegisterCallback("invokeClick", (string el) => {
                var args = new ClickEventArgs { Element = el };

                Clicked?.Invoke(this, args);
            });
        }

    }
}
