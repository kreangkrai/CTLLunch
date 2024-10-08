﻿using CTLLunch.Interface;
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
        string user = "";
        string dep = "";
        byte[] image = new byte[0];
        public AccountController(IEmployee _Employee)
        {
            Employee = _Employee;
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
                    bool check = ActiveDirectoryAuthenticate(model.user, model.password);
                    if (check)
                    {
                        List<EmployeeModel> employees = new List<EmployeeModel>();
                        employees = Employee.GetEmployees();

                        bool emp = employees.Any(a => a.employee_name.ToLower() == user.ToLower());
                        if (emp)
                        {
                            HttpContext.Session.SetString("userId", user);
                            HttpContext.Session.SetString("Department", dep);
                            HttpContext.Session.Set("Image", image);
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
        public bool ActiveDirectoryAuthenticate(string username, string password)
        {
            bool userOk = false;
            try
            {
                using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://192.168.15.1", username, password))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
                    {
                        searcher.Filter = "(samaccountname=" + username + ")";
                        searcher.PropertiesToLoad.Add("displayname");
                        searcher.PropertiesToLoad.Add("thumbnailPhoto");
                        searcher.PropertiesToLoad.Add("department");

                        SearchResult adsSearchResult = searcher.FindOne();

                        if (adsSearchResult != null)
                        {

                            var prop = adsSearchResult.Properties["thumbnailPhoto"];
                            if (adsSearchResult.Properties["displayname"].Count == 1)
                            {
                                user = (string)adsSearchResult.Properties["displayname"][0];
                                dep = (string)adsSearchResult.Properties["department"][0];
                                var img = adsSearchResult.Properties["thumbnailPhoto"].Count;
                                if (img > 0)
                                {
                                    image = adsSearchResult.Properties["thumbnailPhoto"][0] as byte[];
                                }
                            }
                            userOk = true;
                        }

                        return userOk;
                    }
                }
            }
            catch
            {
                return userOk;
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Account");
        }
    }
}
