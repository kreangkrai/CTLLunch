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
        List<UserModel> GetUserAD();
        string GetLastEmployee();
        string Insert(EmployeeModel employee);
        string UpdateRole(EmployeeModel employee);
        string UpdateBalance(EmployeeModel employee);
        string UpdateStatus(EmployeeModel employee);
        EmployeeModel GetEmployeeCTL();
    }
}
