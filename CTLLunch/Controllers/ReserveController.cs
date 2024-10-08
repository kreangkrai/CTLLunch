using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

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
        public ReserveController(IEmployee _Employee, IShop _Shop, IMenu _Menu, IReserve _Reserve, IPlanCloseShop _PlanCloseShop, IPlanOutOfIngredients _PlanOutOfIngredients, ITransaction _Transaction)
        {
            Employee = _Employee;
            Shop = _Shop;
            Menu = _Menu;
            Reserve = _Reserve;
            PlanCloseShop = _PlanCloseShop;
            PlanOutOfIngredients = _PlanOutOfIngredients;
            Transaction = _Transaction;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<EmployeeModel> employees = new List<EmployeeModel>();
                employees = Employee.GetEmployees();
                EmployeeModel employee = employees.Where(w => w.employee_name.ToLower() == user.ToLower()).Select(s => new EmployeeModel()
                {
                    employee_id = s.employee_id,
                    employee_name = s.employee_name,
                    employee_nickname = s.employee_nickname,
                    department = s.department,
                    role = s.role,
                    balance = s.balance
                }).FirstOrDefault();

                List<PlanCloseShopModel> plan_close_shop = PlanCloseShop.GetPlanCloseShopsByDate(DateTime.Now);
                List<ShopModel> shops = Shop.GetShops();
                List<ShopModel> new_shops = new List<ShopModel>();
                for (int i = 0; i < shops.Count; i++)
                {
                    if (!plan_close_shop.Any(a => a.shop_id == shops[i].shop_id))
                    {
                        new_shops.Add(shops[i]);
                    }
                }
                ViewBag.shops = new_shops;

                HttpContext.Session.SetString("Name", employee.employee_name);
                HttpContext.Session.SetString("Department", employee.department);
                HttpContext.Session.SetString("Role", employee.role);

                List<ReserveModel> reserves = Reserve.GetReserveByShopDate("S001", DateTime.Now);
                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpGet]
        public JsonResult GetDataReserveEmployee(string employee_id)
        {
            List<ReserveModel> reserves_employee = Reserve.GetReserveByDateEmployee(DateTime.Now, employee_id).Where(w => w.status != "Cancel").ToList();
            List<ReserveModel> reserves_all = Reserve.GetReserveByDate(DateTime.Now);
            List<MenuModel> menus = Menu.GetMenus();

            var shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new {
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault(),
                amount_order = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count(),
                delivery_service_per_person = Shop.GetShops().Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault() / (double)reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count()
            }).ToList();

            for (int i = 0; i < reserves_employee.Count; i++)
            {
                reserves_employee[i].delivery_service_per_person = shops.Where(w => w.shop_id == reserves_employee[i].shop_id).Select(s => s.delivery_service_per_person).FirstOrDefault();
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
                sum_price = (double)s.Sum(f => f.price) + (double)s.FirstOrDefault().delivery_service_per_person,
            }).ToList();

            var data = new { reserves_employee = reserves_employee, menus = menus };
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetDataReserveShop(string shop_id)
        {
            List<ReserveModel> reserves_shop = Reserve.GetReserveByShopDate(shop_id, DateTime.Now).Where(w => w.status == "Pending").ToList(); ;
            List<ReserveModel> reserves_all = Reserve.GetReserveByDate(DateTime.Now);
            List<PlanOutOfIngredientsModel> plans = PlanOutOfIngredients.GetPlanOutOfIngredientsByDate(DateTime.Now);

            List<MenuModel> menus = Menu.GetMenuByShop(shop_id);
            List<MenuModel> _menus = new List<MenuModel>();
            for (int i = 0; i < menus.Count; i++)
            {
                if (!plans.Any(a => a.ingredients_id == menus[i].ingredients_id && a.shop_id == menus[i].shop_id))
                {
                    _menus.Add(menus[i]);
                }
            }

            var shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new {
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault(),
                amount_order = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count(),
                delivery_service_per_person = Shop.GetShops().Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault() / (double)reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count()
            }).ToList();

            for (int i = 0; i < reserves_shop.Count; i++)
            {
                reserves_shop[i].delivery_service_per_person = shops.Where(w => w.shop_id == reserves_shop[i].shop_id).Select(s => s.delivery_service_per_person).FirstOrDefault();
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
                sum_price = (double)s.Sum(f => f.price) + (double)s.FirstOrDefault().delivery_service_per_person,
            }).ToList();

            // Group Menu
            List<GroupShopMenuModel> groups = _menus.GroupBy(g => g.group_id).Select(s => new GroupShopMenuModel()
            {
                group_id = s.Key,
                group_name = _menus.Where(w => w.group_id == s.Key).FirstOrDefault().group_name,
                menus = _menus.Where(w => w.group_id == s.Key).ToList()
            }).ToList();

            var data = new { reserves_shop = reserves_shop, menus = _menus, groups = groups };
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetMenuByShop(string shop_id)
        {
            List<MenuModel> menus = Menu.GetMenuByShop(shop_id);
            List<GroupMenuModel> groupMenus = menus.GroupBy(g => g.group_id).Select(s => new GroupMenuModel()
            {
                group_id = s.Key,
                group_name = menus.Where(w => w.group_id == s.Key).Select(x => x.group_name).FirstOrDefault()
            }).ToList();
            var data = new { menu = menus, group = groupMenus };
            return Json(data);
        }

        [HttpGet]
        public JsonResult SearchMenuByShop(string shop_id, string menu)
        {
            List<PlanOutOfIngredientsModel> plans = PlanOutOfIngredients.GetPlanOutOfIngredientsByDate(DateTime.Now);

            List<MenuModel> menus = Menu.SearchMenuByShop(shop_id, menu);
            List<MenuModel> _menus = new List<MenuModel>();
            for (int i = 0; i < menus.Count; i++)
            {
                if (!plans.Any(a => a.ingredients_id == menus[i].ingredients_id && a.shop_id == menus[i].shop_id))
                {
                    _menus.Add(menus[i]);
                }
            }

            // Group Menu
            List<GroupShopMenuModel> groups = _menus.GroupBy(g => g.group_id).Select(s => new GroupShopMenuModel()
            {
                group_id = s.Key,
                group_name = _menus.Where(w => w.group_id == s.Key).FirstOrDefault().group_name,
                menus = _menus.Where(w => w.group_id == s.Key).ToList()
            }).ToList();

            var data = new { menus = _menus, groups = groups };
            return Json(data);
        }

        [HttpGet]
        public int GetExtraPriceByMenu(string menu_id)
        {
            MenuModel menu = Menu.GetMenuByMenu(menu_id);
            return menu.extra_price;
        }

        [HttpDelete]
        public string UpdateReserveStatus(string reserve_id)
        {
            string message = Reserve.UpdateStatus(reserve_id, "Cancel");
            return message;
        }
        [HttpPost]
        public string InsertReserve(List<string> strs)
        {
            string user = HttpContext.Session.GetString("userId");
            EmployeeModel employee = Employee.GetEmployees().Where(w => w.employee_name.ToLower() == user.ToLower()).FirstOrDefault();
            double balance = employee.balance;

            List<ReserveModel> reserves_emp = Reserve.GetReserveByDateEmployee(DateTime.Now, employee.employee_id).
                                                  Where(w => w.status == "Pending").ToList();
            double sum_price = reserves_emp.Sum(s => s.price);

            //Current Shop
            
            ReserveModel _reserve = JsonConvert.DeserializeObject<ReserveModel>(strs[0]);
            List<ReserveModel> reserves_shop = Reserve.GetReserveByShopDate(_reserve.shop_id, DateTime.Now).ToList();
            ShopModel _shop = Shop.GetShops().Where(w=>w.shop_id == _reserve.shop_id).FirstOrDefault();

            int count_reserve = reserves_shop.Where(w => w.group_id != "G99" && w.status == "Pending").Count() + 1;
            int count_limit_order = reserves_shop.Where(w=>w.group_id != "G99").GroupBy(g=>g.menu_id).Count();
            int count_limit_menu = reserves_shop.Where(w => w.group_id != "G99").Count();
            double delivery_service_per_reserve = _shop.delivery_service / (double)count_reserve;

            string reserve_id = $"RES{DateTime.Now.ToString("ddMMyyyyHHmmssfff")}";
            DateTime date = DateTime.Now;
            string message = "";
            for (int i = 0; i < strs.Count; i++)
            {
                ReserveModel reserve = JsonConvert.DeserializeObject<ReserveModel>(strs[i]);
                MenuModel menu = Menu.GetMenuByMenu(reserve.menu_id);
                reserve.reserve_id = reserve_id;
                reserve.amount_order = 1;
                reserve.category_id = menu.category_id;
                reserve.group_id = menu.group_id;
                reserve.reserve_date = date;
                reserve.status = "Pending";
                reserve.review = 0;
                reserve.price = menu.price;
                if (count_limit_menu < _shop.limit_menu)
                {
                    if (count_limit_order < _shop.limit_order)
                    {
                        if (balance - (sum_price + reserve.price + delivery_service_per_reserve) >= 20)
                        {
                            message = Reserve.Insert(reserve);
                        }
                        else
                        {
                            return "ยอดเงินไม่เพียงพอ";
                        }
                    }
                    else
                    {
                        return "จำนวนเที่สั่งได้เกินที่กำหนด";
                    }
                }
                else
                {
                    return "จำนวนเมนูได้เกินที่กำหนด";
                }
            }
            return message;
        }

        [HttpPut]
        public string UpdatePayReserve(List<string> strs)
        {
            string message = "";
            for (int i = 0; i < strs.Count; i++)
            {
                ReserveModel reserve = JsonConvert.DeserializeObject<ReserveModel>(strs[i]);
                ReserveModel reserve_ = Reserve.GetReserves().Where(w => w.reserve_id == reserve.reserve_id).FirstOrDefault();
                reserve.delivery_service = reserve.delivery_service_per_person;
                message = Reserve.UpdateStatus(reserve.reserve_id, "Approved");
                if (message == "Success")
                {
                    message = Reserve.UpdateDelivery(reserve);
                    if (message == "Success")
                    {
                        // Update Balance
                        EmployeeModel employee = Employee.GetEmployees().Where(w => w.employee_id == reserve_.employee_id).FirstOrDefault();
                        double old_balance = employee.balance;
                        double new_balance = old_balance - (reserve.price + reserve.delivery_service);
                        employee.balance = new_balance;
                        message = Employee.UpdateBalance(employee);
                        if (message == "Success")
                        {
                            // Insert Transaction
                            string user = HttpContext.Session.GetString("userId");
                            string receiver_id = employee.employee_id;
                            TransactionModel transaction = new TransactionModel()
                            {
                                employee_id = employee.employee_id,
                                receiver_id = receiver_id,
                                type = "Pay",
                                amount = reserve.price + reserve.delivery_service,
                                date = DateTime.Now,
                                note = "",
                            };
                            message = Transaction.Insert(transaction);
                        }
                    }
                }
            }
            return message;
        }
    }
}
