using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CTLLunch.Controllers
{
    public class BalanceController : Controller
    {
        private IEmployee Employee;
        private ITransaction Transaction;
        public BalanceController(IEmployee _Employee, ITransaction _Transaction)
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
                employees = Employee.GetEmployees().Where(w=>w.status == true).ToList();
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

                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetBalances()
        {
            List<EmployeeModel> employees = Employee.GetEmployees().Where(w=>w.status == true).ToList();
            employees = employees.OrderBy(o=>o.employee_name).ToList();
            return Json(employees);
        }

        [HttpGet]
        public IActionResult GetTransaction(string month)
        {
            DateTime date = Convert.ToDateTime(month).AddDays(-1);
            double balance = 0;
            List<TransactionModel> all_transactions = Transaction.GetTransactions();
            all_transactions = all_transactions.Where(w=>w.date <= date).ToList();
            for(int i = 0; i < all_transactions.Count; i++)
            {
                if (all_transactions[i].type == "Add")
                {
                    balance += all_transactions[i].amount;
                }
                if (all_transactions[i].type == "Pay")
                {
                    balance -= all_transactions[i].amount;
                }
                if (all_transactions[i].type == "Close")
                {
                    balance -= all_transactions[i].amount;
                }
            }

            List<TransactionModel> transactions = Transaction.GetTransactionByMonth(month);
            var data = new {transactions = transactions,balance = balance};
            return Json(data);
        }

        [HttpPost]
        public string WithdrawBalances(string employee_id , int amount)
        {
            EmployeeModel employee = Employee.GetEmployees().Where(w=>w.employee_id == employee_id).FirstOrDefault();
            int old_balance = employee.balance;
            if (employee.balance >= amount) 
            {
                employee.balance = old_balance - amount;
                employee.status = false;
                string message = Employee.UpdateBalance(employee);
                if (message == "Success")
                {
                    string user = HttpContext.Session.GetString("userId");
                    EmployeeModel _receiver = Employee.GetEmployees().Where(w => w.employee_name.ToLower() == user.ToLower()).FirstOrDefault();
                    string receiver_id = _receiver.employee_id;
                    TransactionModel transaction = new TransactionModel()
                    {
                        employee_id = employee_id,
                        amount = amount,
                        type = "Close",
                        date = DateTime.Now,
                        receiver_id = receiver_id,
                        note = ""
                    };
                    message = Transaction.Insert(transaction);
                    if (message == "Success")
                    {
                        message = Employee.UpdateStatus(employee);
                    }
                    return message;
                }
                else
                {
                    return "ผิดพลาด";
                }
            }
            else
            {
                return "ถอนเกินยอดเงินที่มีอยู่";
            }
        }
    }
}
