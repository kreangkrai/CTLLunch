using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Controllers
{
    public class ShopManageController : Controller
    {
        private IEmployee Employee;
        private IShop Shop;
        public ShopManageController(IEmployee _Employee, IShop _Shop)
        {
            Employee = _Employee;
            Shop = _Shop;
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
    }
}
