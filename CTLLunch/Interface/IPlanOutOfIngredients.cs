using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IPlanOutOfIngredients
    {
        List<PlanOutOfIngredientsModel> GetPlanOutOfIngredients();
        List<PlanOutOfIngredientsModel> GetPlanOutOfIngredientsByDate(DateTime now);
        List<PlanOutOfIngredientsModel> GetPlanOutOfIngredientsByShop(string shop_id);
        string Insert(PlanOutOfIngredientsModel plan);
        string DeleteById(string id);
        string DeleteByShop(string shop_id);
    }
}
