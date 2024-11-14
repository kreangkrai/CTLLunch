using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface ICategory
    {
        Task<List<CategoryMenuModel>> GetCategories();
        Task<string> GetLastID();
        Task<string> Insert(CategoryMenuModel category);
        Task<string> Update(CategoryMenuModel category);
        Task<string> Delete(string category_id);
    }
}
