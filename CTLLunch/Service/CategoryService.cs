using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class CategoryService : ICategory
    {
        public string Delete(string category_id)
        {
            try
            {
                string string_command = string.Format($@"
                    DELETE FROM CategoryMenu WHERE category_id = @category_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@category_id", category_id);
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

        public List<CategoryMenuModel> GetCategories()
        {
            List<CategoryMenuModel> categories = new List<CategoryMenuModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT category_id,category_name FROM CategoryMenu");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CategoryMenuModel category = new CategoryMenuModel()
                        {
                            category_id = dr["category_id"].ToString(),
                            category_name = dr["category_name"].ToString()
                        };
                        categories.Add(category);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return categories;
        }

        public string GetLastID()
        {
            string category = "C00";
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT t.category_id FROM (
                                                SELECT RANK() OVER(ORDER BY category_id DESC) as rank, category_id FROM CategoryMenu
                                                ) t
                                                WHERE t.rank = 2");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        category = dr["category_id"].ToString();
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return category;
        }

        public string Insert(CategoryMenuModel category)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO CategoryMenu(category_id,category_name)
                    VALUES (@category_id,@category_name)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@category_id", category.category_id);
                    cmd.Parameters.AddWithValue("@category_name", category.category_name);   
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

        public string Update(CategoryMenuModel category)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE CategoryMenu SET category_name = @category_name
                                        WHERE category_id = @category_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@category_id", category.category_id);
                    cmd.Parameters.AddWithValue("@category_name", category.category_name);
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
