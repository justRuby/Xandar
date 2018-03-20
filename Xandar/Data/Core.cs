using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xandar.Data
{
    class Core
    {
        private static Core instance;

        private Core() { }

        public static Core GetInstance()
        {
            if (instance == null)
                instance = new Core();
            return instance;
        }


        private static Task<string> SendQuery(string query)
        {
            string result = "";

            return Task.FromResult(result);
        }

    }
}
