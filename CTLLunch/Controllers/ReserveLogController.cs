﻿using System;
using System.Linq;
using CTLLunch.Interface;
using CTLLunch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CTLLunch.Controllers
{
    public class ReserveLogController : Controller
    {
        private IEmployee Employee;
        private IReserve Reserve;
        private IShop Shop;
        private IPlanCloseShop PlanCloseShop;
        private IMenu Menu;
        private IPlanOutOfIngredients PlanOutOfIngredients;
        public ReserveLogController(IEmployee _Employee, IReserve _Reserve, IShop _Shop, IPlanCloseShop _PlanCloseShop, IMenu _Menu, IPlanOutOfIngredients _PlanOutOfIngredients)
        {
            Employee = _Employee;
            Reserve = _Reserve;
            Shop = _Shop;
            PlanCloseShop = _PlanCloseShop;
            Menu = _Menu;
            PlanOutOfIngredients = _PlanOutOfIngredients;
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
        public IActionResult GetShop(DateTime date)
        {
            List<ShopModel> shops = Shop.GetShops();
            List<ReserveModel> reserves = Reserve.GetReserveByDate(date);
            List<string> _shops = reserves.GroupBy(g=>g.shop_name).Select(s=>s.Key).ToList();
            List<ShopModel> new_shop = new List<ShopModel>();

            for(int i = 0; i < _shops.Count; i++)
            {
                ShopModel shop = shops.Where(w=>w.shop_name == _shops[i]).FirstOrDefault();
                new_shop.Add(shop);
            }
            var data = new { shops = new_shop };
            return Json(data);
        }
        [HttpGet]
        public IActionResult GetReserveLog(DateTime date ,string shop_id)
        {
            List<ReserveModel> reserves_shop = Reserve.GetReserveByShopDate(shop_id, date).ToList(); ;
            List<ReserveModel> reserves_all = Reserve.GetReserveByDate(date);
            List<PlanOutOfIngredientsModel> plans = PlanOutOfIngredients.GetPlanOutOfIngredientsByDate(date);
            EmployeeModel employee_ctl = Employee.GetEmployeeCTL();

            List<MenuModel> menus = Menu.GetMenuByShop(shop_id);
            List<MenuModel> _menus = new List<MenuModel>();
            for (int i = 0; i < menus.Count; i++)
            {
                if (!plans.Any(a => a.ingredients_id == menus[i].ingredients_id && a.shop_id == menus[i].shop_id))
                {
                    _menus.Add(menus[i]);
                }
            }

            List<DeliveryServiceModel> shops = reserves_all.GroupBy(g => g.shop_id).Select(s => new DeliveryServiceModel(){
                shop_id = s.Key,
                delivery_service = Shop.GetShops().Where(w => w.shop_id == s.Key).Select(s1 => s1.delivery_service).FirstOrDefault(),
                count_reserve = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status != "Cancel").Count(),
                delivery_service_per_person = reserves_all.Where(w => w.shop_id == s.Key && w.category_id != "C99" && w.status == "Approved").Select(s1 => s1.delivery_service_per_person).FirstOrDefault(),
            }).ToList();

            for (int i = 0; i < shops.Count; i++)
            {
                // Re-Calculate Delivery Service
                int delivery_service = shops[i].delivery_service;
                int count_reserve = shops[i].count_reserve;
                if (shops[i].delivery_service_per_person == 0)
                {
                    AmountDeliveryBalanceModel amount = Reserve.ComputeAmountDeliveryBalance(delivery_service, count_reserve, employee_ctl.balance);
                    shops[i].delivery_service_per_person = amount.delivery_service;
                }
            }

            for (int i = 0; i < reserves_shop.Count; i++)
            {
                int delivery_serveice_per_person = shops.Where(w => w.shop_id == reserves_shop[i].shop_id).Select(s => s.delivery_service_per_person).FirstOrDefault();
                reserves_shop[i].delivery_service_per_person = delivery_serveice_per_person;
            }

            reserves_shop = reserves_shop.GroupBy(g => g.reserve_id).Select(s => new ReserveModel()
            {
                reserve_id = s.Key,
                employee_id = s.FirstOrDefault().employee_id,
                employee_name = s.FirstOrDefault().employee_name,
                employee_nickname = s.FirstOrDefault().employee_nickname,
                shop_id = s.FirstOrDefault().shop_id,
                shop_name = s.FirstOrDefault().shop_name,
                menu_id = s.FirstOrDefault().menu_id,
                menu_name = string.Join('+', s.Select(f => f.menu_name).ToArray()),
                category_id = s.FirstOrDefault().category_id,
                group_id = s.FirstOrDefault().group_id,
                amount_order = s.FirstOrDefault().amount_order,
                extra = s.FirstOrDefault().extra,
                note = string.Join(' ', s.Select(f => f.note).ToArray()),
                remark = string.Join(' ', s.Select(f => f.remark).ToArray()),
                review = s.FirstOrDefault().review,
                reserve_date = s.FirstOrDefault().reserve_date,
                status = s.FirstOrDefault().status,
                price = s.Sum(f => f.price),
                delivery_service = s.FirstOrDefault().delivery_service,
                delivery_service_per_person = s.FirstOrDefault().delivery_service_per_person,
                sum_price = s.Sum(f => f.price) + s.FirstOrDefault().delivery_service_per_person,
            }).ToList();


            var data = new { reserves_shop = reserves_shop, menus = _menus };
            return Json(data);
        }
    }
}
