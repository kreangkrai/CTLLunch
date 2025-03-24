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
using System.Net.Http;
using System.Net;

namespace CTLLunch.Controllers
{
    public class HomeController : Controller
    {
        private IEmployee Employee;
        private ITransaction Transaction;
        private ITopup Topup;
        private IReserve Resereve;
        private IMail Mail;
        private readonly IHostingEnvironment hostingEnvironment;       
        public HomeController(ITopup _Topup,IEmployee _Employee, ITransaction _Transaction, IReserve _Resereve, IMail _Mail, IHostingEnvironment _hostingEnvironment)
        {
            Employee = _Employee;
            Transaction = _Transaction;
            Topup = _Topup;
            Resereve = _Resereve;
            Mail = _Mail;
            hostingEnvironment = _hostingEnvironment;
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
                    balance = s.balance,
                    status = s.status,
                    notify = s.notify
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
      
        public async Task<string> InsertBalance(string employee_id,int amount)
        {
            string user = HttpContext.Session.GetString("userId");
            List<EmployeeModel> employees = await Employee.GetEmployees();
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
            string message = await Transaction.Insert(transaction);
            if (message == "Success")
            {
                // Update Balance
                List<EmployeeModel> _employees = await Employee.GetEmployees();
                EmployeeModel employee = _employees.Where(w => w.employee_id == employee_id).FirstOrDefault();
                int old_balance = employee.balance;
                int new_balance = old_balance + amount;
                employee.balance = new_balance;
                message = await Employee.UpdateBalance(employee);
            }
            return message;
        }

        [HttpPost]
        public async Task<string> InsertTopup(string employee_id, int amount,string topup_id)
        {
            string user = HttpContext.Session.GetString("userId");
            DateTime date = DateTime.Now;
            TopupModel topup = new TopupModel()
            {
                employee_id = employee_id,
                receiver_id = "",
                amount = amount,
                date = date,
                topup_id = topup_id,
                note = "",
                status = "Pending"
            };
            string message = await Topup.Insert(topup);
            if (message == "Success")
            {
                List<EmployeeModel> employees = await Employee.GetEmployees();
                EmployeeModel employee = employees.Where(w => w.employee_id == employee_id).FirstOrDefault();

                if (employee.notify == true)
                {
                    //User
                    //topup, amount, date
                    MailDataModel data_mail_topup = new MailDataModel()
                    {
                        topup = employee.email,
                        amount = amount,
                        date = date
                    };

                    string message_topup = await Mail.SendEmailTopup(data_mail_topup);
                    string admin = "sarit_t@contrologic.co.th";
                    //Admin
                    //admin, topup, amount, url, date
                    string folderName = "backup/topup/" + topup_id;
                    string webRootPath = hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    DirectoryInfo di = new DirectoryInfo(newPath);
                    FileInfo[] Images = di.GetFiles("*.*");
                    string fullpath = folderName + "/" + Images[0].Name;
                    string scheme = Request.Scheme;
                    string host = Request.Host.Host;
                    //string url = scheme + "://" + host + "/lunch/" + fullpath;
                    string url = scheme + "://" + host + ":44316/" + fullpath;

                    MailDataModel data_mail_admin_topup = new MailDataModel()
                    {
                        admin = admin,
                        topup = employee.email,
                        amount = amount,
                        url = url,
                        date = date
                    };

                    if (message_topup == "Success")
                    {
                        message_topup = await Mail.SendEmailAdminTopup(data_mail_admin_topup);
                    }
                }
            }
            return message;
        }

        [HttpPost]
        public async Task<string> TransferBalance(string employee_id_from,string employee_id_to,int amount)
        {          
            List<EmployeeModel> employees = await Employee.GetEmployees();
            EmployeeModel employee_from = employees.Where(w=>w.employee_id == employee_id_from).FirstOrDefault();
            EmployeeModel employee_to = employees.Where(w => w.employee_id == employee_id_to).FirstOrDefault();
            List<ReserveModel> reserves = await Resereve.GetReserves();
            int amount_reserve = reserves.Where(w => w.employee_id == employee_id_from && w.status == "Pending").Sum(s => s.price);
            string message = "";

            if (amount <= (employee_from.balance - amount_reserve) && amount > 0 && employee_id_from != employee_id_to)
            {
                int old_balance_from = employee_from.balance;
                int new_balance_from = employee_from.balance-amount;
                employee_from.balance = new_balance_from;
                message = await Employee.UpdateBalance(employee_from);
                if (message == "Success")
                {
                    // Insert Transaction Employee From
                    TransactionModel transaction_from = new TransactionModel()
                    {
                        employee_id = employee_id_from,
                        receiver_id = employee_id_to,
                        amount = amount,
                        date = DateTime.Now,
                        type = "Transfer",
                        note = ""
                    };
                    message = await Transaction.Insert(transaction_from);
                    if (message == "Success")
                    {
                        int old_balance_to = employee_to.balance;
                        int new_balance_to = employee_to.balance + amount;
                        employee_to.balance = new_balance_to;
                        message = await Employee.UpdateBalance(employee_to);
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
                            message = await Transaction.Insert(transaction_to);

                            if (message == "Success")
                            {
                                string send_mail_transfer = "";
                                string _transfer = employee_from.email;
                                string _receiver = employee_to.email;
                                if (employee_from.notify == true)
                                {                                    
                                    // transfer, receiver, amount, balance, date
                                    MailDataModel data_mail_transfer = new MailDataModel()
                                    {
                                        transfer = _transfer,
                                        receiver = _receiver,
                                        amount = transaction_from.amount,
                                        balance = employee_from.balance,
                                        date = transaction_from.date
                                    };
                                    send_mail_transfer = await Mail.SendEmailTransfer(data_mail_transfer);
                                }

                                if (employee_to.notify == true)
                                {
                                    if (send_mail_transfer == "Success")
                                    {
                                        // transfer, receiver, amount, balance, date
                                        MailDataModel data_mail_receiver = new MailDataModel()
                                        {
                                            transfer = _transfer,
                                            receiver = _receiver,
                                            amount = transaction_to.amount,
                                            balance = employee_to.balance,
                                            date = transaction_to.date
                                        };
                                        string send_mail_receiver = await Mail.SendEmailReceiver(data_mail_receiver);
                                    }
                                }
                            }
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
        public async Task<IActionResult> GetTransactionAdd(string employee_id)
        {
            List<TransactionModel> transactions = await Transaction.GetTransactionByEmployee(employee_id);
            transactions = transactions.Where(w => w.type == "Add").ToList();
            transactions = transactions.OrderBy(o => o.date).ToList();
            return Json(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionTransfer(string employee_id)
        {
            List<TransactionModel> transactions = await Transaction.GetTransactionByEmployee(employee_id);
            transactions = transactions.Where(w => w.type == "Transfer" || w.type == "Receive").ToList();
            transactions = transactions.OrderBy(o => o.date).ToList();
            return Json(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionPay(string employee_id)
        {
            List<TransactionModel> transactions = await Transaction.GetTransactionByEmployee(employee_id);
            transactions = transactions.Where(w => w.type == "Pay").ToList();
            transactions = transactions.OrderBy(o => o.date).ToList();
            return Json(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionByEmployee(string employee_id)
        {
            double balance = 0;
            List<TransactionModel> transactions = await Transaction.GetTransactionByEmployee(employee_id);
            var data = new { transactions = transactions, balance = balance };
            return Json(data);
        }

        [HttpPut]
        public async Task<string> UpdateStatusTopup(string str)
        {
            TopupModel topup = JsonConvert.DeserializeObject<TopupModel>(str);
            string message = await Topup.UpdateStatus(topup);
            return message;
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminStatusTopup()
        {
            List<TopupModel> topups = await Topup.GetTopups();
            topups = topups.Where(w => w.status == "Pending").ToList();
            topups = topups.OrderByDescending(o => o.date).ToList();
            return Json(topups);
        }
        [HttpGet]
        public async Task<IActionResult> GetStatusTopup(string employee_id)
        {
            List<TopupModel> topups = await Topup.GetTopupByEmployee(employee_id);
            topups = topups.OrderByDescending(o => o.date).ToList();
            return Json(topups);
        }

        [HttpPost]
        public async Task<string> ConfirmTopup(string str)
        {
            string message = "";
            TopupModel topup = JsonConvert.DeserializeObject<TopupModel>(str);
            topup.date = DateTime.Now;
            if (topup.status == "Approve")
            {
                message = await InsertBalance(topup.employee_id, topup.amount);
                if (message == "Success")
                {
                    message = await Topup.UpdateStatus(topup);

                    if (message == "Success")
                    {
                        //topup, amount, date
                        List<EmployeeModel> employees = await Employee.GetEmployees();
                        EmployeeModel employee = employees.Where(w => w.employee_id == topup.employee_id).FirstOrDefault();

                        if (employee.notify == true)
                        {
                            //User
                            //topup, amount, date
                            MailDataModel data_mail_approve = new MailDataModel()
                            {
                                topup = employee.email,
                                amount = topup.amount,
                                date = topup.date
                            };

                            string message_approve = await Mail.SendEmailApproveTopup(data_mail_approve);
                        }
                    }
                }
            }
            if (topup.status == "Cancel")
            {
                message = await Topup.UpdateStatus(topup);

                if (message == "Success")
                {
                    //topup, amount, date
                    List<EmployeeModel> employees = await Employee.GetEmployees();
                    EmployeeModel employee = employees.Where(w => w.employee_id == topup.employee_id).FirstOrDefault();

                    if (employee.notify == true)
                    {
                        //User
                        //topup, amount, date
                        MailDataModel data_mail_approve = new MailDataModel()
                        {
                            topup = employee.email,
                            amount = topup.amount,
                            date = topup.date
                        };

                        string message_approve = await Mail.SendEmailCancelTopup(data_mail_approve);
                    }
                }
            }
            return message;
        }

        [HttpPost]
        public IActionResult ImportFile(string topup_id)
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "backup/topup/" + topup_id;
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
        public IActionResult ReadFile(string topup_id)
        {
            try
            {
                string folderName = "backup/topup/" + topup_id;
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

        [HttpGet]
        public async Task<IActionResult> GetPathImageTopup(string topup_id)
        {
            try
            {
                string folderName = "backup/topup/" + topup_id;
                string webRootPath = hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                DirectoryInfo di = new DirectoryInfo(newPath);
                FileInfo[] Images = di.GetFiles("*.*");
                string fullpath = folderName + "/" + Images[0].Name;
                string scheme = Request.Scheme;
                string host = Request.Host.Host;
                //string _path = scheme +"://" + host + "/lunch/" + fullpath;
                string _path = scheme + "://" + host + ":44316/" + fullpath;
                string base64 = await GetImageAsBase64Url(_path);
                return Json(base64);
            }
            catch
            {
                return Json("ไม่มีสลิป");
            }
        }
        public async static Task<string> GetImageAsBase64Url(string url)
        {
            var credentials = new NetworkCredential();
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var client = new HttpClient(handler))
            {
                var bytes = await client.GetByteArrayAsync(url);
                return "image/jpeg;base64," + Convert.ToBase64String(bytes);
            }
        }

        [HttpPut]
        public async Task<string> UpdateNotify(string str)
        {
            EmployeeModel employee = JsonConvert.DeserializeObject<EmployeeModel>(str);
            string message = await Employee.UpdateNotify(employee);
            return message;
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
