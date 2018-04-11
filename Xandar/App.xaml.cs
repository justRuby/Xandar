using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xandar.Data;
using Xandar.Service;

namespace Xandar
{
	public partial class App : Application
	{
        private static XandarDatabase _database;

		public App ()
		{
			InitializeComponent();

            MainPage = new Xandar.MainPage();
		}

        public static XandarDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new XandarDatabase(DependencyService.Get<ILocalFileHelper>().GetLocalFilePath("Xandar.db3"));
                }

                return _database;
            }
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

	}
}
