using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class PlanCloseShopService : IPlanCloseShop
    {
        public string Delete(string id)
        {
            try
            {
                string string_command = string.Format($@"
                    DELETE FROM PlanCloseShop WHERE id = @id");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
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

        public List<PlanCloseShopModel> GetPlanCloseShops(DateTime now)
        {
            List<PlanCloseShopModel> plans = new List<PlanCloseShopModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT PlanCloseShop.id,
                                                        PlanCloseShop.shop_id,
	                                                    Shop.shop_name,
		                                                date
                                                FROM PlanCloseShop
                                                LEFT JOIN Shop ON Shop.shop_id = PlanCloseShop.shop_id
                                                WHERE date = '{now.ToString("yyyy-MM-dd")}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        PlanCloseShopModel plan = new PlanCloseShopModel()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            date = dr["date"] != DBNull.Value ? Convert.ToDateTime(dr["date"].ToString()) : DateTime.MinValue
                        };
                        plans.Add(plan);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return plans;
        }

        public string Insert(PlanCloseShopModel plan)
        {
            try
            {
                string string_command = string.Format($@"
                    INSERT INTO PlanCloseShop(shop_id,date)
                    VALUES (@shop_id,@date)");
                using (SqlCommand cmd = new SqlCommand(string_command, ConnectSQL.OpenConnect()))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@shop_id", plan.shop_id);
                    cmd.Parameters.AddWithValue("@date", plan.date);
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
