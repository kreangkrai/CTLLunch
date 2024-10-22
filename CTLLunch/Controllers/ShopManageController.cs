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
        public string GetLastID()
        {
            string id = Shop.GetLastID();
            id = "S" + (Int32.Parse(id.Substring(1, id.Length-1)) + 1).ToString().PadLeft(2, '0');
            return id;
        }

        [HttpGet]
        public IActionResult GetShops()
        {
            List<ShopModel> shops = Shop.GetShops();
            return Json(shops);
        }
        [HttpGet]
        public IActionResult GetShopByID(string shop_id)
        {
            ShopModel shop = Shop.GetShops().Where(w => w.shop_id == shop_id).FirstOrDefault();
            List<PlanCloseShopModel> plans = PlanCloseShop.GetPlanCloseShops().Where(w=>w.shop_id == shop_id).ToList();
            plans = plans.Where(w=>w.date.Date >= DateTime.Now.Date).ToList();
            var data = new { shop = shop, plans = plans };
            return Json(data);
        }

        [HttpPost]
        public string InsertShop(string str)
        {
            ShopModel shop = JsonConvert.DeserializeObject<ShopModel>(str);
            shop.open_time = new TimeSpan(9, 0, 0);
            shop.close_time = new TimeSpan(10, 0, 0);
            shop.close_time_shift = new TimeSpan(10, 0, 0);
            string message = Shop.Insert(shop);
            return message;
        }

        [HttpPut]
        public string UpdateShop(string str,string qr_code)
        {
            ShopModel shop = JsonConvert.DeserializeObject<ShopModel>(str);
            if (qr_code != null)
            {
                string _base64 = qr_code.Substring(qr_code.IndexOf(',') + 1);
                _base64 = _base64.Trim('\0');
                byte[] data = Convert.FromBase64String(_base64);
                shop.qr_code = data;
            }
            else
            {
                shop.qr_code = new byte[0];
            }
            string message = Shop.Update(shop);
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
            plans = plans.Where(w=>w.date.Date >= DateTime.Now.Date).ToList();
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
        public string DeletePlanCloseShop(string shop_id,DateTime date)
        {
            string message = PlanCloseShop.Delete(shop_id,date);
            return message;
        }
    }
}
