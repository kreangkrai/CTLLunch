using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IIngredients
    {
        Task<List<IngredientsMenuModel>> GetIngredients();
        Task<string> GetLastID();
        Task<string> Insert(IngredientsMenuModel ingredients);
        Task<string> Update(IngredientsMenuModel ingredients);
        Task<string> Delete(string ingredients_id);
    }
}
