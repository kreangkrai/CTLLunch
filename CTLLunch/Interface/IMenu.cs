using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IMenu
    {
        List<MenuModel> GetMenus();
        List<MenuModel> GetMenuByShop(string shop_id);
        MenuModel GetMenuByMenu(string menu_id);
        List<MenuModel> SearchMenuByShop(string shop_id,string menu);
        string GetLastID();
        string Insert(MenuModel menu);
        string Update(MenuModel menu);
        string Delete(string menu_id);
    }
}
