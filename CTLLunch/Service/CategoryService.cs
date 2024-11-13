using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace CTLLunch.Service
{
    public class CategoryService : ICategory
    {
        private IConnectAPI API;
        private readonly string URL;
        public CategoryService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
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

        public async Task<string> GetLastID()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"category/getlastid");
            var content = await response.Content.ReadAsStringAsync();
            string last_id = JsonConvert.DeserializeObject<string>(content);
            return last_id;
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
