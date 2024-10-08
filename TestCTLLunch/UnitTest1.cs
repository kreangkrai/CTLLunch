using CTLLunch.Interface;
using CTLLunch.Models;
using CTLLunch.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
namespace TestCTLLunch
{
    [TestClass]
    public class UnitTest1
    {
        private IEmployee Employee;

        [TestMethod]

        public void TestMethod()
        {
            Employee = new EmployeeService();
            List<EmployeeModel> employees = Employee.GetEmployees();

            Assert.IsNotNull(employees);
        }
    }
}
