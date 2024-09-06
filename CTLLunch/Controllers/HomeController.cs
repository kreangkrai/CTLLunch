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
        public HomeController(IEmployee _Employee, IShop _Shop, IMenu _Menu)
        {
            Employee = _Employee;
            Shop = _Shop;
            Menu = _Menu;
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
                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
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
