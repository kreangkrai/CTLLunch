using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IEmployee
    {
        Task<List<EmployeeModel>> GetEmployees();
        Task<List<UserModel>> GetUserAD();
        Task<string> GetLastEmployee();
        Task<string> Insert(EmployeeModel employee);
        Task<string> UpdateRole(EmployeeModel employee);
        Task<string> UpdateBalance(EmployeeModel employee);
        Task<string> UpdateStatus(EmployeeModel employee);
        Task<string> UpdateNotify(EmployeeModel employee);
        Task<EmployeeModel> GetEmployeeCTL();
    }
}
