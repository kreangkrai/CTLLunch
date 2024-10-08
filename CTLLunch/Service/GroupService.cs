using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class GroupService : IGroup
    {
        public string Delete(string group_id)
        {
            try
            {
                string string_command = string.Format($@"DELETE FROM GroupMenu WHERE group_id = @group_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@group_id", group_id);
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

        public List<GroupMenuModel> GetGroups()
        {
            List<GroupMenuModel> groups = new List<GroupMenuModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT group_id,group_name FROM GroupMenu");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        GroupMenuModel group = new GroupMenuModel()
                        {
                            group_id = dr["group_id"].ToString(),
                            group_name = dr["group_name"].ToString()
                        };
                        groups.Add(group);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return groups;
        }

        public string GetLastID()
        {
            string group = "G00";
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT t.group_id FROM (
                                                SELECT RANK() OVER(ORDER BY group_id DESC) as rank, group_id FROM GroupMenu
                                                ) t
                                                WHERE t.rank = 2");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        group = dr["group_id"].ToString();
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return group;
        }

        public string Insert(GroupMenuModel group)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO GroupMenu(group_id,group_name)
                    VALUES (@group_id,@group_name)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@group_id", group.group_id);
                    cmd.Parameters.AddWithValue("@group_name", group.group_name);
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

        public string Update(GroupMenuModel group)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE GroupMenu SET group_name = @group_name
                                        WHERE group_id = @group_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@group_id", group.group_id);
                    cmd.Parameters.AddWithValue("@group_name", group.group_name);
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
