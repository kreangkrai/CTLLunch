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
                                              FROM [Lunch].[dbo].[Menu]
                                              LEFT JOIN GroupMenu ON GroupMenu.group_id = Menu.group_id
                                              LEFT JOIN Shop ON Shop.shop_id = Menu.shop_id
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
                            menu_pic = dr["menu_pic"] != DBNull.Value ? (byte[])dr["menu_pic"] : null
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
                                              FROM [Lunch].[dbo].[Menu]
                                              LEFT JOIN GroupMenu ON GroupMenu.group_id = Menu.group_id
                                              LEFT JOIN Shop ON Shop.shop_id = Menu.shop_id");
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
                            menu_pic = dr["menu_pic"] != DBNull.Value ? (byte[])dr["menu_pic"] : null
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
    }
}
