using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IShop
    {
        Task<List<ShopModel>> GetShops();
        Task<string> GetLastID();
        Task<string> Insert(ShopModel shop);
        Task<string> Update(ShopModel shop);
        Task<string> Delete(string shop_id);
        Task<string> UpdateCloseTimeShift(string shop_id);
    }
}
