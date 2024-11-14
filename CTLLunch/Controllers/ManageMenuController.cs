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
    public class ManageMenuController : Controller
    {
        private IEmployee Employee;
        private IShop Shop;
        private IMenu Menu;
        private ICategory Category;
        private IGroup Group;
        private IIngredients Ingredients;
        private IPlanOutOfIngredients PlanOutOfIngredients;
        public ManageMenuController(IEmployee _Employee, IShop _Shop, IMenu _Menu, ICategory _Category, IGroup _Group, IIngredients _Ingredients, IPlanOutOfIngredients _PlanOutOfIngredients)
        {
            Employee = _Employee;
            Shop = _Shop;
            Menu = _Menu;
            Category = _Category;
            Group = _Group;
            Ingredients = _Ingredients;
            PlanOutOfIngredients = _PlanOutOfIngredients;
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

                List<ShopModel> shops = await Shop.GetShops();
                ViewBag.shops = shops;
                return View(employee);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDataShops(string shop_id)
        {
            List<CategoryMenuModel> categories = await Category.GetCategories();
            List<GroupMenuModel> groups = await Group.GetGroups();
            List<IngredientsMenuModel> ingredients = await Ingredients.GetIngredients();

            var data = new { categories =  categories , groups = groups,ingredients = ingredients};
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetMenus(string shop_id)
        {
            List<MenuModel> menus = await Menu.GetMenuByShop(shop_id);
            return Json(menus);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlanMenus(string shop_id)
        {
            List<PlanOutOfIngredientsModel> plans = await PlanOutOfIngredients.GetPlanOutOfIngredientsByShop(shop_id);
            return Json(plans);
        }

        [HttpPost]
        public async Task<string> InsertMenu(string str)
        {
            MenuModel menu = JsonConvert.DeserializeObject<MenuModel>(str);
            string lastID = await Menu.GetLastID();
            lastID = "M" + (Int32.Parse(lastID.Substring(1, 4)) + 1).ToString().PadLeft(4, '0');
            menu.menu_id = lastID;
            menu.menu_pic = new byte[0];
            string message = await Menu.Insert(menu);
            return message;

        }

        [HttpPost]
        public async Task<string> InsertPlanMenu(string str)
        {
            PlanOutOfIngredientsModel plan = JsonConvert.DeserializeObject<PlanOutOfIngredientsModel>(str);
            string message = await PlanOutOfIngredients.Insert(plan);
            return message;
        }

        [HttpDelete]
        public async Task<string> DeletePlanById(string id)
        {
            string message = await PlanOutOfIngredients.DeleteById(id);
            return message;
        }

        [HttpDelete]
        public async Task<string> DeletePlanByShop(string shop_id)
        {
            string message = await PlanOutOfIngredients.DeleteByShop(shop_id);
            return message;
        }

        [HttpDelete]
        public async Task<string> DeleteMenu(string menu_id)
        {
            string message = await Menu.Delete(menu_id);
            return message;
        }

        [HttpPut]
        public async Task<string> UpdateMenu(string str)
        {
            MenuModel menu = JsonConvert.DeserializeObject<MenuModel>(str);
            string message = await Menu.Update(menu);
            return message;
        }
    }
}
