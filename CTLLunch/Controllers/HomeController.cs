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
using System.Drawing;

namespace CTLLunch.Controllers
{
    public class HomeController : Controller
    {
        private IEmployee Employee;
        private ITransaction Transaction;
        public HomeController(IEmployee _Employee, ITransaction _Transaction)
        {
            Employee = _Employee;
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

                if (employee != null)
                {
                    HttpContext.Session.SetString("Name", employee.employee_name);
                    HttpContext.Session.SetString("Department", employee.department);
                    HttpContext.Session.SetString("Role", employee.role);
                    if (employee.role == "Admin")
                    {
                        ViewBag.employees = employees;
                    }
                    else
                    {
                        ViewBag.employees = employees.Where(w => w.employee_id == employee.employee_id).ToList();
                    }
                    return View(employee);
                }
                else
                {                   
                    return RedirectToAction("Index", "Account");
                }
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        public string InsertBalance(string employee_id,int amount)
        {
            string user = HttpContext.Session.GetString("userId");
            List<EmployeeModel> employees = new List<EmployeeModel>();
            employees = Employee.GetEmployees();
            string receiver_id = employees.Where(w => w.employee_name.ToLower() == user.ToLower()).FirstOrDefault().employee_id;
            TransactionModel transaction = new TransactionModel()
            {
                employee_id= employee_id,
                receiver_id = receiver_id,
                amount = (double)amount,
                date = DateTime.Now,
                type = "Add",
                note = ""
            };
            string message = Transaction.Insert(transaction);
            if (message == "Success")
            {
                // Update Balance
                EmployeeModel employee = Employee.GetEmployees().Where(w => w.employee_id == employee_id).FirstOrDefault();
                double old_balance = employee.balance;
                double new_balance = old_balance + (double)amount;
                employee.balance = new_balance;
                message = Employee.UpdateBalance(employee);
            }
            return message;
        }

        [HttpGet]
        public IActionResult GetTransaction(string employee_id)
        {
            string role = HttpContext.Session.GetString("Role");
            if (role == "Admin")
            {
                List<TransactionModel> transactions = Transaction.GetTransactions().Where(w=>w.type == "Add").ToList();
                transactions = transactions.OrderByDescending(o=>o.date).ToList();
                return Json(transactions);
            }
            else
            {
                List<TransactionModel> transactions = Transaction.GetTransactionByEmployee(employee_id).Where(w => w.type == "Add").ToList();
                transactions = transactions.OrderByDescending(o => o.date).ToList();
                return Json(transactions);
            }
        }

        [HttpGet]
        public IActionResult GetTransactionByEmployee(string employee_id)
        {
            double balance = 0;
            List<TransactionModel> transactions = Transaction.GetTransactionByEmployee(employee_id).ToList();
            var data = new { transactions = transactions, balance = balance };
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
