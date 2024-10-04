using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class IngredientsService : IIngredients
    {
        public string Delete(string ingredients_id)
        {
            try
            {
                string string_command = string.Format($@"
                    DELETE FROM IngredientsMenu WHERE ingredients_id = @ingredients_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@ingredients_id", ingredients_id);
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

        public List<IngredientsMenuModel> GetIngredients()
        {
            List<IngredientsMenuModel> ingredients = new List<IngredientsMenuModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT ingredients_id,ingredients_name FROM IngredientsMenu");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        IngredientsMenuModel ingredients_ = new IngredientsMenuModel()
                        {
                            ingredients_id = dr["ingredients_id"].ToString(),
                            ingredients_name = dr["ingredients_name"].ToString()
                        };
                        ingredients.Add(ingredients_);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return ingredients;
        }

        public string GetLastID()
        {
            string ingredients_id = "I00";
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT TOP 1 ingredients_id FROM IngredientsMenu ORDER BY ingredients_id DESC");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ingredients_id = dr["ingredients_id"].ToString();
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return ingredients_id;
        }

        public string Insert(IngredientsMenuModel ingredients)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO IngredientsMenu(ingredients_id,ingredients_name)
                    VALUES (@ingredients_id,@ingredients_name)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@ingredients_id", ingredients.ingredients_id);
                    cmd.Parameters.AddWithValue("@ingredients_name", ingredients.ingredients_name);
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

        public string Update(IngredientsMenuModel ingredients)
        {

            try
            {
                string string_command = string.Format($@"
                    UPDATE IngredientsMenu SET ingredients_name = @ingredients_name
                                        WHERE ingredients_id = @ingredients_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@ingredients_id", ingredients.ingredients_id);
                    cmd.Parameters.AddWithValue("@ingredients_name", ingredients.ingredients_name);
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
