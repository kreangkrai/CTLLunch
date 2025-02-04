using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using Rotativa.AspNetCore;
using System.Data;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace CTLLunch.Controllers
{
    public class ReserveController : Controller
    {
        private IEmployee Employee;
        private IShop Shop;
        private IMenu Menu;
        private IReserve Reserve;
        private ITransaction Transaction;
        private IPlanCloseShop PlanCloseShop;
        private IPlanOutOfIngredients PlanOutOfIngredients;
        private readonly IHostingEnvironment hostingEnvironment;
        static string g_shop_id = "";
        static string path = "";
        public ReserveController(IEmployee _Employee, IShop _Shop, IMenu _Menu, IReserve _Reserve, IPlanCloseShop _PlanCloseShop, IPlanOutOfIngredients _PlanOutOfIngredients, ITransaction _Transaction, IHostingEnvironment _hostingEnvironment)
        {
            Employee = _Employee;
            Shop = _Shop;
            Menu = _Menu;
            Reserve = _Reserve;
            PlanCloseShop = _PlanCloseShop;
            PlanOutOfIngredients = _PlanOutOfIngredients;
            Transaction = _Transaction;
            hostingEnvironment = _hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<EmployeeModel> employees = new List<EmployeeModel>();
                employees = await Employee.GetEmployees();
                EmployeeModel employee = employees.Where(w => w.employee_name.ToLower() == user.ToLower()).Select(s => new EmployeeModel()
                {
                    employee_id = s.employee_id,
                    employee_name = s.employee_name,
                    employee_nickname = s.employee_nickname,
                    department = s.department,
                    role = s.role,
                    balance = s.balance
                }).FirstOrDefault();

                List<PlanCloseShopModel> plan_close_shop = await PlanCloseShop.GetPlanCloseShopsByDate(DateTime.Now);
                List<ShopModel> shops = await Shop.GetShops();
                shops = shops.Where(w => w.status == true).ToList();
                List<ShopModel> new_shops = new List<ShopModel>();
                for (int i = 0; i < shops.Count; i++)
                {
                    if (!plan_close_shop.Any(a => a.shop_id == shops[i].shop_id) && shops[i].status_close == true && shops[i].status == true)
                    {
                        new_shops.Add(shops[i]);
                    }
                }
                ViewBag.shops = new_shops;

                HttpContext.Session.SetString("Name", employee.employee_name);
                HttpContext.Session.SetString("Department", employee.department);
                HttpContext.Session.SetString("Role", employee.role);

                List<ReserveModel> reserves = await Reserve.GetReserveByShopDate("S001", DateTime.Now);
                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetDataReserveEmployee(string employee_id)
        {
            List<ReserveModel> reserves_employee = await Reserve.GetReserveByDateEmployee(DateTime.Now, employee_id);
            reserves_employee = reserves_employee.Where(w => w.status != "Cancel").ToList();

            List<ReserveModel> reserves_all = await Reserve.GetReserveByDate(DateTime.Now);
            List<MenuModel> menus = await Menu.GetMenus();
            EmployeeModel employee_ctl = await Employee.GetEmployeeCTL();

            List<DeliveryServiceModel> shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new DeliveryServiceModel()
            {
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Result.Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault(),
                count_reserve = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count(),
                delivery_service_per_person = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status == "Approved").Select(s1=>s1.delivery_service_per_person).FirstOrDefault(),
            }).ToList();

            
            for(int i = 0; i < shops.Count; i++)
            {
                // Re-Calculate Delivery Service
                int delivery_service = shops[i].delivery_service;
                int count_reserve = shops[i].count_reserve;
                if (shops[i].delivery_service_per_person == 0)
                {
                    AmountDeliveryBalanceModel amount = await Reserve.ComputeAmountDeliveryBalance(delivery_service, count_reserve, employee_ctl.balance);
                    shops[i].delivery_service_per_person = amount.delivery_service;
                }               
            }

            for (int i = 0; i < reserves_employee.Count; i++)
            {              
                int delivery_serveice_per_person = shops.Where(w => w.shop_id == reserves_employee[i].shop_id).Select(s => s.delivery_service_per_person).FirstOrDefault();
                reserves_employee[i].delivery_service_per_person = delivery_serveice_per_person;
            }

            reserves_employee = reserves_employee.GroupBy(g => g.reserve_id).Select(s => new ReserveModel()
            {
                reserve_id = s.Key,
                employee_id = s.FirstOrDefault().employee_id,
                employee_name = s.FirstOrDefault().employee_name,
                employee_nickname = s.FirstOrDefault().employee_nickname,
                shop_id = s.FirstOrDefault().shop_id,
                shop_name = s.FirstOrDefault().shop_name,
                menu_id = s.FirstOrDefault().menu_id,
                menu_name = string.Join('+', s.Select(f => f.menu_name).ToArray()),
                category_id = s.FirstOrDefault().category_id,
                group_id = s.FirstOrDefault().group_id,
                amount_order = s.FirstOrDefault().amount_order,
                extra = s.FirstOrDefault().extra,
                note = string.Join(' ', s.Select(f => f.note).ToArray()),
                remark = string.Join(' ', s.Select(f => f.remark).ToArray()),
                review = s.FirstOrDefault().review,
                reserve_date = s.FirstOrDefault().reserve_date,
                status = s.FirstOrDefault().status,
                price = s.Sum(f => f.price),
                delivery_service = s.FirstOrDefault().delivery_service,
                delivery_service_per_person = s.FirstOrDefault().delivery_service_per_person,
                sum_price = s.Sum(f => f.price) + s.FirstOrDefault().delivery_service_per_person,
            }).ToList();

            List<string> menus_id = reserves_employee.GroupBy(g => g.menu_id).Select(s => s.FirstOrDefault().menu_id).ToList();
            menus = menus.Where(w=> menus_id.Contains(w.menu_id)).ToList();

            var data = new { reserves_employee = reserves_employee, menus = menus };
            return Json(data);
        }
        [HttpGet]
        public async Task<JsonResult> GetDataReserveShop(string shop_id)
        {
            g_shop_id = shop_id;
            List<ReserveModel> reserves_shop = await Reserve.GetReserveByShopDate(shop_id, DateTime.Now);
            reserves_shop = reserves_shop.Where(w => w.status == "Pending").ToList();

            List<ReserveModel> reserves_all = await Reserve.GetReserveByDate(DateTime.Now);
            List<PlanOutOfIngredientsModel> plans = await PlanOutOfIngredients.GetPlanOutOfIngredientsByDate(DateTime.Now);
            EmployeeModel employee_ctl = await Employee.GetEmployeeCTL();

            List<MenuModel> menus = await Menu.GetMenuByShop(shop_id);
            List<MenuModel> _menus = new List<MenuModel>();
            for (int i = 0; i < menus.Count; i++)
            {
                if (!plans.Any(a => a.ingredients_id == menus[i].ingredients_id && a.shop_id == menus[i].shop_id))
                {
                    _menus.Add(menus[i]);
                }
            }

            List<DeliveryServiceModel> shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new DeliveryServiceModel() {
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Result.Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault(),
                count_reserve = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count(),
                delivery_service_per_person = 0,
            }).ToList();

            for (int i = 0; i < shops.Count; i++)
            {
                // Re-Calculate Delivery Service
                int delivery_service = shops[i].delivery_service;
                int count_reserve = shops[i].count_reserve;
                AmountDeliveryBalanceModel amount = await Reserve.ComputeAmountDeliveryBalance(delivery_service, count_reserve, employee_ctl.balance);
                shops[i].delivery_service_per_person = amount.delivery_service;
            }

            for (int i = 0; i < reserves_shop.Count; i++)
            {
                int delivery_serveice_per_person = shops.Where(w => w.shop_id == reserves_shop[i].shop_id).Select(s => s.delivery_service_per_person).FirstOrDefault();
                reserves_shop[i].delivery_service_per_person = delivery_serveice_per_person;
            }

            reserves_shop = reserves_shop.GroupBy(g => g.reserve_id).Select(s => new ReserveModel()
            {
                reserve_id = s.Key,
                employee_id = s.FirstOrDefault().employee_id,
                employee_name = s.FirstOrDefault().employee_name,
                employee_nickname = s.FirstOrDefault().employee_nickname,
                shop_id = s.FirstOrDefault().shop_id,
                shop_name = s.FirstOrDefault().shop_name,
                menu_id = s.FirstOrDefault().menu_id,
                menu_name = string.Join('+', s.Select(f => f.menu_name).ToArray()),
                category_id = s.FirstOrDefault().category_id,
                group_id = s.FirstOrDefault().group_id,
                amount_order = s.FirstOrDefault().amount_order,
                extra = s.FirstOrDefault().extra,
                note = string.Join(' ', s.Select(f => f.note).ToArray()),
                remark = string.Join(' ', s.Select(f => f.remark).ToArray()),
                review = s.FirstOrDefault().review,
                reserve_date = s.FirstOrDefault().reserve_date,
                status = s.FirstOrDefault().status,
                price = s.Sum(f => f.price),
                delivery_service = s.FirstOrDefault().delivery_service,
                delivery_service_per_person = s.FirstOrDefault().delivery_service_per_person,
                sum_price = s.Sum(f => f.price) + s.FirstOrDefault().delivery_service_per_person,
            }).ToList();

            reserves_shop = reserves_shop.OrderBy(o => o.menu_name).ToList();

            // Group Menu
            List<GroupShopMenuModel> groups = _menus.GroupBy(g => g.group_id).Select(s => new GroupShopMenuModel()
            {
                group_id = s.Key,
                group_name = _menus.Where(w => w.group_id == s.Key && w.status == true).FirstOrDefault().group_name,
                menus = _menus.Where(w => w.group_id == s.Key && w.status == true).ToList()
            }).ToList();

            var data = new { reserves_shop = reserves_shop, menus = _menus, groups = groups };
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetMenuByShop(string shop_id)
        {
            List<MenuModel> menus = await Menu.GetMenuByShop(shop_id);
            List<GroupMenuModel> groupMenus = menus.GroupBy(g => g.group_id).Select(s => new GroupMenuModel()
            {
                group_id = s.Key,
                group_name = menus.Where(w => w.group_id == s.Key).Select(x => x.group_name).FirstOrDefault()
            }).ToList();
            var data = new { menu = menus, group = groupMenus };
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> SearchMenuByShop(string shop_id, string menu)
        {
            List<PlanOutOfIngredientsModel> plans = await PlanOutOfIngredients.GetPlanOutOfIngredientsByDate(DateTime.Now);
            List<ShopModel> shops = await Shop.GetShops();
            ShopModel shop = shops.Where(w => w.shop_id == shop_id).FirstOrDefault();
            List<MenuModel> menus = await Menu.SearchMenuByShop(shop_id, menu);
            List<MenuModel> _menus = new List<MenuModel>();
            List<GroupShopMenuModel> groups = new List<GroupShopMenuModel>();
            if (shop.close_time_shift.TotalMinutes > DateTime.Now.TimeOfDay.TotalMinutes)
            {
                if (menus != null)
                {
                    for (int i = 0; i < menus.Count; i++)
                    {
                        if (!plans.Any(a => a.ingredients_id == menus[i].ingredients_id && a.shop_id == menus[i].shop_id))
                        {
                            _menus.Add(menus[i]);
                        }
                    }
                }

                // Group Menu
                groups = _menus.GroupBy(g => g.group_id).Select(s => new GroupShopMenuModel()
                {
                    group_id = s.Key,
                    group_name = _menus.Where(w => w.group_id == s.Key).FirstOrDefault().group_name,
                    menus = _menus.Where(w => w.group_id == s.Key).ToList()
                }).ToList();

            }
            
            var data = new { menus = _menus, groups = groups };
            return Json(data);
        }

        [HttpGet]
        public async Task<int> GetExtraPriceByMenu(string menu_id)
        {
            MenuModel menu = await Menu.GetMenuByMenu(menu_id);
            return menu.extra_price;
        }

        [HttpDelete]
        public async Task<string> UpdateReserveStatus(string reserve_id)
        {
            string message = await Reserve.UpdateStatus(reserve_id, "Cancel");
            return message;
        }
        [HttpPost]
        public async Task<string> InsertReserve(List<string> strs)
        {
            string user = HttpContext.Session.GetString("userId");
            List<EmployeeModel> employees = await Employee.GetEmployees();
            EmployeeModel employee = employees.Where(w => w.employee_name.ToLower() == user.ToLower()).FirstOrDefault();
            double balance = employee.balance;

            List<ReserveModel> reserves_emp = await Reserve.GetReserveByDateEmployee(DateTime.Now, employee.employee_id);
            reserves_emp = reserves_emp.Where(w => w.status == "Pending").ToList();

            double sum_price = 0;
            for (int i = 0; i < reserves_emp.Count; i++)
            {
                if (reserves_emp[i].extra)
                {
                    MenuModel _menu = await Menu.GetMenuByMenu(reserves_emp[i].menu_id);
                    sum_price += _menu.extra_price;
                }
                sum_price += reserves_emp[i].price;
            }

            // List Shop

            List<string> shop_id = reserves_emp.GroupBy(g => g.shop_id).Select(s => s.Key).ToList();
            //Current Shop
            double sum_delivery_service_per_reserve = 0;
            ReserveModel _reserve = JsonConvert.DeserializeObject<ReserveModel>(strs[0]);
            List<ReserveModel> reserves_shop = await Reserve.GetReserveByShopDate(_reserve.shop_id, DateTime.Now);
            List<ShopModel> _shops = await Shop.GetShops();
            ShopModel _shop = _shops.Where(w => w.shop_id == _reserve.shop_id).FirstOrDefault();

            TimeSpan time_shop = _shop.close_time_shift;
            if (time_shop.TotalMinutes > DateTime.Now.TimeOfDay.TotalMinutes && _shop.status == true)
            {
                int count_reserve = reserves_shop.Where(w => w.group_id != "G99" && w.status == "Pending").Count() + 1;
                List<ReserveModel> list_menus = reserves_shop.Where(w => w.group_id != "G99" && w.status == "Pending").GroupBy(g => g.menu_id).Select(s => new ReserveModel()
                {
                    menu_id = s.Key
                }).ToList();

                int count_limit_menu = reserves_shop.Where(w => w.group_id != "G99" && w.status == "Pending").GroupBy(g => g.menu_id).Count();
                int count_limit_order = reserves_shop.Where(w => w.group_id != "G99" && w.status == "Pending").Count();
                double delivery_service_per_reserve = _shop.delivery_service / (double)count_reserve;
                sum_delivery_service_per_reserve = (reserves_emp.Where(w => w.shop_id == _reserve.shop_id && w.group_id != "G99").Count() * delivery_service_per_reserve) + delivery_service_per_reserve;

                // Sum All Delivery Service Per Person
                shop_id.Remove(_reserve.shop_id); // Remove Current Shop
                for (int i = 0; i < shop_id.Count; i++)
                {
                    List<ReserveModel> _reserves_shop = await Reserve.GetReserveByShopDate(shop_id[i], DateTime.Now);
                    List<ShopModel> _shops_ = await Shop.GetShops();
                    ShopModel _shop_ = _shops_.Where(w => w.shop_id == shop_id[i]).FirstOrDefault();
                    int _count_reserve = _reserves_shop.Where(w => w.group_id != "G99" && w.status == "Pending").Count() + 1;
                    double _delivery_service_per_reserve = _shop_.delivery_service / (double)_count_reserve;
                    sum_delivery_service_per_reserve += (reserves_emp.Where(w => w.shop_id == shop_id[i] && w.group_id != "G99").Count() * _delivery_service_per_reserve) + _delivery_service_per_reserve;
                }

                string reserve_id = $"RES{DateTime.Now.ToString("ddMMyyyyHHmmssfff")}";
                DateTime date = DateTime.Now;
                string message = "";
                for (int i = 0; i < strs.Count; i++)
                {
                    ReserveModel reserve = JsonConvert.DeserializeObject<ReserveModel>(strs[i]);
                    MenuModel menu = await Menu.GetMenuByMenu(reserve.menu_id);
                    int extra_price = 0;
                    if (reserve.extra)
                    {
                        extra_price = menu.extra_price;
                    }
                    reserve.reserve_id = reserve_id;
                    reserve.amount_order = 1;
                    reserve.category_id = menu.category_id;
                    reserve.group_id = menu.group_id;
                    reserve.reserve_date = date;
                    reserve.status = "Pending";
                    reserve.review = 0;
                    reserve.price = menu.price;

                    if (count_limit_menu < _shop.limit_menu || list_menus.Any(a => a.menu_id == reserve.menu_id))
                    {
                        if (count_limit_order < _shop.limit_order)
                        {
                            if (count_limit_order > 1)
                            {
                                if (balance - (sum_price + reserve.price + extra_price + sum_delivery_service_per_reserve) >= 20)
                                {
                                    message = await Reserve.Insert(reserve);
                                }
                                else
                                {
                                    return "ยอดเงินไม่เพียงพอ";
                                }
                            }
                            else
                            {
                                if (balance - (sum_price + reserve.price + extra_price) >= 20)
                                {
                                    message = await Reserve.Insert(reserve);
                                }
                                else
                                {
                                    return "ยอดเงินไม่เพียงพอ";
                                }
                            }
                        }
                        else
                        {
                            return "จำนวนการสั่งชื้อเกินกำหนด กรุณาเปลี่ยนร้านอาหาร";
                        }
                    }
                    else
                    {
                        return "จำนวนเมนูที่สั่งได้เกินกำหนด กรุณาสั่งตามเพื่อน";
                    }
                }
                return message;
            }
            else
            {
                return "หมดเวลาสั่งอาหาร/ร้านปิด";
            }
        }

        [HttpPut]
        public async Task<string> UpdatePayReserve(List<string> strs)
        {
            string message = "";
            string receiver_id = "";
            for (int i = 0; i < strs.Count; i++)
            {
                ReserveModel reserve = JsonConvert.DeserializeObject<ReserveModel>(strs[i]);
                List<ReserveModel> reserves_ = await Reserve.GetReserves();
                ReserveModel reserve_ = reserves_.Where(w => w.reserve_id == reserve.reserve_id).FirstOrDefault();
                reserve.delivery_service_per_person = reserve.delivery_service_per_person;
                message = await Reserve.UpdateStatus(reserve.reserve_id, "Approved");
                if (message == "Success")
                {
                    message = await Reserve.UpdateDelivery(reserve);
                    if (message == "Success")
                    {
                        // Update Balance
                        List<EmployeeModel> employees = await Employee.GetEmployees();
                        EmployeeModel employee = employees.Where(w => w.employee_id == reserve_.employee_id).FirstOrDefault();
                        int old_balance = employee.balance;
                        int new_balance = old_balance - (reserve.price + reserve.delivery_service_per_person);
                        employee.balance = new_balance;
                        message = await Employee .UpdateBalance(employee);
                        if (message == "Success")
                        {
                            // Insert Transaction
                            string user = HttpContext.Session.GetString("userId");
                            List<EmployeeModel> _employees = await Employee.GetEmployees();
                            EmployeeModel _employee = _employees.Where(w => w.employee_name.ToLower() == user.ToLower()).FirstOrDefault();
                            receiver_id = _employee.employee_id;
                            TransactionModel transaction = new TransactionModel()
                            {
                                employee_id = employee.employee_id,
                                receiver_id = receiver_id,
                                type = "Pay",
                                amount = reserve.price + reserve.delivery_service_per_person,
                                date = DateTime.Now,
                                note = "",
                            };
                            message = await Transaction.Insert(transaction);
                        }
                    }
                }

                // Send Update Transaction เงินกองกลาง Employee
                if (i == strs.Count - 1)
                {
                    if (message == "Success")
                    {
                        int sum_delivery_service = reserve.delivery_service_per_person * strs.Count;
                        int remainder = 0;
                        List<EmployeeModel> _employees = await Employee.GetEmployees();
                        EmployeeModel _employee = _employees.Where(w => w.employee_id == "EM999").FirstOrDefault();
                        int old_balance = _employee.balance;
                        int remain_balance = sum_delivery_service - reserve_.delivery_service;
                        if (remain_balance != 0)
                        {
                            if (remain_balance < old_balance)
                            {
                                if (remain_balance < 0)
                                {
                                    remainder = old_balance + remain_balance;
                                }
                                else
                                {
                                    remainder = old_balance - remain_balance;
                                }
                                EmployeeModel employee = new EmployeeModel()
                                {
                                    employee_id = "EM999",
                                    balance = remainder,
                                };
                                message = await Employee.UpdateBalance(employee);

                                if (message == "Success")
                                {
                                    // Insert Transaction                               
                                    TransactionModel transaction = new TransactionModel()
                                    {
                                        employee_id = employee.employee_id,
                                        receiver_id = receiver_id,
                                        type = "Pay",
                                        amount = Math.Abs(remain_balance),
                                        date = DateTime.Now,
                                        note = "",
                                    };
                                    message = await Transaction.Insert(transaction);
                                }
                            }
                            if (remain_balance > old_balance)
                            {
                                remainder = old_balance + remain_balance;
                                EmployeeModel employee = new EmployeeModel()
                                {
                                    employee_id = "EM999",
                                    balance = remainder,
                                };
                                message = await Employee .UpdateBalance(employee);

                                if (message == "Success")
                                {
                                    // Insert Transaction                               
                                    TransactionModel transaction = new TransactionModel()
                                    {
                                        employee_id = employee.employee_id,
                                        receiver_id = receiver_id,
                                        type = "Add",
                                        amount = Math.Abs(remain_balance),
                                        date = DateTime.Now,
                                        note = "",
                                    };
                                    message = await Transaction.Insert(transaction);
                                }
                            }                           
                        }                       
                    }
                    if (message == "Success")
                    {
                        message = await Shop.UpdateCloseTimeShift(reserve_.shop_id);
                    }
                }
            }

            return message;
        }

        [HttpPut]
        public async Task<string> UpdateReviewReserve(string reserve_id,int review)
        {
            string message = await Reserve.UpdateReview(reserve_id, review);
            return message;
        }

        public async Task<IActionResult> FormSummaryReserve()
        {
            List<ReserveModel> reserves_shop = await Reserve.GetReserveByShopDate(g_shop_id, DateTime.Now);
            reserves_shop = reserves_shop.Where(w => w.status == "Pending").ToList();

            List<ReserveModel> reserves_all = await Reserve.GetReserveByDate(DateTime.Now);
            List<PlanOutOfIngredientsModel> plans = await PlanOutOfIngredients.GetPlanOutOfIngredientsByDate(DateTime.Now);
            EmployeeModel employee_ctl = await Employee.GetEmployeeCTL();
            List<MenuModel> menus = await Menu.GetMenuByShop(g_shop_id);

            List<DeliveryServiceModel> shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new DeliveryServiceModel()
            {
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Result.Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault(),
                count_reserve = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count(),
                delivery_service_per_person = 0,
            }).ToList();

            for (int i = 0; i < shops.Count; i++)
            {
                // Re-Calculate Delivery Service
                int delivery_service = shops[i].delivery_service;
                int count_reserve = shops[i].count_reserve;
                AmountDeliveryBalanceModel amount = await Reserve.ComputeAmountDeliveryBalance(delivery_service, count_reserve, employee_ctl.balance);
                shops[i].delivery_service_per_person = amount.delivery_service;
            }

            for (int i = 0; i < reserves_shop.Count; i++)
            {
                int delivery_serveice_per_person = shops.Where(w => w.shop_id == reserves_shop[i].shop_id).Select(s => s.delivery_service_per_person).FirstOrDefault();
                reserves_shop[i].delivery_service_per_person = delivery_serveice_per_person;
            }

            reserves_shop = reserves_shop.GroupBy(g => g.reserve_id).Select(s => new ReserveModel()
            {
                reserve_id = s.Key,
                employee_id = s.FirstOrDefault().employee_id,
                employee_name = s.FirstOrDefault().employee_name,
                employee_nickname = s.FirstOrDefault().employee_nickname,
                shop_id = s.FirstOrDefault().shop_id,
                shop_name = s.FirstOrDefault().shop_name,
                menu_id = s.FirstOrDefault().menu_id,
                menu_name = string.Join('+', s.Select(f => f.menu_name).ToArray()),
                category_id = s.FirstOrDefault().category_id,
                group_id = s.FirstOrDefault().group_id,
                amount_order = s.FirstOrDefault().amount_order,
                extra = s.FirstOrDefault().extra,
                note = string.Join(' ', s.Select(f => f.note).ToArray()),
                remark = string.Join(' ', s.Select(f => f.remark).ToArray()),
                review = s.FirstOrDefault().review,
                reserve_date = s.FirstOrDefault().reserve_date,
                status = s.FirstOrDefault().status,
                price = s.Sum(f => f.price),
                delivery_service = s.FirstOrDefault().delivery_service,
                delivery_service_per_person = s.FirstOrDefault().delivery_service_per_person,
                sum_price = s.Sum(f => f.price) + s.FirstOrDefault().delivery_service_per_person,
            }).ToList();

            reserves_shop = reserves_shop.OrderBy(o => o.menu_name).ToList();
            List<SummaryReserveModel> summaries = new List<SummaryReserveModel>();
            for (int i = 0; i < reserves_shop.Count; i++)
            {
                int extra_price = 0;
                List<string> note_remark = new List<string>();
                if (reserves_shop[i].note.Trim() != "")
                {
                    note_remark.Add(reserves_shop[i].note.Trim());
                }
                if (reserves_shop[i].remark.Trim() != "")
                {
                    note_remark.Add(reserves_shop[i].remark.Trim());
                }

                string _note_remark = string.Join(",", note_remark.ToArray());
                if (_note_remark.Trim() != "" && _note_remark.Trim() != ",")
                {
                    _note_remark = $" ( {_note_remark} )";
                }
                else
                {
                    _note_remark = "";
                }

                if (reserves_shop[i].extra == true)
                {
                    extra_price = menus.Where(w => w.menu_id == reserves_shop[i].menu_id).Select(s => s.extra_price).FirstOrDefault();
                }

                SummaryReserveModel summary = new SummaryReserveModel()
                {
                    employee_id = reserves_shop[i].employee_nickname,
                    shop_name = reserves_shop[i].shop_name,
                    menu_id = reserves_shop[i].menu_id,
                    menu = reserves_shop[i].menu_name + _note_remark,
                    price = reserves_shop[i].price + extra_price,
                    delivery = reserves_shop[i].delivery_service_per_person,
                };
                summaries.Add(summary);

            }
            summaries = summaries.OrderBy(o => o.menu).ToList();

            //Summary
            int sum_price = 0;
            int sum_delivery = 0;
            for (int j = 0; j < summaries.Count; j++)
            {
                sum_price += summaries[j].price;
                sum_delivery += summaries[j].delivery;
            }
            
            int _delivery_service = reserves_shop[0].delivery_service;
            SummaryReserveModel _summary = new SummaryReserveModel()
            {
                employee_id = "รวม",
                shop_name = "",
                menu = "",
                menu_id = "",
                price = sum_price,
                delivery = _delivery_service,
            };
            summaries.Add(_summary);

            SummaryReserveModel summary_ = new SummaryReserveModel()
            {
                employee_id = "รวมทั้งหมด",
                shop_name = "",
                menu = "",
                menu_id = "",
                price = 0,
                delivery = sum_price + summaries[summaries.Count-1].delivery,
            };
            summaries.Add(summary_);

            var form = new ViewAsPdf("FormSummaryReserve")
            {
                Model = summaries,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins { Top = 5, Left = 5, Right = 5, Bottom = 2 }
            };
            return form;
        }

        [HttpPost]
        public string InsertPath(string reserve_id)
        {
            path = reserve_id;
            return "Success";
        }

        [HttpPost]
        public IActionResult ImportFile()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "backup/pay/" + path;
            string webRootPath = hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(newPath);
                foreach (FileInfo f in di.GetFiles())
                {
                    f.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {

                    dir.Delete(true);
                }
                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)
            {
                string fullPath = Path.Combine(newPath, file.FileName);
                FileStream stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);

                stream.Position = 0;
                stream.Close();
            }
            return Json("Success");
        }
        [HttpGet]
        public async Task<IActionResult> GetPathImageReserve(string reserve_id)
        {
            try
            {
                path = reserve_id;
                string folderName = "backup/pay/" + path;
                string webRootPath = hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                DirectoryInfo di = new DirectoryInfo(newPath);
                FileInfo[] Images = di.GetFiles("*.*");
                string fullpath = folderName + "/" + Images[0].Name;
                string scheme = Request.Scheme;
                string host = Request.Host.Value;
                string _path = scheme + "://" + host + "/lunch/" + fullpath;
                string base64 = await GetImageAsBase64Url(_path);
                return Json(base64);
            }
            catch
            {
                return Json("ไม่มีสลิป");
            }
        }
        public async static Task<string> GetImageAsBase64Url(string url)
        {
            var credentials = new NetworkCredential();
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var client = new HttpClient(handler))
            {
                var bytes = await client.GetByteArrayAsync(url);
                return "image/jpeg;base64," + Convert.ToBase64String(bytes);
            }
        }
    }
}
