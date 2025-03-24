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
    public class CentralMoneyController : Controller
    {
        private IEmployee Employee;
        private ITransaction Transaction;
        public CentralMoneyController(IEmployee _Employee, ITransaction _Transaction)
        {
            Employee = _Employee;
            Transaction = _Transaction;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("userId") != null)
            {
                string user = HttpContext.Session.GetString("userId");
                List<EmployeeModel> employees = await Employee.GetEmployees();
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

                    ViewBag.employees = employees.Where(w => w.employee_id != "EM999" && w.status == true).ToList();
                    ViewBag.employee = employee;
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

        [HttpGet]
        public async Task<IActionResult> GetCentralMoneyTransaction()
        {
            List<TransactionModel> transactions = await Transaction.GetCentralMoneyTransactions();
            transactions = transactions.OrderBy(o => o.date).ToList();
            return Json(transactions);
        }
    }
}
