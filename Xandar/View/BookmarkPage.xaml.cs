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
	public partial class BookmarkPage : ContentPage
	{
        private ObservableCollection<Bookmarks> BookmarkList;

        public BookmarkPage ()
		{
			InitializeComponent ();
		}

        protected override async void OnAppearing()
        {
            BookmarkList = new ObservableCollection<Bookmarks>();

            var content = await App.Database.GetBookmarksAsync();

            if (content.Count == 0)
            {
                DependencyService.Get<IToast>().ShowLong("Закладки отсуствуют!");
                return;
            }

            foreach (var item in content)
            {
                BookmarkList.Add(item);
            }

            ListViewBookMarks.ItemsSource = BookmarkList;
            base.OnAppearing();
        }

        private async void ListViewBookMarks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var input = await DisplayActionSheet("", "Отмена", "", "Открыть", "Удалить");

            if (input.Equals("Открыть"))
            {
                Transfer.IsTransfer = true;
                Transfer.Value1 = (e.Item as Bookmarks).URL;
                await this.Navigation.PopModalAsync();
            }
            else if (input.Equals("Удалить"))
            {
                var item = (Bookmarks)e.Item;
                BookmarkList.Remove(item);
                await App.Database.DeleteBookmarkAsync(item);
            }
        }
    }
}