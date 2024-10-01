using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class TransactionService : ITransaction
    {
        public List<TransactionModel> GetTransactionByDate(DateTime date)
        {
            List<TransactionModel> transactions = new List<TransactionModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT [Transaction].id,
		                                                [Transaction].employee_id,
		                                                E1.employee_name,
		                                                [Transaction].receiver_id,
		                                                E2.employee_name as receiver_name,
		                                                type,
		                                                amount,
		                                                date,
		                                                note
		                                        FROM [Transaction]
                                                LEFT JOIN Employee E1 ON E1.employee_id = [Transaction].employee_id
                                                LEFT JOIN Employee E2 ON E2.employee_id = [Transaction].receiver_id
                                                WHERE date = '{date.ToString("yyyy-MM-dd")}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TransactionModel transaction = new TransactionModel()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            receiver_id = dr["receiver_id"].ToString(),
                            receiver_name = dr["receiver_name"].ToString(),
                            type = dr["type"].ToString(),
                            amount = dr["amount"] != DBNull.Value ? Convert.ToDouble(dr["amount"].ToString()) : 0.0,
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"].ToString()) : DateTime.MinValue,
                            note = dr["note"].ToString()
                        };
                        transactions.Add(transaction);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return transactions;
        }

        public List<TransactionModel> GetTransactionByEmployee(string employee_id)
        {
            List<TransactionModel> transactions = new List<TransactionModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT [Transaction].id,
		                                                [Transaction].employee_id,
		                                                E1.employee_name,
		                                                [Transaction].receiver_id,
		                                                E2.employee_name as receiver_name,
		                                                type,
		                                                amount,
		                                                date,
		                                                note
		                                        FROM [Transaction]
                                                LEFT JOIN Employee E1 ON E1.employee_id = [Transaction].employee_id
                                                LEFT JOIN Employee E2 ON E2.employee_id = [Transaction].receiver_id
                                                WHERE [Transaction].employee_id = '{employee_id}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TransactionModel transaction = new TransactionModel()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            receiver_id = dr["receiver_id"].ToString(),
                            receiver_name = dr["receiver_name"].ToString(),
                            type = dr["type"].ToString(),
                            amount = dr["amount"] != DBNull.Value ? Convert.ToDouble(dr["amount"].ToString()) : 0.0,
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"].ToString()) : DateTime.MinValue,
                            note = dr["note"].ToString()
                        };
                        transactions.Add(transaction);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return transactions;
        }

        public List<TransactionModel> GetTransactions()
        {
            List<TransactionModel> transactions = new List<TransactionModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT [Transaction].id,
		                                                [Transaction].employee_id,
		                                                E1.employee_name,
		                                                [Transaction].receiver_id,
		                                                E2.employee_name as receiver_name,
		                                                type,
		                                                amount,
		                                                date,
		                                                note
		                                        FROM [Transaction]
                                                LEFT JOIN Employee E1 ON E1.employee_id = [Transaction].employee_id
                                                LEFT JOIN Employee E2 ON E2.employee_id = [Transaction].receiver_id");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TransactionModel transaction = new TransactionModel()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            receiver_id = dr["receiver_id"].ToString(),
                            receiver_name = dr["receiver_name"].ToString(),
                            type = dr["type"].ToString(),
                            amount = dr["amount"] != DBNull.Value ? Convert.ToDouble(dr["amount"].ToString()) : 0.0,
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"].ToString()) : DateTime.MinValue,
                            note = dr["note"].ToString()
                        };
                        transactions.Add(transaction);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return transactions;
        }

        public string Insert(TransactionModel transaction)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO [Transaction](employee_id,receiver_id,[type],amount,date,note)
                    VALUES (@employee_id,@receiver_id,@type,@amount,@date,@note)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@employee_id", transaction.employee_id);
                    cmd.Parameters.AddWithValue("@receiver_id", transaction.receiver_id);
                    cmd.Parameters.AddWithValue("@type", transaction.type);
                    cmd.Parameters.AddWithValue("@amount", transaction.amount);
                    cmd.Parameters.AddWithValue("@date", transaction.date);
                    cmd.Parameters.AddWithValue("@note", transaction.note);
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
