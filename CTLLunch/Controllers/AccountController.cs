using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Controllers
{
    public class AccountController : Controller
    {
        private IEmployee Employee;
        private IAuthen Authen;
        public AccountController(IEmployee _Employee, IAuthen _Authen)
        {
            Employee = _Employee;
            Authen = _Authen;
        }
        public IActionResult Index()
        {
            return View(new LoginModel());
        }
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.user == null)
                {
                    ModelState.AddModelError("Password", "Invalid login attempt.");
                    return View("Index");
                }
                else
                {
                    AuthenModel authen = Authen.ActiveDirectoryAuthenticate(model.user, model.password);
                    if (authen.authen)
                    {
                        List<EmployeeModel> employees = new List<EmployeeModel>();
                        employees = Employee.GetEmployees();

                        bool emp = employees.Any(a => a.employee_name.ToLower() == authen.user.ToLower());
                        if (emp)
                        {
                            HttpContext.Session.SetString("userId", authen.user);
                            HttpContext.Session.SetString("Department", authen.department);
                            HttpContext.Session.Set("Image", authen.image);
                            HttpContext.Session.SetString("Login_ENG", "1234");
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Not Registered!!!");
                            return View("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Invalid login attempt.");
                        return View("Index");
                    }
                }
            }
            else
            {
                return View("Login");
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Account");
        }
    }
}
