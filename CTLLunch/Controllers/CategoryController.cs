using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CTLLunch.Controllers
{
    public class CategoryController : Controller
    {
        private IEmployee Employee;
        private ICategory Category;
        private IGroup Group;
        private IIngredients Ingredients;
        public CategoryController(IEmployee _Employee, ICategory _Category, IGroup _Group, IIngredients _Ingredients)
        {
            Employee = _Employee;
            Category = _Category;
            Group = _Group;
            Ingredients = _Ingredients;
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

                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            List<CategoryMenuModel> categories = Category.GetCategories();
            return Json(categories);
        }

        [HttpGet]
        public IActionResult GetGroups()
        {
            List<GroupMenuModel> groups = Group.GetGroups();
            return Json(groups);
        }

        [HttpGet]
        public IActionResult GetIngredients()
        {
            List<IngredientsMenuModel> ingredients = Ingredients.GetIngredients();
            return Json(ingredients);
        }

        [HttpPost]
        public string InsertCategory(string category)
        {           
            string lastID = Category.GetLastID();
            lastID = "C" + (Int32.Parse(lastID.Substring(1,2))+1).ToString().PadLeft(2,'0');
            CategoryMenuModel _category = new CategoryMenuModel()
            {
                category_id = lastID,
                category_name = category,
            };
            string message = Category.Insert(_category);
            return message;
        }

        [HttpPost]
        public string InsertGroup(string group)
        {
            string lastID = Group.GetLastID();
            lastID = "G" + (Int32.Parse(lastID.Substring(1, 2)) + 1).ToString().PadLeft(2, '0');
            GroupMenuModel _group = new GroupMenuModel()
            {
                group_id = lastID,
                group_name = group,
            };
            string message = Group.Insert(_group);
            return message;
        }

        [HttpPost]
        public string InsertIngredients(string ingredients)
        {
            string lastID = Ingredients.GetLastID();
            lastID = "I" + (Int32.Parse(lastID.Substring(1, 2)) + 1).ToString().PadLeft(2, '0');
            IngredientsMenuModel _ingredients = new IngredientsMenuModel()
            {
                ingredients_id = lastID,
                ingredients_name = ingredients,
            };
            string message = Ingredients.Insert(_ingredients);
            return message;
        }

        [HttpPut]
        public string UpdateCategory(string category_id ,string category_name)
        {
            CategoryMenuModel category = new CategoryMenuModel()
            {
                category_id = category_id,
                category_name = category_name
            };
            string message = Category.Update(category);
            return message;
        }

        [HttpPut]
        public string UpdateGroup(string group_id, string group_name)
        {
            GroupMenuModel group = new GroupMenuModel()
            {
                group_id = group_id,
                group_name = group_name
            };
            string message = Group.Update(group);
            return message;
        }

        [HttpPut]
        public string UpdateIngredients(string ingredients_id, string ingredients_name)
        {
            IngredientsMenuModel ingredients = new IngredientsMenuModel()
            {
                ingredients_id = ingredients_id,
                ingredients_name = ingredients_name
            };
            string message = Ingredients.Update(ingredients);
            return message;
        }
    }
}
