using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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
            catch(Exception ex)
            {
                return ex.Message;
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
                                                  ,[close_time_shift]
                                                  ,[limit_order]
                                                  ,[limit_menu]
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
                            qr_code = dr["qr_code"] != DBNull.Value ? (byte[])dr["qr_code"] : null,
                            open_time = dr["open_time"] != DBNull.Value ? Convert.ToDateTime(dr["open_time"].ToString()).TimeOfDay : TimeSpan.Zero,
                            close_time = dr["close_time"] != DBNull.Value ? Convert.ToDateTime(dr["close_time"].ToString()).TimeOfDay : TimeSpan.Zero,
                            close_time_shift = dr["close_time_shift"] != DBNull.Value ? Convert.ToDateTime(dr["close_time_shift"].ToString()).TimeOfDay : TimeSpan.Zero,
                            limit_order = dr["limit_order"] != DBNull.Value ? Convert.ToInt32(dr["limit_order"].ToString()) : 100,
                            limit_menu = dr["limit_menu"] != DBNull.Value ? Convert.ToInt32(dr["limit_menu"].ToString()) : 100,
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
                                         open_time,
                                         close_time,
                                         close_time_shift)
                                        VALUES( @shop_id,
                                                @shop_name,
                                                @open_time,
                                                @close_time,
                                                @close_time_shift)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@shop_id", shop.shop_id);
                    cmd.Parameters.AddWithValue("@shop_name", shop.shop_name);
                    cmd.Parameters.AddWithValue("@open_time", shop.open_time);
                    cmd.Parameters.AddWithValue("@close_time", shop.close_time);
                    cmd.Parameters.AddWithValue("@close_time_shift", shop.close_time_shift);
                    if (ConnectSQL.con.State != System.Data.ConnectionState.Open)
                    {
                        ConnectSQL.CloseConnect();
                        ConnectSQL.OpenConnect();
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
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

        public string Update(ShopModel shop)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Shop SET shop_name = @shop_name,
                                    phone = @phone,
                                    bank_account = @bank_account,
                                    qr_code = @qr_code,
                                    open_time = @open_time,
                                    close_time = @close_time,
                                    close_time_shift = @close_time_shift,
                                    limit_menu = @limit_menu,
                                    limit_order = @limit_order,
                                    delivery_service = @delivery_service
                     WHERE shop_id = @shop_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@shop_id", shop.shop_id);
                    cmd.Parameters.AddWithValue("@shop_name", shop.shop_name);
                    cmd.Parameters.AddWithValue("@phone", shop.phone);
                    cmd.Parameters.AddWithValue("@bank_account", shop.bank_account);
                    cmd.Parameters.AddWithValue("@qr_code", ResizeImage(byteArrayToImage(shop.qr_code), new Size(500, 500)));
                    cmd.Parameters.AddWithValue("@open_time", shop.open_time);
                    cmd.Parameters.AddWithValue("@close_time", shop.close_time);
                    cmd.Parameters.AddWithValue("@close_time_shift", shop.close_time_shift);
                    cmd.Parameters.AddWithValue("@limit_menu", shop.limit_menu);
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
            catch (Exception ex)
            {
                return ex.Message;
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

        public byte[] ResizeImage(Image imgToResize, Size size)
        {
            byte[] image;
            // Get the image current width
            int sourceWidth = imgToResize.Width;
            // Get the image current height
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calculate width and height with new desired size
            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);
            nPercent = Math.Min(nPercentW, nPercentH);
            // New Width and Height
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            MemoryStream ms = new MemoryStream();
            b.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            image = ms.ToArray();
            return image;
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        public string UpdateCloseTimeShift(string shop_id)
        {
            try
            {
                string string_command = string.Format($@"
                    Update Shop SET close_time_shift = close_time where shop_id = @shop_id");
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
            catch (Exception ex)
            {
                return ex.Message;
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
