using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class PlanOutOfIngredientsService : IPlanOutOfIngredients
    {
        public List<PlanOutOfIngredientsModel> GetPlanOutOfIngredients(DateTime now)
        {
            List<PlanOutOfIngredientsModel> plans = new List<PlanOutOfIngredientsModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT PlanOutOfIngredients.shop_id,
		                                                Shop.shop_name,
		                                                PlanOutOfIngredients.ingredients_id,
		                                                IngredientsMenu.ingredients_name,
		                                                date
	                                                  FROM PlanOutOfIngredients
                                                LEFT JOIN Shop ON Shop.shop_id = PlanOutOfIngredients.shop_id
                                                LEFT JOIN IngredientsMenu ON IngredientsMenu.ingredients_id = PlanOutOfIngredients.ingredients_id
                                                WHERE date = '{now.ToString("yyyy-MM-dd")}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        PlanOutOfIngredientsModel plan = new PlanOutOfIngredientsModel()
                        {
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            ingredients_id = dr["ingredients_id"].ToString(),
                            ingredients_name = dr["ingredients_name"].ToString(),
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
