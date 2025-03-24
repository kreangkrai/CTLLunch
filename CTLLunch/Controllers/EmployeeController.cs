using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployee Employee;
        private IMail Mail;
        public EmployeeController(IEmployee _Employee, IMail _Mail)
        {
            Employee = _Employee;
            Mail = _Mail;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<EmployeeModel> employees = await Employee.GetEmployees();
                employees = employees.Where(w => w.status == true).ToList();
                EmployeeModel employee = employees.Where(w => w.employee_name.ToLower() == user.ToLower()).Select(s => new EmployeeModel()
                {
                    employee_id = s.employee_id,
                    employee_name = s.employee_name,
                    employee_nickname = s.employee_nickname,
                    department = s.department,
                    role = s.role,
                    balance = s.balance,
                    status = s.status,
                    notify = s.notify,
                }).FirstOrDefault();

                HttpContext.Session.SetString("Name", employee.employee_name);
                HttpContext.Session.SetString("Department", employee.department);
                HttpContext.Session.SetString("Role", employee.role);

                List<UserModel> users = await Employee.GetUserAD();
                users = users.Where(w=>w.active == true).ToList();

                List<EmployeeModel> _employees = await Employee.GetEmployees();
                _employees = _employees.Where(w=>w.status == true).ToList();

                users = users.Where(w => !_employees.Any(a => a.employee_name == w.name)).ToList();
                ViewBag.Employees = users;
                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            List<EmployeeModel> employees = await Employee.GetEmployees();
            employees = employees.Where(w => w.status == true).ToList();
            return Json(employees);
        }

        [HttpPost]
        public async Task<string> InsertEmployee(string str)
        {
            List<MailModel> mails = await Mail.GetEmailAddress();
            EmployeeModel employee = JsonConvert.DeserializeObject<EmployeeModel>(str);
            string lastID = await Employee.GetLastEmployee();
            lastID = "EM" + (Int32.Parse(lastID.Substring(2, 3)) + 1).ToString().PadLeft(3, '0');
            employee.employee_id = lastID;
            employee.role = "";
            employee.balance = 0;
            employee.status = true;
            employee.notify = true;
            employee.email = mails.Where(w => w.name.ToLower() == employee.employee_name.ToLower()).Select(s => s.email).FirstOrDefault();
            string message = await Employee.Insert(employee);
            return message;
        }

        [HttpPut]
        public async Task<string> UpdateEmployee(string str) 
        {
            EmployeeModel employee = JsonConvert.DeserializeObject<EmployeeModel>(str);
            string message = await Employee.UpdateRole(employee);
            return message;
        }

    }
}
