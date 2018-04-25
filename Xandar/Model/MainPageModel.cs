using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xandar.Model
{
    public class MainPageModel : INotifyPropertyChanged
    {
        private string _privacyImagePath;
        private string _fullscreenImagePath;

        public string PrivacyImagePath
        {
            get
            {
                return _privacyImagePath;
            }
            set
            {
                _privacyImagePath = value;
                OnPropertyChanged("PrivacyImagePath");
            }
        }

        public string FullscreenImagePath
        {
            get
            {
                return _fullscreenImagePath;
            }
            set
            {
                _fullscreenImagePath = value;
                OnPropertyChanged("FullscreenImagePath");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
