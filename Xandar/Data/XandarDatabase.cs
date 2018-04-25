using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Xandar.Data
{
    public class XandarDatabase
    {
        readonly SQLiteAsyncConnection database;

        public XandarDatabase(string DBpath)
        {
            database = new SQLiteAsyncConnection(DBpath);
            database.CreateTableAsync<History>().Wait();
            database.CreateTableAsync<Bookmarks>().Wait();
            database.CreateTableAsync<Page>().Wait();
        }

        #region Methods for Bookmarks

        public Task<List<Bookmarks>> GetBookmarksAsync()
        {
            return database.Table<Bookmarks>().ToListAsync();
        }

        public Task<Bookmarks> GetBookmarkAsync(int ID)
        {
            return database.Table<Bookmarks>().Where(i => i.ID == ID).FirstOrDefaultAsync();
        }

        public Task<int> SaveBookmarkAsync(Bookmarks bookmarks)
        {
            return database.InsertAsync(bookmarks);
        }

        public async void DeleteAllBookmarksAsync()
        {
            var count = await database.Table<Bookmarks>().CountAsync();

            for (int i = 0; i < count - 1; i++)
            {
                var element = await database.Table<Bookmarks>().ElementAtAsync(i);
                await database.DeleteAsync(element);
            }
        }

        public Task<int> DeleteBookmarkAsync(Bookmarks bookmarks)
        {
            return database.DeleteAsync(bookmarks);
        }

        #endregion

        #region Methods for History

        public Task<List<History>> GetHistoryAsync()
        {
            return database.Table<History>().ToListAsync();
        }

        public Task<List<History>> GetHistoryAsyncSO(bool fisrtGet, int start, int offset)
        {
            var list = database.Table<History>().ToListAsync().Result;
            list.Reverse();

            List<History> result = new List<History>();

            if(fisrtGet)
            {
                for (int i = 0; i < start; i++)
                {
                    if (i < list.Count)
                    {
                        result.Add(list[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int i = offset; i < start + offset; i++)
                {
                    if(i < list.Count)
                    {
                        result.Add(list[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            

            return Task.FromResult(result);
        }

        public Task<int> SaveHistoryAsync(History history)
        {
            return database.InsertAsync(history);
        }

        public async void DeleteAllHistoryAsync()
        {
            var count = await database.Table<History>().CountAsync();

            for (int i = 0; i < count - 1; i++)
            {
                var element = await database.Table<History>().ElementAtAsync(i);
                await database.DeleteAsync(element);
            }
        }

        public Task<int> DeleteHistoryAsync(History history)
        {
            return database.DeleteAsync(history);
        }

        #endregion

        #region Methods for Page

        public Task<List<Page>> GetPagesAsync()
        {
            return database.Table<Page>().ToListAsync();
        }

        public Task<int> SavePagesAsync(Page page)
        {
            var check = database.Table<Page>().ToListAsync().Result;

            if (check.Count == 0)
                return database.InsertAsync(page);

            if (database.FindAsync<Page>(x => x.IDPage == page.IDPage) != null)
            {
                return database.UpdateAsync(page);
            }
            else
            {
                return database.InsertAsync(page);
            }
        }

        #endregion
    }
}
