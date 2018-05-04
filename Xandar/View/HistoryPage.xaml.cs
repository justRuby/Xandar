using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xandar.Data;
using Xandar.Service;

namespace Xandar.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryPage : ContentPage
	{
        private const int start = 15;

        private int offset = 0;

        private bool isPaginationWork;
        private bool firstLoadHistory = true;

        private ObservableCollection<History> HistoryList;

		public HistoryPage()
		{
			InitializeComponent ();

            TapGestureRecognizer tapSettings = new TapGestureRecognizer();
            tapSettings.Tapped += TapSettings_Tapped;
            Settings.GestureRecognizers.Add(tapSettings);
        }

        private void TapSettings_Tapped(object sender, EventArgs e)
        {
            HistorySettingsTableView.IsVisible = true;
        }

        private async void DeleteAllHistory_Clicked(object sender, EventArgs e)
        {
            await App.Database.DeleteAllHistoryAsync();
            RefreshingList();

            if (HistorySettingsTableView.IsVisible == true)
                HistorySettingsTableView.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            RefreshingList();
            ListViewHistory.ItemAppearing += ListViewHistory_ItemAppearing;

            base.OnAppearing();
        }

        private async void ListViewHistory_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if(HistorySettingsTableView.IsVisible == true)
                HistorySettingsTableView.IsVisible = false;

            var input = await DisplayActionSheet("", "Отмена", "", "Открыть", "Удалить");

            if(input.Equals("Открыть"))
            {
                Transfer.IsTransfer = true;
                Transfer.Value1 = (e.Item as History).URL;
                await this.Navigation.PopModalAsync();
            }
            else if(input.Equals("Удалить"))
            {
                var item = (History)e.Item;
                HistoryList.Remove(item);
                await App.Database.DeleteHistoryAsync(item);
            }
        }

        private void ListViewHistory_Refreshing(object sender, EventArgs e)
        {
            if (HistorySettingsTableView.IsVisible == true)
                HistorySettingsTableView.IsVisible = false;

            RefreshingList();
            ListViewHistory.EndRefresh();
        }

        private async void ListViewHistory_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                if(isPaginationWork)
                {
                    int presentItemId = HistoryList.IndexOf((History)e.Item);
                    int lastItem = HistoryList.Count - 1;

                    if(presentItemId == lastItem)
                    {
                        var content = await App.Database.GetHistoryAsyncSO(firstLoadHistory, start, offset);

                        if (content.Count == 0)
                        {  
                            isPaginationWork = false;
                            return;
                        }

                        foreach (var item in content)
                        {
                            HistoryList.Add(item);
                        }

                        if (!firstLoadHistory)
                        {
                            offset += 10;
                        }

                        ListViewHistory.ItemsSource = HistoryList;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private async void RefreshingList()
        {
            if (HistoryList != null)
            {
                HistoryList.Clear();
            }
            else
            {
                HistoryList = new ObservableCollection<History>();
            }

            firstLoadHistory = true;
            offset = 0;

            var content = await App.Database.GetHistoryAsyncSO(firstLoadHistory, start, offset);

            if (content.Count == 0)
            {
                DependencyService.Get<IToast>().ShowLong("История серфинга пуста!");
                return;
            }

            foreach (var item in content)
            {
                HistoryList.Add(item);
            }

            firstLoadHistory = false;
            isPaginationWork = true;

            ListViewHistory.ItemsSource = HistoryList;
            offset = 10;
        }

    }
}