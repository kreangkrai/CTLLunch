using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class MenuService : IMenu
    {
        public string Delete(string menu_id)
        {
            try
            {
                string string_command = string.Format($@"DELETE FROM Menu WHERE menu_id = @menu_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@menu_id", menu_id);
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
            string memu_id = "M000";
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT TOP 1 menu_id FROM Menu ORDER BY menu_id DESC");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        memu_id = dr["memu_id"].ToString();
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return memu_id;
        }

        public List<MenuModel> GetMenuByMenu(string menu_id)
        {
            List<MenuModel> menus = new List<MenuModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT Menu.[menu_id]
                                                  ,Menu.[group_id]
	                                              ,GroupMenu.group_name
                                                  ,Menu.[shop_id]
	                                              ,Shop.shop_name
                                                  ,[menu_name]
                                                  ,[price]
                                                  ,[menu_pic]
                                                  ,[extra_price],
												  Menu.category_id,
												  CategoryMenu.category_name,
												  Menu.ingredients_id,
												  IngredientsMenu.ingredients_name
                                              FROM [Lunch].[dbo].[Menu]
                                              LEFT JOIN GroupMenu ON GroupMenu.group_id = Menu.group_id
                                              LEFT JOIN Shop ON Shop.shop_id = Menu.shop_id
											  LEFT JOIN CategoryMenu ON CategoryMenu.category_id = Menu.category_id
											  LEFT JOIN IngredientsMenu ON IngredientsMenu.ingredients_id = Menu.ingredients_id
                                              WHERE Menu.[menu_id] = '{menu_id}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        MenuModel menu = new MenuModel()
                        {
                            menu_id = dr["menu_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            group_name = dr["group_name"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            menu_pic = dr["menu_pic"] != DBNull.Value ? (byte[])dr["menu_pic"] : null,
                            extra_price = dr["extra_price"] != DBNull.Value ? Convert.ToInt32(dr["extra_price"].ToString()) : 0,
                            category_id = dr["category_id"].ToString(),
                            category_name = dr["category_name"].ToString(),
                            ingredients_id = dr["ingredients_id"].ToString(),
                            ingredients_name = dr["ingredients_name"].ToString()
                        };
                        menus.Add(menu);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return menus;
        }

        public List<MenuModel> GetMenuByShop(string shop_id)
        {
            List<MenuModel> menus = new List<MenuModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT Menu.[menu_id]
                                                  ,Menu.[group_id]
	                                              ,GroupMenu.group_name
                                                  ,Menu.[shop_id]
	                                              ,Shop.shop_name
                                                  ,[menu_name]
                                                  ,[price]
                                                  ,[menu_pic]
                                                  ,[extra_price],
												  Menu.category_id,
												  CategoryMenu.category_name,
												  Menu.ingredients_id,
												  IngredientsMenu.ingredients_name
                                              FROM [Lunch].[dbo].[Menu]
                                              LEFT JOIN GroupMenu ON GroupMenu.group_id = Menu.group_id
                                              LEFT JOIN Shop ON Shop.shop_id = Menu.shop_id
											  LEFT JOIN CategoryMenu ON CategoryMenu.category_id = Menu.category_id
											  LEFT JOIN IngredientsMenu ON IngredientsMenu.ingredients_id = Menu.ingredients_id
                                              WHERE Menu.[shop_id] = '{shop_id}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        MenuModel menu = new MenuModel()
                        {
                            menu_id = dr["menu_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            group_name = dr["group_name"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            menu_pic = dr["menu_pic"] != DBNull.Value ? (byte[])dr["menu_pic"] : null,
                            extra_price = dr["extra_price"] != DBNull.Value ? Convert.ToInt32(dr["extra_price"].ToString()) : 0,
                            category_id = dr["category_id"].ToString(),
                            category_name = dr["category_name"].ToString(),
                            ingredients_id = dr["ingredients_id"].ToString(),
                            ingredients_name = dr["ingredients_name"].ToString()
                        };
                        menus.Add(menu);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return menus;
        }

        public List<MenuModel> GetMenus()
        {
            List<MenuModel> menus = new List<MenuModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT Menu.[menu_id]
                                                  ,Menu.[group_id]
	                                              ,GroupMenu.group_name
                                                  ,Menu.[shop_id]
	                                              ,Shop.shop_name
                                                  ,[menu_name]
                                                  ,[price]
                                                  ,[menu_pic]
                                                  ,[extra_price],
												  Menu.category_id,
												  CategoryMenu.category_name,
												  Menu.ingredients_id,
												  IngredientsMenu.ingredients_name
                                              FROM [Lunch].[dbo].[Menu]
                                              LEFT JOIN GroupMenu ON GroupMenu.group_id = Menu.group_id
                                              LEFT JOIN Shop ON Shop.shop_id = Menu.shop_id
											  LEFT JOIN CategoryMenu ON CategoryMenu.category_id = Menu.category_id
											  LEFT JOIN IngredientsMenu ON IngredientsMenu.ingredients_id = Menu.ingredients_id");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        MenuModel menu = new MenuModel()
                        {
                            menu_id = dr["menu_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            group_name = dr["group_name"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            menu_pic = dr["menu_pic"] != DBNull.Value ? (byte[])dr["menu_pic"] : null,
                            extra_price = dr["extra_price"] != DBNull.Value ? Convert.ToInt32(dr["extra_price"].ToString()) : 0,
                            category_id = dr["category_id"].ToString(),
                            category_name = dr["category_name"].ToString(),
                            ingredients_id = dr["ingredients_id"].ToString(),
                            ingredients_name = dr["ingredients_name"].ToString()
                        };
                        menus.Add(menu);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return menus;
        }

        public string Insert(MenuModel menu)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO Menu (menu_id,
                                         group_id,
                                         shop_id,
                                         ingredients_id,
                                         category_id,
                                         menu_name,
                                         price,
                                         menu_pic,
                                         extra_price)
                                        VALUES( @menu_id,
                                                @group_id,
                                                @shop_id,
                                                @ingredients_id,
                                                @category_id,
                                                @menu_name,
                                                @price,
                                                @menu_pic,
                                                @extra_price)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@menu_id", menu.menu_id);
                    cmd.Parameters.AddWithValue("@group_id", menu.group_id);
                    cmd.Parameters.AddWithValue("@shop_id", menu.shop_id);
                    cmd.Parameters.AddWithValue("@ingredients_id", menu.ingredients_id);
                    cmd.Parameters.AddWithValue("@category_id", menu.category_id);
                    cmd.Parameters.AddWithValue("@menu_name", menu.menu_name);
                    cmd.Parameters.AddWithValue("@price", menu.price);
                    cmd.Parameters.AddWithValue("@menu_pic", menu.menu_pic);
                    cmd.Parameters.AddWithValue("@extra_price", menu.extra_price); 
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

        public List<MenuModel> SearchMenuByShop(string shop_id, string menu)
        {
            List<MenuModel> menus = new List<MenuModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT Menu.[menu_id]
                                                  ,Menu.[group_id]
	                                              ,GroupMenu.group_name
                                                  ,Menu.[shop_id]
	                                              ,Shop.shop_name
                                                  ,[menu_name]
                                                  ,[price]
                                                  ,[menu_pic]
                                                  ,[extra_price],
												  Menu.category_id,
												  CategoryMenu.category_name,
												  Menu.ingredients_id,
												  IngredientsMenu.ingredients_name
                                              FROM [Lunch].[dbo].[Menu]
                                              LEFT JOIN GroupMenu ON GroupMenu.group_id = Menu.group_id
                                              LEFT JOIN Shop ON Shop.shop_id = Menu.shop_id
											  LEFT JOIN CategoryMenu ON CategoryMenu.category_id = Menu.category_id
											  LEFT JOIN IngredientsMenu ON IngredientsMenu.ingredients_id = Menu.ingredients_id
                                              WHERE Menu.shop_id = '{shop_id}' AND menu_name LIKE '%{menu}%'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        MenuModel _menu = new MenuModel()
                        {
                            menu_id = dr["menu_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            group_name = dr["group_name"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            menu_pic = dr["menu_pic"] != DBNull.Value ? (byte[])dr["menu_pic"] : null,
                            extra_price = dr["extra_price"] != DBNull.Value ? Convert.ToInt32(dr["extra_price"].ToString()) : 0,
                            category_id = dr["category_id"].ToString(),
                            category_name = dr["category_name"].ToString(),
                            ingredients_id = dr["ingredients_id"].ToString(),
                            ingredients_name = dr["ingredients_name"].ToString()
                        };
                        menus.Add(_menu);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return menus;
        }

    }
}
