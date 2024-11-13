using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface ICategory
    {
        List<CategoryMenuModel> GetCategories();
        Task<string> GetLastID();
        string Insert(CategoryMenuModel category);
        string Update(CategoryMenuModel category);
        string Delete(string category_id);
    }
}
