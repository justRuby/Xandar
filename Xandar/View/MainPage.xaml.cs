using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xandar.CustomControl;
using Xandar.Data;
using Xandar.Model;
using Xandar.Service;
using Xandar.View;


namespace Xandar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : CustomContentPage
    {
        private XandarWebView Page;
        private MainPageModel BindingModel;
        private string currentURL;

        private int _isInitializeLastSessionComplete;
        private bool _isPageOpen;
        private bool _isMainMenuView = true;
        private bool _isPrivacyMode;
        private bool _stopLoadingPage;

        public MainPage()
		{
			InitializeComponent();
            Initialize();
            InitializeEvents();
        }

        private void Initialize()
        {
            BindingModel = new MainPageModel()
            {
                FullscreenImagePath = "fullscreen_off.png",
                PrivacyImagePath = "privacy_off.png"
            };

            BindingContext = BindingModel;

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

            OnOrientationChanged += DeviceRotated;

            TapGestureRecognizer tapOpenSettings = new TapGestureRecognizer();
            tapOpenSettings.Tapped += OpenSettingsButton_Clicked;
            OpenSettingsButton.GestureRecognizers.Add(tapOpenSettings);

            TapGestureRecognizer tapGoToBeforePage = new TapGestureRecognizer();
            tapGoToBeforePage.Tapped += GoToBeforePage_Clicked;
            GoToBeforePage.GestureRecognizers.Add(tapGoToBeforePage);

            TapGestureRecognizer tapGoToNextPage = new TapGestureRecognizer();
            tapGoToNextPage.Tapped += GoToNextPage_Clicked;
            GoToNextPage.GestureRecognizers.Add(tapGoToNextPage);

            //TapGestureRecognizer tapOpenHistoryPage = new TapGestureRecognizer();
            //tapOpenHistoryPage.Tapped += OpenHistoryPage_Clicked;
            //OpenHistoryPage.GestureRecognizers.Add(tapOpenHistoryPage);

            //TapGestureRecognizer tapTurnModePrivacy = new TapGestureRecognizer();
            //tapTurnModePrivacy.Tapped += TurnModePrivacy_Clicked;
            //TurnModePrivacy.GestureRecognizers.Add(tapTurnModePrivacy);

            //TapGestureRecognizer tapTurnModeMobileOrDesktop = new TapGestureRecognizer();
            //tapTurnModeMobileOrDesktop.Tapped += TurnModeMobileOrDesktop_Clicked;
            //TurnModeMobileOrDesktop.GestureRecognizers.Add(tapTurnModeMobileOrDesktop);

            TapGestureRecognizer tapStopLoadingPage = new TapGestureRecognizer();
            tapStopLoadingPage.Tapped += StopLoadingPage_Clicked;
            StopLoadingPage.GestureRecognizers.Add(tapStopLoadingPage);

            TapGestureRecognizer tapUpdatePage = new TapGestureRecognizer();
            tapUpdatePage.Tapped += UpdatePage_Clicked;
            UpdatePageButton.GestureRecognizers.Add(tapUpdatePage);

            TapGestureRecognizer tapOpenBookmarks = new TapGestureRecognizer();
            tapOpenBookmarks.Tapped += OpenBookmarks_Clicked;
            OpenBookmarks.GestureRecognizers.Add(tapOpenBookmarks);
        }

        private async void InitializeLastSession()
        {
            var result = await App.Database.GetPagesAsync();

            if (result.Count != 0)
            {
                CreateWebPage(result[0].OriginalURL);
            }
        }

        private void DeviceRotated(object sender, PageOrientationEventArgs e)
        {
            switch (e.Orientation)
            {
                case PageOrientation.Horizontal:

                    AbsoluteLayout.SetLayoutBounds(AppSettings, new Rectangle(0.5, 0.65, 300, AbsoluteLayout.AutoSize));

                    break;

                case PageOrientation.Vertical:

                    AbsoluteLayout.SetLayoutBounds(AppSettings, new Rectangle(0.5, 0.85, 300, AbsoluteLayout.AutoSize));

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

            if (!_isPrivacyMode)
            {
                var history = new History()
                {
                    Title = Page.TitlePage,
                    URL = e.Url,
                    Date = DateTime.Now.ToLongTimeString(),
                    Time = DateTime.Now.ToShortDateString()
                };

                await App.Database.SaveHistoryAsync(history);
            }

            var sessionPage = new Data.Page();

            sessionPage.ID = 1;
            sessionPage.OriginalURL = e.Url;

            await App.Database.SavePagesAsync(sessionPage);
        }

        #endregion

        #region ActionBar

        private void WillInProgress_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IToast>().ShowLong("WIP...");
        }

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

        private async void OpenBookmarks_Clicked(object sender, EventArgs e)
        {
            CloseAllVisibleMenu();
            var page = new BookmarkPage();
            await Navigation.PushModalAsync(page);
        }

        #endregion

        #region SettingsBar

        private async void OpenHistoryPage_Clicked(object sender, EventArgs e)
        {
            CloseAllVisibleMenu();

            var page = new HistoryPage();
            await Navigation.PushModalAsync(page);
        }

        private async void AddFavoritePage_Clicked(object sender, EventArgs e)
        {
            var bookmarks = new Bookmarks()
            {
                Title = Page.TitlePage,
                URL = currentURL
            };

            await App.Database.SaveBookmarkAsync(bookmarks);
        }

        private void TurnModePrivacy_Clicked(object sender, EventArgs e)
        {
            if(_isPrivacyMode)
            {
                _isPrivacyMode = false;
                BindingModel.PrivacyImagePath = "privacy_off.png";
            }
            else
            {
                _isPrivacyMode = true;
                BindingModel.PrivacyImagePath = "privacy_on.png";
            }
        }

        private void TurnModeMobileOrDesktop_Clicked(object sender, EventArgs e)
        {
            if(Page.OriginalURL == null)
            {
                return;
            }

            bool existMobile = currentURL.Contains(".m.");

            if (existMobile)
            {
                if (Page.IsMobileVersion)
                {
                    Page.IsMobileVersion = false;
                    BindingModel.FullscreenImagePath = "fullscreen_on.png";
                }
                else
                {
                    Page.IsMobileVersion = true;
                    BindingModel.FullscreenImagePath = "fullscreen_off.png";
                }

                Page.Source = Page.OriginalURL.Replace(".m", "");
            }
            else
            {
                if (Page.IsMobileVersion)
                {
                    Page.IsMobileVersion = false;
                    BindingModel.FullscreenImagePath = "fullscreen_on.png";
                }
                else
                {
                    Page.IsMobileVersion = true;
                    BindingModel.FullscreenImagePath = "fullscreen_off.png";
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

        private void CloseAllVisibleMenu()
        {
            BackgroundBlackTransparent.IsVisible = false;
            AppSettings.IsVisible = false;
        }
    }
}
