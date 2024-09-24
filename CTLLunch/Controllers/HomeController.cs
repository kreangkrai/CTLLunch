using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CTLLunch.Controllers
{
    public class HomeController : Controller
    {
        private IEmployee Employee;
        private IShop Shop;
        private IMenu Menu;
        private IReserve Reserve;
        private IPlanCloseShop PlanCloseShop;
        private IPlanOutOfIngredients PlanOutOfIngredients;
        public HomeController(IEmployee _Employee, IShop _Shop, IMenu _Menu, IReserve _Reserve, IPlanCloseShop _PlanCloseShop, IPlanOutOfIngredients _PlanOutOfIngredients)
        {
            Employee = _Employee;
            Shop = _Shop;
            Menu = _Menu;
            Reserve = _Reserve;
            PlanCloseShop = _PlanCloseShop;
            PlanOutOfIngredients = _PlanOutOfIngredients;
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

                List<PlanCloseShopModel> plan_close_shop = PlanCloseShop.GetPlanCloseShops(DateTime.Now);
                List<ShopModel> shops = Shop.GetShops();
                List<ShopModel> new_shops = new List<ShopModel>();
                for (int i = 0; i < shops.Count; i++)
                {
                    if (!plan_close_shop.Any(a=>a.shop_id == shops[i].shop_id)){
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
            List<ReserveModel> reserves_employee = Reserve.GetReserveByDateEmployee(DateTime.Now, employee_id).Where(w => w.status != "Cancel").ToList() ;
            List<ReserveModel> reserves_all = Reserve.GetReserveByDate(DateTime.Now);
            List<MenuModel> menus = Menu.GetMenus();

            var shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new {
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Where(w=>w.shop_id == s.Key).Select(s1=>s1.delivery_service).FirstOrDefault(),
                amount_order = reserves_all.Where(w=>w.shop_id == s.Key && w.category_id != "C99" &&  w.status != "Cancel").Count(),
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
                price =  s.Sum(f => f.price),
                delivery_service = s.FirstOrDefault().delivery_service,
                delivery_service_per_person = s.FirstOrDefault().delivery_service_per_person,
                sum_price = (double)s.Sum(f=>f.price) + (double)s.FirstOrDefault().delivery_service_per_person,
            }).ToList();

            var data = new { reserves_employee = reserves_employee, menus = menus };
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetDataReserveShop(string shop_id)
        {
            List<ReserveModel> reserves_shop = Reserve.GetReserveByShopDate(shop_id, DateTime.Now).Where(w => w.status != "Cancel").ToList(); ;           
            List<ReserveModel> reserves_all = Reserve.GetReserveByDate(DateTime.Now);
            List<PlanOutOfIngredientsModel> plans = PlanOutOfIngredients.GetPlanOutOfIngredients(DateTime.Now);

            List<MenuModel> menus = Menu.GetMenuByShop(shop_id);
            List<MenuModel> _menus = new List<MenuModel>();
            for(int i = 0; i < menus.Count; i++)
            {
                if (!plans.Any(a=>a.ingredients_id == menus[i].ingredients_id && a.shop_id == menus[i].shop_id))
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
                group_name = _menus.Where(w=>w.group_id == s.Key).FirstOrDefault().group_name,
                menus = _menus.Where(w=> w.group_id == s.Key).ToList()
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
                group_name = menus.Where(w=>w.group_id == s.Key).Select(x=>x.group_name).FirstOrDefault()
            }).ToList();
            var data = new { menu = menus, group = groupMenus };
            return Json(data);
        }

        [HttpGet]
        public JsonResult SearchMenuByShop(string shop_id,string menu)
        {
            List<PlanOutOfIngredientsModel> plans = PlanOutOfIngredients.GetPlanOutOfIngredients(DateTime.Now);

            List<MenuModel> menus = Menu.SearchMenuByShop(shop_id,menu);
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

        [HttpDelete]
        public string UpdateReserveStatus(string reserve_id)
        {
            string message = Reserve.UpdateStatus(reserve_id,"Cancel");
            return message;
        }
        [HttpPost]
        public string InsertReserve(List<string> strs)
        {
            string reserve_id = $"RES{DateTime.Now.ToString("ddMMyyyyHHmmssfff")}";
            DateTime date = DateTime.Now;
            string message = "";
            for (int i = 0; i < strs.Count; i++)
            {
                ReserveModel reserve = JsonConvert.DeserializeObject<ReserveModel>(strs[i]);
                MenuModel menu = Menu.GetMenuByMenu(reserve.menu_id).FirstOrDefault();
                reserve.reserve_id = reserve_id;
                reserve.amount_order = 1;
                reserve.category_id = menu.category_id;
                reserve.group_id = menu.group_id;
                reserve.reserve_date = date;
                reserve.status = "Pending";
                reserve.review = 0;
                message = Reserve.Insert(reserve);
            }
            return message;
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
