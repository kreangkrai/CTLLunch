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
        public string Delete(string shop_id)
        {
            try
            {
                string string_command = string.Format($@"DELETE FROM Shop WHERE shop_id = @shop_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@shop_id", shop_id);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return "Success";
        }

        public string GetLastID()
        {
            string shop_id = "S000";
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT TOP 1 shop_id FROM Shop ORDER BY shop_id DESC");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        shop_id = dr["shop_id"].ToString();
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return shop_id;
        }

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

        public string Insert(ShopModel shop)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO Shop (   shop_id,
                                         shop_name,
                                         phone,
                                         bank_account,
                                         qr_code,
                                         open_time,
                                         close_time,
                                         limit_menu,
                                         limit_order,
                                         delivery_service)
                                        VALUES( @shop_id,
                                                @shop_name,
                                                @phone,
                                                @bank_account,
                                                @qr_code,
                                                @open_time,
                                                @close_time,
                                                @limit_menu,
                                                @limit_order,
                                                @delivery_service)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@shop_id", shop.shop_id);
                    cmd.Parameters.AddWithValue("@shop_name", shop.shop_name);
                    cmd.Parameters.AddWithValue("@phone", shop.phone);
                    cmd.Parameters.AddWithValue("@bank_account", shop.bank_account);
                    cmd.Parameters.AddWithValue("@qr_code", shop.qr_code);
                    cmd.Parameters.AddWithValue("@open_time", shop.open_time);
                    cmd.Parameters.AddWithValue("@close_time", shop.close_time);
                    cmd.Parameters.AddWithValue("@limit_menu", shop.limit_memu);
                    cmd.Parameters.AddWithValue("@limit_order", shop.limit_order);
                    cmd.Parameters.AddWithValue("@delivery_service", shop.delivery_service);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (ConnectSQL.con.State == System.Data.ConnectionState.Open)
                {
                    ConnectSQL.CloseConnect();
                }
            }
            return "Success";
        }
    }
}
