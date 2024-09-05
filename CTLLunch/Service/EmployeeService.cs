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
    }
}
