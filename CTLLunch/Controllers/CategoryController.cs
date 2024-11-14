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
        public async Task<IActionResult> GetCategories()
        {
            List<CategoryMenuModel> categories = await Category.GetCategories();
            return Json(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            List<GroupMenuModel> groups = await Group.GetGroups();
            return Json(groups);
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredients()
        {
            List<IngredientsMenuModel> ingredients = await Ingredients.GetIngredients();
            return Json(ingredients);
        }

        [HttpPost]
        public async Task<string> InsertCategory(string category)
        {           
            string lastID = await Category.GetLastID();
            lastID = "C" + (Int32.Parse(lastID.Substring(1,2))+1).ToString().PadLeft(2,'0');
            CategoryMenuModel _category = new CategoryMenuModel()
            {
                category_id = lastID,
                category_name = category,
            };
            string message = await Category.Insert(_category);
            return message;
        }

        [HttpPost]
        public async Task<string> InsertGroup(string group)
        {
            string lastID = await Group.GetLastID();
            lastID = "G" + (Int32.Parse(lastID.Substring(1, 2)) + 1).ToString().PadLeft(2, '0');
            GroupMenuModel _group = new GroupMenuModel()
            {
                group_id = lastID,
                group_name = group,
            };
            string message = await Group.Insert(_group);
            return message;
        }

        [HttpPost]
        public async Task<string> InsertIngredients(string ingredients)
        {
            string lastID = await Ingredients.GetLastID();
            lastID = "I" + (Int32.Parse(lastID.Substring(1, 2)) + 1).ToString().PadLeft(2, '0');
            IngredientsMenuModel _ingredients = new IngredientsMenuModel()
            {
                ingredients_id = lastID,
                ingredients_name = ingredients,
            };
            string message = await Ingredients.Insert(_ingredients);
            return message;
        }

        [HttpPut]
        public async Task<string> UpdateCategory(string category_id ,string category_name)
        {
            CategoryMenuModel category = new CategoryMenuModel()
            {
                category_id = category_id,
                category_name = category_name
            };
            string message = await Category .Update(category);
            return message;
        }

        [HttpPut]
        public async Task<string> UpdateGroup(string group_id, string group_name)
        {
            GroupMenuModel group = new GroupMenuModel()
            {
                group_id = group_id,
                group_name = group_name
            };
            string message = await Group.Update(group);
            return message;
        }

        [HttpPut]
        public async Task<string> UpdateIngredients(string ingredients_id, string ingredients_name)
        {
            IngredientsMenuModel ingredients = new IngredientsMenuModel()
            {
                ingredients_id = ingredients_id,
                ingredients_name = ingredients_name
            };
            string message = await Ingredients.Update(ingredients);
            return message;
        }
    }
}
