using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IShop
    {
        List<ShopModel> GetShops();
        string GetLastID();
        string Insert(ShopModel shop);
        string Update(ShopModel shop);
        string Delete(string shop_id);
    }
}
