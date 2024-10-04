using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CTLLunch.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployee Employee;
        public EmployeeController(IEmployee _Employee)
        {
              Employee = _Employee;
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

                HttpContext.Session.SetString("Name", employee.employee_name);
                HttpContext.Session.SetString("Department", employee.department);
                HttpContext.Session.SetString("Role", employee.role);

                List<UserModel> users = Employee.GetUserAD();
                ViewBag.Employees = users;
                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetEmployees()
        {
            List<EmployeeModel> employees = Employee.GetEmployees();
            return Json(employees);
        }

        [HttpPost]
        public string InsertEmployee(string str)
        {
            EmployeeModel employee = JsonConvert.DeserializeObject<EmployeeModel>(str);
            string lastID = Employee.GetLastEmployee();
            lastID = "EM" + (Int32.Parse(lastID.Substring(2, 3)) + 1).ToString().PadLeft(3, '0');
            employee.employee_id = lastID;
            employee.role = "";
            employee.balance = 0;
            string message = Employee.Insert(employee);
            return message;
        }

        [HttpPut]
        public string UpdateEmployee(string str) 
        {
            EmployeeModel employee = JsonConvert.DeserializeObject<EmployeeModel>(str);
            string message = Employee.UpdateRole(employee);
            return message;
        }

    }
}
