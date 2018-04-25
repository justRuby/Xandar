using SQLite;

namespace Xandar.Data
{
    public class History
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string URL
        {
            get;
            set;
        }

        public string Date
        {
            get;
            set;
        }

        public string Time
        {
            get;
            set;
        }

    }
}
