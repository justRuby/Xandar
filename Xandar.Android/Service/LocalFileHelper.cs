using System;
using System.IO;
using Xandar.Droid.Service;
using Xandar.Service;

[assembly: Xamarin.Forms.Dependency(typeof(LocalFileHelper))]
namespace Xandar.Droid.Service
{
    public class LocalFileHelper : ILocalFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //var docFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
            string libFolder = Path.Combine(docFolder, "..", "Xandar", "Database");

            if(!Directory.Exists(libFolder))
                Directory.CreateDirectory(libFolder);

            return Path.Combine(libFolder, filename);
        }
    }
}