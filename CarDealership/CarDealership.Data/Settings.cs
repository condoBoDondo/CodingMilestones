using System;
using System.Collections.Generic;
using System.Configuration; // needed to access web.config
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealership.Data
{
    // this exists for parity between different coding approaches, and for ease of use
    public class Settings
    {
        // these get set only once per app session
        private static string _connString;
        private static string _mode;

        public static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connString))
                _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            return _connString;
        }

        public static string GetMode()
        {
            if (string.IsNullOrEmpty(_mode))
                _mode = ConfigurationManager.AppSettings["mode"].ToString();

            return _mode;
        }
    }
}
