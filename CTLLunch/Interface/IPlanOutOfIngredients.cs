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
        string Insert(PlanOutOfIngredientsModel plan);
        string Delete(string id);
    }
}
