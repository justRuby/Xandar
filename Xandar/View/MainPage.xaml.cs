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

namespace Xandar
{
    public partial class MainPage : ContentPage
	{
        private const string START_PAGE = "http://www.google.com";
        public ObservableCollection<View> PageCollection;
        private bool isListOfLoadedPagesOpen;
        private int indexOfCurrentPage = 0;

        //Events, Gesture's
        TapGestureRecognizer pageTap = new TapGestureRecognizer();

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
                new XWebView()
                {
                    Source = START_PAGE,
                    Index = 0,
                    Scale = 1,
                    InputTransparent = false,
                    WidthRequest = 400,
                    HeightRequest = 600
                }
            };

            PageCollection[indexOfCurrentPage].Unfocused += MainPage_Unfocused;
            PageCollection[indexOfCurrentPage].Focused += MainPage_Focused;

            ListViewWebPages.SetBinding(ListView.ItemsSourceProperty, "PageCollection");
            ListViewWebPages.ItemsSource = PageCollection;

            UrlEntry.Text = START_PAGE;
        }



        private void InitializeEvents()
        {
            pageTap.NumberOfTapsRequired = 2;
            pageTap.Tapped += PageTap_Tapped;
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
            var page = new XWebView()
            {
                Source = START_PAGE,
                Index = 0,
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
