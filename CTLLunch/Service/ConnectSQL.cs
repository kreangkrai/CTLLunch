using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class ConnectSQL
    {
        public static SqlConnection con;
        public static SqlConnection con_ad;
        public static SqlConnection OpenConnect()
        {
            con = new SqlConnection("Data Source = 192.168.15.202, 1433; Initial Catalog = Lunch; User Id = sa; Password = p@ssw0rd; Timeout = 120;TrustServerCertificate=True");
            
            con.Open();
            
            return con;
        }
        public static SqlConnection CloseConnect()
        {
            con.Close();
            return con;
        }

        public static SqlConnection OpenADConnect()
        {
            con_ad = new SqlConnection("Data Source = 192.168.15.202, 1433; Initial Catalog = gps_sale_tracking; User Id = sa; Password = p@ssw0rd; Timeout = 120");

            con_ad.Open();

            return con_ad;
        }
        public static SqlConnection CloseADConnect()
        {
            con_ad.Close();
            return con_ad;
        }
    }
}
