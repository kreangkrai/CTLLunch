using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IIngredients
    {
        List<IngredientsMenuModel> GetIngredients();
        string GetLastID();
        string Insert(IngredientsMenuModel ingredients);
        string Update(IngredientsMenuModel ingredients);
        string Delete(string ingredients_id);
    }
}
