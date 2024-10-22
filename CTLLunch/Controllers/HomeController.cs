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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CTLLunch.Controllers
{
    public class HomeController : Controller
    {
        private IEmployee Employee;
        private ITransaction Transaction;
        private readonly IHostingEnvironment hostingEnvironment;
        static string path = "";
        public HomeController(IEmployee _Employee, ITransaction _Transaction, IHostingEnvironment _hostingEnvironment)
        {
            Employee = _Employee;
            Transaction = _Transaction;
            hostingEnvironment = _hostingEnvironment;
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
                amount = amount,
                date = DateTime.Now,
                type = "Add",
                note = ""
            };
            string message = Transaction.Insert(transaction);
            if (message == "Success")
            {
                // Update Balance
                EmployeeModel employee = Employee.GetEmployees().Where(w => w.employee_id == employee_id).FirstOrDefault();
                int old_balance = employee.balance;
                int new_balance = old_balance + amount;
                employee.balance = new_balance;
                message = Employee.UpdateBalance(employee);
            }
            return message;
        }


        [HttpPost]
        public string TransferBalance(string employee_id_from,string employee_id_to,int amount)
        {
            List<EmployeeModel> employees = Employee.GetEmployees();
            EmployeeModel employee_from = employees.Where(w=>w.employee_id == employee_id_from).FirstOrDefault();
            EmployeeModel employee_to = employees.Where(w => w.employee_id == employee_id_to).FirstOrDefault();
            string message = "";

            if (amount <= employee_from.balance && amount > 0 && employee_id_from != employee_id_to)
            {
                int old_balance_from = employee_from.balance;
                int new_balance_from = employee_from.balance-amount;
                employee_from.balance = new_balance_from;
                message = Employee.UpdateBalance(employee_from);
                if (message == "Success")
                {
                    // Insert Transaction Employee Fromฉธศ
                    TransactionModel transaction_from = new TransactionModel()
                    {
                        employee_id = employee_id_from,
                        receiver_id = employee_id_to,
                        amount = amount,
                        date = DateTime.Now,
                        type = "Transfer",
                        note = ""
                    };
                    message = Transaction.Insert(transaction_from);
                    if (message == "Success")
                    {
                        int old_balance_to = employee_to.balance;
                        int new_balance_to = employee_to.balance + amount;
                        employee_to.balance = new_balance_to;
                        message = Employee.UpdateBalance(employee_to);
                        if (message == "Success")
                        {
                            // Insert Transaction Employee To
                            TransactionModel transaction_to = new TransactionModel()
                            {
                                employee_id = employee_id_to,
                                receiver_id = employee_id_from,
                                amount = amount,
                                date = DateTime.Now,
                                type = "Receive",
                                note = ""
                            };
                            message = Transaction.Insert(transaction_to);
                        }
                    }
                }
            }
            else
            {
                return "ยอดเงินที่โอนไม่เพียงพอ";
            }
            return message;
        }

        [HttpGet]
        public IActionResult GetTransactionAdd(string employee_id)
        {
            List<TransactionModel> transactions = Transaction.GetTransactionByEmployee(employee_id).Where(w => w.type == "Add").ToList();
            transactions = transactions.OrderBy(o => o.date).ToList();
            return Json(transactions);
        }

        [HttpGet]
        public IActionResult GetTransactionTransfer(string employee_id)
        {
            List<TransactionModel> transactions = Transaction.GetTransactionByEmployee(employee_id).Where(w => w.type == "Transfer" || w.type == "Receive").ToList();
            transactions = transactions.OrderBy(o => o.date).ToList();
            return Json(transactions);
        }

        [HttpGet]
        public IActionResult GetTransactionPay(string employee_id)
        {
            List<TransactionModel> transactions = Transaction.GetTransactionByEmployee(employee_id).Where(w => w.type == "Pay").ToList();
            transactions = transactions.OrderBy(o => o.date).ToList();
            return Json(transactions);
        }

        [HttpGet]
        public IActionResult GetTransactionByEmployee(string employee_id)
        {
            double balance = 0;
            List<TransactionModel> transactions = Transaction.GetTransactionByEmployee(employee_id).ToList();
            var data = new { transactions = transactions, balance = balance };
            return Json(data);
        }
        [HttpPost]
        public string InsertPath(DateTime date)
        {
            path = date.ToString("yyyyMMddHHmmss");
            return "Success";
        }

        [HttpPost]
        public IActionResult ImportFile()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "backup/add/" + path;
            string webRootPath = hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(newPath);
                foreach (FileInfo f in di.GetFiles())
                {
                    f.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {

                    dir.Delete(true);
                }
                Directory.CreateDirectory(newPath);

            }

            if (file.Length > 0)
            {
                string fullPath = Path.Combine(newPath, file.FileName);
                FileStream stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);

                stream.Position = 0;
                stream.Close();
            }
            return Json("Success");
        }

        [HttpGet]
        public IActionResult ReadFile(DateTime date)
        {
            try
            {
                path = date.ToString("yyyyMMddHHmmss");
                string folderName = "backup/add/" + path;
                string webRootPath = hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                DirectoryInfo di = new DirectoryInfo(newPath);
                FileInfo[] Images = di.GetFiles("*.*");
                string fullpath = folderName + "/" + Images[0].Name;
                return Json(fullpath);
            }
            catch
            {
                return Json("ไม่มีสลิป");
            }
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
