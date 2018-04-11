using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xandar.CustomControl;
using Xandar.Data;
using Xandar.View;


namespace Xandar
{
    public partial class MainPage : ContentPage
	{
        private XandarWebView Page;
        private string currentURL;

        private int _isInitializeLastSessionComplete;
        private bool _isPageOpen;
        private bool _isMainMenuView = true;
        private bool _isPrivacyMode = true;
        private bool _stopLoadingPage;

        public MainPage()
		{
			InitializeComponent();
            Initialize();
            InitializeEvents();
        }

        private void Initialize()
        {
            Page = new XandarWebView()
            {
                HeightRequest = 600,
                WidthRequest = 400
            };

            Page.Navigating += CurrentWebView_OnNavigating;
            Page.Navigated += CurrentWebView_OnEndNavigating;

            BackgroundBlackTransparent.BackgroundColor = Color.FromRgba(0, 0, 0, 100);
        }

        private void InitializeEvents()
        {
            TapGestureRecognizer tapOpenSettings = new TapGestureRecognizer();
            tapOpenSettings.Tapped += OpenSettingsButton_Clicked;
            OpenSettingsButton.GestureRecognizers.Add(tapOpenSettings);

            TapGestureRecognizer tapGoToBeforePage = new TapGestureRecognizer();
            tapGoToBeforePage.Tapped += GoToBeforePage_Clicked;
            GoToBeforePage.GestureRecognizers.Add(tapGoToBeforePage);

            TapGestureRecognizer tapGoToNextPage = new TapGestureRecognizer();
            tapGoToNextPage.Tapped += GoToNextPage_Clicked;
            GoToNextPage.GestureRecognizers.Add(tapGoToNextPage);

            TapGestureRecognizer tapOpenHistoryPage = new TapGestureRecognizer();
            tapOpenHistoryPage.Tapped += OpenHistoryPage_Clicked;
            OpenHistoryPage.GestureRecognizers.Add(tapOpenHistoryPage);

            TapGestureRecognizer tapTurnModePrivacy = new TapGestureRecognizer();
            tapTurnModePrivacy.Tapped += TurnModePrivacy_Clicked;
            TurnModePrivacy.GestureRecognizers.Add(tapTurnModePrivacy);

            TapGestureRecognizer tapTurnModeMobileOrDesktop = new TapGestureRecognizer();
            tapTurnModeMobileOrDesktop.Tapped += TurnModeMobileOrDesktop_Clicked;
            TurnModeMobileOrDesktop.GestureRecognizers.Add(tapTurnModeMobileOrDesktop);

            TapGestureRecognizer tapStopLoadingPage = new TapGestureRecognizer();
            tapStopLoadingPage.Tapped += StopLoadingPage_Clicked;
            StopLoadingPage.GestureRecognizers.Add(tapStopLoadingPage);

            TapGestureRecognizer tapUpdatePage = new TapGestureRecognizer();
            tapUpdatePage.Tapped += UpdatePage_Clicked;
            UpdatePageButton.GestureRecognizers.Add(tapUpdatePage);
        }

        private async void InitializeLastSession()
        {
            var result = await App.Database.GetPagesAsync();

            if (result.Count != 0)
            {
                CreateWebPage(result[0].OriginalURL);
            }


        }

        protected override void OnAppearing()
        {
            if (_isInitializeLastSessionComplete == 0)
            {
                InitializeLastSession();

                _isInitializeLastSessionComplete = 1;
            }

            if (Transfer.IsTransfer)
            {
                Transfer.IsTransfer = false;

                if (!_isPageOpen)
                    CreateWebPage(Transfer.Value1);

                Page.Source = Transfer.Value1;

                if (_isMainMenuView)
                {
                    MainPageGrid.IsVisible = false;
                    WebClientStackLayout.IsVisible = true;
                }
            }

            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            if (!Page.CanGoBack && _isPageOpen)
            {
                CurrentPage.Children.Remove(Page);

                _isPageOpen = false;
                MainPageGrid.IsVisible = true;
                WebClientStackLayout.IsVisible = false;
                return true;
            }

            if (Page.CanGoBack)
            {
                Page.GoBack();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }
        
        #region CurrentPage Event

        private void CurrentWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if(_stopLoadingPage)
            {
                bool cancel = e.Cancel;

                GoToNextPage.IsVisible = false;
                StopLoadingPage.IsVisible = true;
            }
        }

        private async void CurrentWebView_OnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            currentURL = UrlEntry.Text = e.Url;

            GoToNextPage.IsVisible = true;
            StopLoadingPage.IsVisible = false;

            if (_isInitializeLastSessionComplete == 1)
            {
                _isInitializeLastSessionComplete = 2;
                return;
            }

            if (_isPrivacyMode)
            {
                var history = new History()
                {
                    URL = e.Url,
                    Date = DateTime.Now.ToLongTimeString(),
                    Time = DateTime.Now.ToShortDateString()
                };

                await App.Database.SaveHistoryAsync(history);
            }

            await App.Database.SavePagesAsync(new Xandar.Data.Page()
            {
                OriginalURL = e.Url
            });
        }

        #endregion 

        #region ActionBar

        private void GoToBeforePage_Clicked(object sender, EventArgs e)
        {
            if (Page.CanGoBack)
            {
                //Page.Source = Page.OriginalURL;
                Page.GoBack();
            }
            else
            {
                _isMainMenuView = true;
                MainPageGrid.IsVisible = true;
                WebClientStackLayout.IsVisible = false;
            }
        }

        private void GoToNextPage_Clicked(object sender, EventArgs e)
        {
            if (Page.CanGoForward)
            {
                _isMainMenuView = false;
                MainPageGrid.IsVisible = false;
                WebClientStackLayout.IsVisible = true;
                
                //Page.Source = Page.OriginalURL;
                Page.GoForward();
            }
        }

        private void StopLoadingPage_Clicked(object sender, EventArgs e)
        {
            _stopLoadingPage = true;
            GoToNextPage.IsVisible = true;
            (sender as CachedImage).IsVisible = false;
        }

        private void GoSearchOrUrlButton_Clicked(object sender, EventArgs e)
        { 
            if(!_isPageOpen)
              CreateWebPage(SearchOrUrlEntry.Text);

            if(SearchOrUrlEntry.Text != string.Empty)
            {
                SearchOrUrlEntry.Text = string.Empty;

                Page.Source = UrlEntry.Text = SetWebViewSource(UrlEntry.Text);

                MainPageGrid.IsVisible = false;
                WebClientStackLayout.IsVisible = true;
            }
        }

        private void OpenSettingsButton_Clicked(object sender, EventArgs e)
        {
            if(!AppSettings.IsVisible)
            {
                AppSettings.IsVisible = true;
                BackgroundBlackTransparent.IsVisible = true;
            }
            else
            {
                BackgroundBlackTransparent.IsVisible = false;
                AppSettings.IsVisible = false;
            }
        }

        #endregion

        #region SettingsBar

        private async void OpenHistoryPage_Clicked(object sender, EventArgs e)
        {
            BackgroundBlackTransparent.IsVisible = false;
            AppSettings.IsVisible = false;

            var page = new HistoryPage();
            await Navigation.PushModalAsync(page);
        }

        private void TurnModePrivacy_Clicked(object sender, EventArgs e)
        {
            if(_isPrivacyMode)
            {
                _isPrivacyMode = false;
            }
            else
            {
                _isPrivacyMode = true;
            }
        }

        private void TurnModeMobileOrDesktop_Clicked(object sender, EventArgs e)
        {
            bool existMobile = currentURL.Contains(".m.");

            if (existMobile)
            {
                if (Page.IsMobileVersion)
                {
                    Page.IsMobileVersion = false;
                }
                else
                {
                    Page.IsMobileVersion = true;
                }

                Page.Source = Page.OriginalURL.Replace(".m", "");
            }
            else
            {
                if (Page.IsMobileVersion)
                {
                    Page.IsMobileVersion = false;
                }
                else
                {
                    Page.IsMobileVersion = true;
                }

                Page.Source = Page.OriginalURL;
            }

            
        }

        #endregion

        #region Entry and Update

        private void UrlEntry_Unfocused(object sender, FocusEventArgs e)
        {
            Page.Source = UrlEntry.Text = SetWebViewSource((sender as Entry).Text);
        }

        private void UpdatePage_Clicked(object sender, EventArgs e)
        {
            Page.Source = UrlEntry.Text;
        }

        #endregion

        private void CreateWebPage(string source)
        {
            _isPageOpen = true;
            UrlEntry.Text = source;
            SearchOrUrlEntry.Text = string.Empty;

            Page.Source = UrlEntry.Text = SetWebViewSource(UrlEntry.Text);

            MainPageGrid.IsVisible = false;
            WebClientStackLayout.IsVisible = true;

            CurrentPage.Children.Add(Page);
        }

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
