using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class ReserveService : IReserve
    {
        public string Delete(string reserve_id)
        {
            throw new NotImplementedException();
        }

        public List<ReserveModel> GetReserveByDate(DateTime date)
        {
            List<ReserveModel> reserves = new List<ReserveModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT reserve_id,
                                                  Reserve.employee_id,
	                                              Employee.employee_name,
	                                              Employee.employee_nickname,
                                                  Reserve.shop_id,
	                                              Shop.shop_name,
	                                              Shop.delivery_service,
                                                  Reserve.menu_id,
	                                              Menu.menu_name,
                                                  Reserve.category_id,
	                                              Reserve.group_id,
	                                              Menu.price,
                                                  amount_order,
                                                  extra,
                                                  note,
                                                  reserve_date,
                                                  remark,
                                                  status,
                                                  review
                                            FROM Reserve
                                            LEFT JOIN Employee ON Employee.employee_id = Reserve.employee_id
                                            LEFT JOIN Shop ON Shop.shop_id = Reserve.shop_id
                                            LEFT JOIN Menu ON Menu.menu_id = Reserve.menu_id
                                            WHERE Convert(date,reserve_date,103) = '{date.ToString("yyyy-MM-dd")}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReserveModel reserve = new ReserveModel()
                        {
                            reserve_id = dr["reserve_id"].ToString(),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            employee_nickname = dr["employee_nickname"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            delivery_service = dr["delivery_service"] != DBNull.Value ? Convert.ToDouble(dr["delivery_service"].ToString()) : 0.0,
                            delivery_service_per_person = 0.0,
                            menu_id = dr["menu_id"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            category_id = dr["category_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            amount_order = dr["amount_order"] != DBNull.Value ? Convert.ToInt32(dr["amount_order"].ToString()) : 0,
                            extra = dr["extra"] != DBNull.Value ? Convert.ToBoolean(dr["extra"].ToString()) : false,
                            note = dr["note"].ToString(),
                            reserve_date = dr["reserve_date"] != DBNull.Value ? Convert.ToDateTime(dr["reserve_date"].ToString()) : DateTime.MinValue,
                            remark = dr["remark"].ToString(),
                            status = dr["status"].ToString(),
                            review = dr["review"] != DBNull.Value ? Convert.ToInt32(dr["review"].ToString()) : 0,
                            sum_price = 0.0
                        };
                        reserves.Add(reserve);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return reserves;
        }

        public List<ReserveModel> GetReserveByDateEmployee(DateTime date, string employee_id)
        {
            List<ReserveModel> reserves = new List<ReserveModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT reserve_id,
                                                  Reserve.employee_id,
	                                              Employee.employee_name,
	                                              Employee.employee_nickname,
                                                  Reserve.shop_id,
	                                              Shop.shop_name,
	                                              Shop.delivery_service,
                                                  Reserve.menu_id,
	                                              Menu.menu_name,
                                                  Reserve.category_id,
	                                              Reserve.group_id,
	                                              Menu.price,
                                                  amount_order,
                                                  extra,
                                                  note,
                                                  reserve_date,
                                                  remark,
                                                  status,
                                                  review
                                            FROM Reserve
                                            LEFT JOIN Employee ON Employee.employee_id = Reserve.employee_id
                                            LEFT JOIN Shop ON Shop.shop_id = Reserve.shop_id
                                            LEFT JOIN Menu ON Menu.menu_id = Reserve.menu_id
                                            WHERE Convert(date,reserve_date,103) = '{date.ToString("yyyy-MM-dd")}' AND Reserve.employee_id = '{employee_id}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReserveModel reserve = new ReserveModel()
                        {
                            reserve_id = dr["reserve_id"].ToString(),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            employee_nickname = dr["employee_nickname"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            delivery_service = dr["delivery_service"] != DBNull.Value ? Convert.ToDouble(dr["delivery_service"].ToString()) : 0.0,
                            delivery_service_per_person = 0.0,
                            menu_id = dr["menu_id"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            category_id = dr["category_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            amount_order = dr["amount_order"] != DBNull.Value ? Convert.ToInt32(dr["amount_order"].ToString()) : 0,
                            extra = dr["extra"] != DBNull.Value ? Convert.ToBoolean(dr["extra"].ToString()) : false,
                            note = dr["note"].ToString(),
                            reserve_date = dr["reserve_date"] != DBNull.Value ? Convert.ToDateTime(dr["reserve_date"].ToString()) : DateTime.MinValue,
                            remark = dr["remark"].ToString(),
                            status = dr["status"].ToString(),
                            review = dr["review"] != DBNull.Value ? Convert.ToInt32(dr["review"].ToString()) : 0,
                            sum_price = 0.0
                        };
                        reserves.Add(reserve);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return reserves;
        }

        public List<ReserveModel> GetReserveByShopDate(string shop_id, DateTime date)
        {
            List<ReserveModel> reserves = new List<ReserveModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT reserve_id,
                                                  Reserve.employee_id,
	                                              Employee.employee_name,
	                                              Employee.employee_nickname,
                                                  Reserve.shop_id,
	                                              Shop.shop_name,
	                                              Shop.delivery_service,
                                                  Reserve.menu_id,
	                                              Menu.menu_name,
                                                  Reserve.category_id,
	                                              Reserve.group_id,
	                                              Menu.price,
                                                  amount_order,
                                                  extra,
                                                  note,
                                                  reserve_date,
                                                  remark,
                                                  status,
                                                  review
                                            FROM Reserve
                                            LEFT JOIN Employee ON Employee.employee_id = Reserve.employee_id
                                            LEFT JOIN Shop ON Shop.shop_id = Reserve.shop_id
                                            LEFT JOIN Menu ON Menu.menu_id = Reserve.menu_id
                                            WHERE Convert(date,reserve_date,103) = '{date.ToString("yyyy-MM-dd")}' AND
                                                  Reserve.shop_id = '{shop_id}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReserveModel reserve = new ReserveModel()
                        {
                            reserve_id = dr["reserve_id"].ToString(),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            employee_nickname = dr["employee_nickname"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            delivery_service = dr["delivery_service"] != DBNull.Value ? Convert.ToDouble(dr["delivery_service"].ToString()) : 0.0,
                            delivery_service_per_person = 0.0,
                            menu_id = dr["menu_id"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            category_id = dr["category_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            amount_order = dr["amount_order"] != DBNull.Value ? Convert.ToInt32(dr["amount_order"].ToString()) : 0,
                            extra = dr["extra"] != DBNull.Value ? Convert.ToBoolean(dr["extra"].ToString()) : false,
                            note = dr["note"].ToString(),
                            reserve_date = dr["reserve_date"] != DBNull.Value ? Convert.ToDateTime(dr["reserve_date"].ToString()) : DateTime.MinValue,
                            remark = dr["remark"].ToString(),
                            status = dr["status"].ToString(),
                            review = dr["review"] != DBNull.Value ? Convert.ToInt32(dr["review"].ToString()) : 0,
                            sum_price = 0.0
                        };
                        reserves.Add(reserve);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return reserves;
        }

        public List<ReserveModel> GetReserveByShopDateEmployee(string shop_id, DateTime date, string employee_id)
        {
            List<ReserveModel> reserves = new List<ReserveModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT reserve_id,
                                                  Reserve.employee_id,
	                                              Employee.employee_name,
	                                              Employee.employee_nickname,
                                                  Reserve.shop_id,
	                                              Shop.shop_name,
	                                              Shop.delivery_service,
                                                  Reserve.menu_id,
	                                              Menu.menu_name,
                                                  Reserve.category_id,
	                                              Reserve.group_id,
	                                              Menu.price,
                                                  amount_order,
                                                  extra,
                                                  note,
                                                  reserve_date,
                                                  remark,
                                                  status,
                                                  review
                                            FROM Reserve
                                            LEFT JOIN Employee ON Employee.employee_id = Reserve.employee_id
                                            LEFT JOIN Shop ON Shop.shop_id = Reserve.shop_id
                                            LEFT JOIN Menu ON Menu.menu_id = Reserve.menu_id
                                            WHERE Convert(date,reserve_date,103) = '{date.ToString("yyyy-MM-dd")}' AND
                                                  Reserve.employee_id = '{employee_id}' AND
                                                  Reserve.shop_id = '{shop_id}'");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReserveModel reserve = new ReserveModel()
                        {
                            reserve_id = dr["reserve_id"].ToString(),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            employee_nickname = dr["employee_nickname"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            delivery_service = dr["delivery_service"] != DBNull.Value ? Convert.ToDouble(dr["delivery_service"].ToString()) : 0.0,
                            delivery_service_per_person = 0.0,
                            menu_id = dr["menu_id"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            category_id = dr["category_id"].ToString(),
                            group_id = dr["group_id"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            amount_order = dr["amount_order"] != DBNull.Value ? Convert.ToInt32(dr["amount_order"].ToString()) : 0,
                            extra = dr["extra"] != DBNull.Value ? Convert.ToBoolean(dr["extra"].ToString()) : false,
                            note = dr["note"].ToString(),
                            reserve_date = dr["reserve_date"] != DBNull.Value ? Convert.ToDateTime(dr["reserve_date"].ToString()) : DateTime.MinValue,
                            remark = dr["remark"].ToString(),
                            status = dr["status"].ToString(),
                            review = dr["review"] != DBNull.Value ? Convert.ToInt32(dr["review"].ToString()) : 0,
                            sum_price = 0.0
                        };
                        reserves.Add(reserve);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return reserves;
        }

        public List<ReserveModel> GetReserves()
        {
            List<ReserveModel> reserves = new List<ReserveModel>();
            SqlConnection connection = ConnectSQL.OpenConnect();
            try
            {
                string strCmd = string.Format($@"SELECT reserve_id,
                                                  Reserve.employee_id,
	                                              Employee.employee_name,
	                                              Employee.employee_nickname,
                                                  Reserve.shop_id,
	                                              Shop.shop_name,
	                                              Shop.delivery_service,
                                                  Reserve.menu_id,
	                                              Menu.menu_name,
	                                              Menu.price,
                                                  amount_order,
                                                  extra,
                                                  note,
                                                  reserve_date,
                                                  remark,
                                                  status,
                                                  review
                                            FROM Reserve
                                            LEFT JOIN Employee ON Employee.employee_id = Reserve.employee_id
                                            LEFT JOIN Shop ON Shop.shop_id = Reserve.shop_id
                                            LEFT JOIN Menu ON Menu.menu_id = Reserve.menu_id");
                SqlCommand command = new SqlCommand(strCmd, connection);
                SqlDataReader dr = command.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ReserveModel reserve = new ReserveModel()
                        {
                            reserve_id = dr["reserve_id"].ToString(),
                            employee_id = dr["employee_id"].ToString(),
                            employee_name = dr["employee_name"].ToString(),
                            employee_nickname = dr["employee_nickname"].ToString(),
                            shop_id = dr["shop_id"].ToString(),
                            shop_name = dr["shop_name"].ToString(),
                            delivery_service = dr["delivery_service"] != DBNull.Value ? Convert.ToDouble(dr["delivery_service"].ToString()) : 0.0,
                            delivery_service_per_person = 0.0,
                            menu_id = dr["menu_id"].ToString(),
                            menu_name = dr["menu_name"].ToString(),
                            price = dr["price"] != DBNull.Value ? Convert.ToDouble(dr["price"].ToString()) : 0.0,
                            amount_order = dr["amount_order"] != DBNull.Value ? Convert.ToInt32(dr["amount_order"].ToString()) : 0,
                            extra = dr["extra"] != DBNull.Value ? Convert.ToBoolean(dr["extra"].ToString()) : false,
                            note = dr["note"].ToString(),
                            reserve_date = dr["reserve_date"] != DBNull.Value ? Convert.ToDateTime(dr["reserve_date"].ToString()) : DateTime.MinValue,
                            remark = dr["remark"].ToString(),
                            status = dr["status"].ToString(),
                            review = dr["review"] != DBNull.Value ? Convert.ToInt32(dr["review"].ToString()) : 0,
                            sum_price = 0.0
                        };
                        reserves.Add(reserve);
                    }
                    dr.Close();
                }
            }
            finally
            {
                connection.Close();
            }
            return reserves;
        }

        public string Insert(ReserveModel reserve)
        {
            throw new NotImplementedException();
        }
    }
}
