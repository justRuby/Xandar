using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace Xandar.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private const string START_PAGE = "http://www.google.com";

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            MyItemsSource = new ObservableCollection<WebView>()
            {
                new WebView()
                {
                    HeightRequest = 1000,
                    WidthRequest = 1000,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Source = START_PAGE
                },

            };

            MyCommand = new Command(() =>
            {
                Debug.WriteLine("Position selected.");
            });
        }

        private ObservableCollection<WebView> _myItemsSource;
        public ObservableCollection<WebView> MyItemsSource
        {
            set
            {
                _myItemsSource = value;
                OnPropertyChanged("MyItemsSource");
            }
            get
            {
                return _myItemsSource;
            }
        }

        public Command MyCommand { protected set; get; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
