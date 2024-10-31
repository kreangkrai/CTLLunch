using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
namespace CTLLunch.Service
{
    public class TopupService : ITopup
    {
        public List<TopupModel> GetTopupByEmployee(string employee_id)
        {
            List<TopupModel> employees = new List<TopupModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT Topup.topup_id,
                                                    Topup.employee_id,
                                                    EMP1.employee_name,
                                                    Topup.date,
                                                    topup.receiver_id,
                                                    EMP2.employee_name as receiver_name,
                                                    amount,
                                                    Topup.status,
                                                    note FROM Topup 
                                                    LEFT JOIN Employee EMP1 ON EMP1.employee_id = Topup.employee_id
                                                    LEFT JOIN Employee EMP2 ON EMP2.employee_id = Topup.receiver_id
                                                    WHERE Topup.employee_id = '{employee_id}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TopupModel employee = new TopupModel()
                        {
                            topup_id = dr["topup_id"].ToString(),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            receiver_id = dr["receiver_id"].ToString(),
                            receiver_name = dr["receiver_name"].ToString(),
                            date = Convert.ToDateTime(dr["date"].ToString()),
                            amount = dr["amount"] != DBNull.Value ? Convert.ToInt32(dr["amount"].ToString()) : 0,
                            status = dr["status"].ToString(),
                            note = dr["note"].ToString()
                        };
                        employees.Add(employee);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return employees;
        }

        public List<TopupModel> GetTopups()
        {
            List<TopupModel> employees = new List<TopupModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT Topup.topup_id,
                                                    Topup.employee_id,
                                                    EMP1.employee_name,
                                                    Topup.date,
                                                    topup.receiver_id,
                                                    EMP2.employee_name as receiver_name,
                                                    amount,
                                                    Topup.status,
                                                    note FROM Topup 
                                                    LEFT JOIN Employee EMP1 ON EMP1.employee_id = Topup.employee_id
                                                    LEFT JOIN Employee EMP2 ON EMP2.employee_id = Topup.receiver_id");
                SqlCommand command = new SqlCommand(strCmd, connection);
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TopupModel employee = new TopupModel()
                        {
                            topup_id = dr["topup_id"].ToString(),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            receiver_id = dr["receiver_id"].ToString(),
                            receiver_name = dr["receiver_name"].ToString(),
                            date = Convert.ToDateTime(dr["date"].ToString()),
                            amount = dr["amount"] != DBNull.Value ? Convert.ToInt32(dr["amount"].ToString()) : 0,
                            status = dr["status"].ToString(),
                            note = dr["note"].ToString()
                        };
                        employees.Add(employee);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return employees;
        }

        public string Insert(TopupModel model)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO Topup (topup_id,
                                         date,
                                         employee_id,
                                         receiver_id,
                                        amount,
                                        status,
                                        note)
                                        VALUES(@topup_id,
                                                @date,
                                                @employee_id,
                                                @receiver_id,
                                                @amount,
                                                @status,
                                                @note)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@topup_id", model.topup_id);
                    cmd.Parameters.AddWithValue("@date", model.date);
                    cmd.Parameters.AddWithValue("@employee_id", model.employee_id);
                    cmd.Parameters.AddWithValue("@receiver_id", model.receiver_id);
                    cmd.Parameters.AddWithValue("@amount", model.amount);
                    cmd.Parameters.AddWithValue("@status", model.status);
                    cmd.Parameters.AddWithValue("@note", model.note);
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

        public string UpdateStatus(TopupModel model)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Topup SET status = @status,
                                      note = @note,
                                      receiver_id = @receiver_id,
                                      date = @date
                    WHERE topup_id = @topup_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@topup_id", model.topup_id);
                    cmd.Parameters.AddWithValue("@status", model.status);
                    cmd.Parameters.AddWithValue("@note", model.note);
                    cmd.Parameters.AddWithValue("@receiver_id", model.receiver_id);
                    cmd.Parameters.AddWithValue("@date", model.date);
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
