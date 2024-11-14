using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IPlanOutOfIngredients
    {
        Task<List<PlanOutOfIngredientsModel>> GetPlanOutOfIngredients();
        Task<List<PlanOutOfIngredientsModel>> GetPlanOutOfIngredientsByDate(DateTime now);
        Task<List<PlanOutOfIngredientsModel>> GetPlanOutOfIngredientsByShop(string shop_id);
        Task<string> Insert(PlanOutOfIngredientsModel plan);
        Task<string> DeleteById(string id);
        Task<string> DeleteByShop(string shop_id);
    }
}
