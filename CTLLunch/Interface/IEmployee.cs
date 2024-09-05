using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IEmployee
    {
        List<EmployeeModel> GetEmployees();        
    }
}
