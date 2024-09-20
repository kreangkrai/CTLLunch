using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class ShopService : IShop
    {
        public List<ShopModel> GetShops()
        {
            List<ShopModel> shops = new List<ShopModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT [shop_id]
                                                  ,[shop_name]
                                                  ,[phone]
                                                  ,[bank_account]
                                                  ,[qr_code]
                                                  ,[open_time]
                                                  ,[close_time]
                                                  ,[limit_order]
                                                  ,[delivery_service]
                                              FROM [Lunch].[dbo].[Shop]");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ShopModel shop = new ShopModel()
                        {
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            phone = dr["phone"].ToString(),
                            bank_account = dr["bank_account"].ToString(),
                            qr_code = dr["qr_code"] != DBNull.Value ? (byte[])dr["qr_code"] :null,
                            open_time = dr["open_time"] != DBNull.Value ? Convert.ToDateTime(dr["open_time"].ToString()).TimeOfDay : TimeSpan.Zero,
                            close_time = dr["close_time"] != DBNull.Value ? Convert.ToDateTime(dr["close_time"].ToString()).TimeOfDay : TimeSpan.Zero,
                            limit_order = dr["limit_order"] != DBNull.Value ? Convert.ToInt32(dr["limit_order"].ToString()) : 100,
                            delivery_service = dr["delivery_service"] != DBNull.Value ? Convert.ToInt32(dr["delivery_service"].ToString()) : 0,
                        };
                        shops.Add(shop);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return shops;
        }
    }
}
