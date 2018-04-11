using SQLite;

namespace Xandar.Data
{
    public class Bookmarks
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public string URL
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
