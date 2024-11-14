using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IMenu
    {
        Task<List<MenuModel>> GetMenus();
        Task<List<MenuModel>> GetMenuByShop(string shop_id);
        Task<MenuModel> GetMenuByMenu(string menu_id);
        Task<List<MenuModel>> SearchMenuByShop(string shop_id,string menu);
        Task<string> GetLastID();
        Task<string> Insert(MenuModel menu);
        Task<string> Update(MenuModel menu);
        Task<string> Delete(string menu_id);
    }
}
