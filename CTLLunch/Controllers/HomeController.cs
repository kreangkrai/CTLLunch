using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Controllers
{
    public class HomeController : Controller
    {
        private IEmployee Employee;
        private IShop Shop;
        private IMenu Menu;
        private IReserve Reserve;
        public HomeController(IEmployee _Employee, IShop _Shop, IMenu _Menu, IReserve _Reserve)
        {
            Employee = _Employee;
            Shop = _Shop;
            Menu = _Menu;
            Reserve = _Reserve;
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

                List<ShopModel> shops = Shop.GetShops();
                ViewBag.shops = shops;

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
            List<ReserveModel> reserves_employee = Reserve.GetReserveByDateEmployee(DateTime.Now, employee_id);
            List<ReserveModel> reserves_all = Reserve.GetReserveByDate(DateTime.Now);
            List<MenuModel> menus = Menu.GetMenus();

            var shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new {
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Where(w=>w.shop_id == s.Key).Select(s1=>s1.delivery_service).FirstOrDefault(),
                amount_order = reserves_all.Where(w=>w.shop_id == s.Key && w.category_id != "C99").Count(),
                delivery_service_per_person = Shop.GetShops().Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault() / (double)reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99").Count()
            }).ToList();

            //Group reserve
            

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
                menu_name = string.Join(',', s.Select(f => f.menu_name).ToArray()),
                category_id = s.FirstOrDefault().category_id,
                group_id = s.FirstOrDefault().group_id,
                amount_order = s.FirstOrDefault().amount_order,
                extra = s.FirstOrDefault().extra,
                note = string.Join(',', s.Select(f => f.note).ToArray()),
                remark = string.Join(',', s.Select(f => f.remark).ToArray()),
                review = s.FirstOrDefault().review,
                reserve_date = s.FirstOrDefault().reserve_date,
                status = s.FirstOrDefault().status,
                price = s.Sum(f => f.price),
                delivery_service = s.FirstOrDefault().delivery_service,
                delivery_service_per_person = s.FirstOrDefault().delivery_service_per_person,
                sum_price = (double)s.Sum(f=>f.price) + (double)s.FirstOrDefault().delivery_service_per_person,
            }).ToList();
            return Json(reserves_employee);
        }
        [HttpGet]
        public JsonResult GetDataReserveShop(string shop_id)
        {
            List<ReserveModel> reserves_shop = Reserve.GetReserveByShopDate(shop_id, DateTime.Now);           
            return Json(reserves_shop);
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
