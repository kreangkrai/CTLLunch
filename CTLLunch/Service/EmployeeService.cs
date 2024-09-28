using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class EmployeeService : IEmployee
    {
        public List<EmployeeModel> GetEmployees()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT employee_id,
	                                                employee_name,
	                                                employee_nickname,
	                                                department,
	                                                balance,
	                                                role
                                                 FROM Employee");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EmployeeModel employee = new EmployeeModel()
                        {
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            employee_nickname = dr["employee_nickname"].ToString(),
                            department = dr["department"].ToString(),
                            role = dr["role"].ToString(),
                            balance = dr["balance"] != DBNull.Value ? Convert.ToDouble(dr["balance"].ToString()) : 0.0
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
        public List<UserModel> GetUserAD()
        {
            List<UserModel> users = new List<UserModel>();
            SqlConnection connection = ConnectSQL.OpenADConnect();
            try
            {
                string strCmd = string.Format($@"SELECT DISTINCT Name as name ,Department2 as department FROM Sale_User ORDER BY Name");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserModel user = new UserModel()
                        {
                            name = dr["name"].ToString(),
                            department = dr["department"].ToString()
                        };
                        users.Add(user);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return users;
        }

        public string GetLastEmployee()
        {
            string employee = "EM000";
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT TOP 1 employee_id FROM Employee ORDER BY employee_id DESC");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        employee = dr["employee_id"].ToString();
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return employee;
        }

        public string Insert(EmployeeModel employee)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO Employee(employee_id,employee_name,employee_nickname,department,balance,role)
                    VALUES (@employee_id,@employee_name,@employee_nickname,@department,@balance,@role)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@employee_id", employee.employee_id);
                    cmd.Parameters.AddWithValue("@employee_name", employee.employee_name);
                    cmd.Parameters.AddWithValue("@employee_nickname", employee.employee_nickname);
                    cmd.Parameters.AddWithValue("@department", employee.department);
                    cmd.Parameters.AddWithValue("@balance", employee.balance);
                    cmd.Parameters.AddWithValue("@role", employee.role);
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

        public string UpdateBalance(EmployeeModel employee)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Employee SET balance = @balance
                                        WHERE employee_id = @employee_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@employee_id", employee.employee_id);
                    cmd.Parameters.AddWithValue("@balance", employee.balance);
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

        public string UpdateRole(EmployeeModel employee)
        {
            try
            {
                string string_command = string.Format($@"
                    UPDATE Employee SET employee_nickname = @employee_nickname,
                                        department = @department,
                                        role = @role
                                        WHERE employee_id = @employee_id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@employee_id", employee.employee_id);
                    cmd.Parameters.AddWithValue("@employee_nickname", employee.employee_nickname);
                    cmd.Parameters.AddWithValue("@department", employee.department);   
                    cmd.Parameters.AddWithValue("@role", employee.role);
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
