using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CTLLunch.Controllers
{
    public class ShopManageController : Controller
    {
        private IEmployee Employee;
        private IShop Shop;
        private IPlanCloseShop PlanCloseShop;
        public ShopManageController(IEmployee _Employee, IShop _Shop, IPlanCloseShop _PlanCloseShop)
        {
            Employee = _Employee;
            Shop = _Shop;
            PlanCloseShop = _PlanCloseShop;
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
        public IActionResult GetShops()
        {
            List<ShopModel> shops = Shop.GetShops();
            return Json(shops);
        }

        [HttpPost]
        public string InsertShop(string str)
        {
            ShopModel shop = JsonConvert.DeserializeObject<ShopModel>(str);
            string last_shop = Shop.GetLastID();
            last_shop = "S" + (Int32.Parse(last_shop.Substring(1, last_shop.Length)) + 1).ToString().PadLeft(2,'0');
            shop.shop_id = last_shop;

            string message = Shop.Insert(shop);
            return message;
        }

        [HttpDelete]
        public string DeleteShop(string shop_id)
        {
            string message = Shop.Delete(shop_id);
            return message;
        }

        [HttpGet]
        public IActionResult GetPlanCloseShops()
        {
            List<PlanCloseShopModel> plans = PlanCloseShop.GetPlanCloseShops();
            return Json(plans);
        }

        [HttpPost]
        public string InsertPlanCloseShop(string str)
        {
            PlanCloseShopModel plan = JsonConvert.DeserializeObject<PlanCloseShopModel>(str);
            string message = PlanCloseShop.Insert(plan);
            return message;
        }

        [HttpDelete]
        public string DeletePlanCloseShop(string id)
        {
            string message = PlanCloseShop.Delete(id);
            return message;
        }
    }
}
