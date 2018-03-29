using System;
using Xamarin.Forms;

using Xandar.Model;

namespace Xandar
{
    public partial class MainPage : ContentPage
	{
        private WebView Page;

        public MainPage()
		{
			InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Page = new WebView()
            {
                HeightRequest = 600,
                WidthRequest = 400
            };

            Page.Navigating += CurrentWebView_OnNavigating;
            Page.Navigated += CurrentWebView_OnEndNavigating;
        }

        #region CurrentPage Event

        private void CurrentWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            UrlEntry.Text = e.Url;
            IndicatorOfLoadingPageProgressBar.IsVisible = true;
        }

        private void CurrentWebView_OnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            IndicatorOfLoadingPageProgressBar.IsVisible = false;
        }

        #endregion 

        #region ActionBar

        private void GoToBeforePage_Clicked(object sender, EventArgs e)
        {
            if (Page.CanGoBack)
            {
                Page.GoBack();
            }
            else
            {
                MainPageGrid.IsVisible = true;
                WebClientStackLayout.IsVisible = false;
            }
        }

        private void GoToNextPage_Clicked(object sender, EventArgs e)
        {
            if (Page.CanGoForward)
            {
                Page.GoForward();
            }
        }

        private void GoSearchOrUrlButton_Clicked(object sender, EventArgs e)
        {
            if(SearchOrUrlEntry.Text != string.Empty)
            {
                UrlEntry.Text = SearchOrUrlEntry.Text;
                SearchOrUrlEntry.Text = string.Empty;

                Page.Source = UrlEntry.Text = SetWebViewSource(UrlEntry.Text);

                MainPageGrid.IsVisible = false;
                WebClientStackLayout.IsVisible = true;
                
                CurrentPage.Children.Add(Page);
            }
        }

        #endregion

        #region Entry and Update

        private void UrlEntry_Unfocused(object sender, FocusEventArgs e)
        {
            Page.Source = UrlEntry.Text = SetWebViewSource((sender as Entry).Text);
        }

        private void UpdatePageButton_Clicked(object sender, EventArgs e)
        {
            Page.Source = UrlEntry.Text;
        }

        #endregion

        private string SetWebViewSource(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }
            else
            {
                return "https://www.google.com/search?q=" + url;
            }
        }

    }
}
