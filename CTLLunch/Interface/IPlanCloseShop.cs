using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IPlanCloseShop
    {
        Task<List<PlanCloseShopModel>> GetPlanCloseShops();
        Task<List<PlanCloseShopModel>> GetPlanCloseShopsByDate(DateTime now);
        Task<string> Insert(PlanCloseShopModel plan);
        Task<string> Delete(string shop_id,DateTime date);
    }
}
