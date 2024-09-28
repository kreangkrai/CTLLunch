using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IPlanCloseShop
    {
        List<PlanCloseShopModel> GetPlanCloseShops(DateTime now);
        string Insert(PlanCloseShopModel plan);
        string Delete(string id);
    }
}
