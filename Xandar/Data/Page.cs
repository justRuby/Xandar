using SQLite;

namespace Xandar.Data
{
    public class Page
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public string OriginalURL
        {
            get;
            set;
        }
    }
}

