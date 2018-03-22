using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using Xandar.Model;
using Xandar.Service;
using Xandar.CustomView;
using XLabs.Serialization.JsonNET;

namespace Xandar
{
    public partial class MainPage : ContentPage
	{
        private const string START_PAGE = "http://www.google.com";

        private readonly JsonSerializer _serializer = new JsonSerializer();

        public ObservableCollection<View> PageCollection;
        private bool isListOfLoadedPagesOpen;
        private int indexOfCurrentPage = 0;

        public MainPage()
		{
			InitializeComponent();
            InitializeVariables();
            InitializeEvents();
        }

        private void InitializeVariables()
        {
            PageCollection = new ObservableCollection<View>()
            {
                new XandarWebView(_serializer)
                {
                    Source = START_PAGE,
                    Scale = 1,
                    BackgroundColor = Color.White,
                    InputTransparent = false,
                    WidthRequest = 400,
                    HeightRequest = 600
                }
            };

            (PageCollection[indexOfCurrentPage] as XandarWebView).Initialize();

            (PageCollection[indexOfCurrentPage] as XandarWebView).Clicked += WebView_Clicked;
            //PageCollection[indexOfCurrentPage].Unfocused += MainPage_Unfocused;
            //PageCollection[indexOfCurrentPage].Focused += MainPage_Focused;

            ListViewWebPages.SetBinding(ListView.ItemsSourceProperty, "PageCollection");
            ListViewWebPages.ItemsSource = PageCollection;

            UrlEntry.Text = START_PAGE;
        }

        private void WebView_Clicked(object sender, ClickEventArgs e)
        {
            DisplayAlert("WebView Clicked", e.Element, "Dismiss");
        }

        private void InitializeEvents()
        {
            //CurrentWebView.Navigating += CurrentWebView_OnNavigating;
            //CurrentWebView.Navigated += CurrentWebView_OnEndNavigating;
        }

        #region Tests

        private void MainPage_Focused(object sender, FocusEventArgs e)
        {
            (sender as XWebView).IsVisible = true;
        }

        private void MainPage_Unfocused(object sender, FocusEventArgs e)
        {
            (sender as XWebView).IsVisible = false;
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            int index = (sender as XWebView).Index;
            ListViewWebPages.IsVisible = false;
            //CurrentWebView = (sender as XWebView);
            //CurrentWebView.IsVisible = true;
        }

        private void PageTap_Tapped(object sender, EventArgs e)
        {
            int index = (sender as CarouselView.FormsPlugin.Abstractions.CarouselViewControl).Position;
            ListViewWebPages.IsVisible = false;
            //CurrentWebView = (PageCollection[index] as XWebView);
            //CurrentWebView.IsVisible = true;
        }


        #endregion 

        #region List of Pages Events

        private async void UseCurrentPage_Clicked(object sender, EventArgs e)
        {
            indexOfCurrentPage = ListViewWebPages.Position;
            await PageCollection[indexOfCurrentPage].ScaleTo(1, 250, Easing.CubicOut);
            PageCollection[indexOfCurrentPage].InputTransparent = false;

            ActionBar.IsVisible = true;
            isListOfLoadedPagesOpen = false;
            ActionBarWhenListOfPagesOpen.IsVisible = false;

            ListViewWebPages.IsSwipeEnabled = false;
            ListViewWebPages.ShowArrows = false;
        }

        #endregion

        #region CurrentPage Event

        private void CurrentWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            UrlEntry.Text = e.Source.ToString();
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
            var page = (PageCollection[indexOfCurrentPage] as XWebView);

            if (page.CanGoBack)
            {
                page.GoBack();
            }
        }

        private void GoToNextPage_Clicked(object sender, EventArgs e)
        {
            var page = (PageCollection[indexOfCurrentPage] as XWebView);

            if (page.CanGoForward)
            {
                page.GoForward();
            }
        }

        private void GoSearchOrUrlButton_Clicked(object sender, EventArgs e)
        {

        }

        #endregion

        private async void GetListOfLoadedPagesButton_Clicked(object sender, EventArgs e)
        {
            ActionBar.IsVisible = false;

            await PageCollection[indexOfCurrentPage].ScaleTo(0.5, 250, Easing.CubicIn);
            PageCollection[indexOfCurrentPage].InputTransparent = true;

            ListViewWebPages.IsSwipeEnabled = true;
            ListViewWebPages.ShowArrows = true;
            isListOfLoadedPagesOpen = true;
            ActionBarWhenListOfPagesOpen.IsVisible = true;

            

            //if(isListOfLoadedPagesOpen)
            //{
            //    isListOfLoadedPagesOpen = false;
            //    ActionBarWhenListOfPagesOpen.IsVisible = false;

            //    ListViewWebPages.IsSwipeEnabled = false;
            //    ListViewWebPages.ShowArrows = false;
            //    await PageCollection[indexOfCurrentPage].ScaleTo(1, 250, Easing.CubicOut);
            //}
            //else
            //{

            //}
        }

        #region Манипуляция с вкладками

        private void CreateNewPageButton_Clicked(object sender, EventArgs e)
        {
            CreateNewPage();
        }

        private void Handle_Scrolled(object sender, CarouselView.FormsPlugin.Abstractions.ScrolledEventArgs e)
        {
            if(e.Direction == CarouselView.FormsPlugin.Abstractions.ScrollDirection.Up)
            {
                DependencyService.Get<IToast>().ShowShort("Уиии");
            }

            Debug.WriteLine("Scrolled to " + e.NewValue + " percent.");
            Debug.WriteLine("Direction = " + e.Direction);
        }

        private void CreateNewPage()
        {
            var page = new XandarWebView(_serializer)
            {
                Source = START_PAGE,
                BackgroundColor = Color.White,
                Scale = 0.5,
                InputTransparent = true,
                WidthRequest = 400,
                HeightRequest = 600
            };

            PageCollection.Add(page);
        }

        private void DeletePage(int index)
        {
            PageCollection.RemoveAt(index);
        }

        #endregion

    }
}
