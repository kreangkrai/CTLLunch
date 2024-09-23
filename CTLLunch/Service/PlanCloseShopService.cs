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
        public List<PlanCloseShopModel> GetPlanCloseShops(DateTime now)
        {
            List<PlanCloseShopModel> plans = new List<PlanCloseShopModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT PlanCloseShop.shop_id,
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
    }
}
