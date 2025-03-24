using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class EmployeeModel
    {
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string employee_nickname { get; set; }
        public string department { get; set; }
        public int balance { get; set; }
        public string role { get; set; }
        public bool status { get; set; }
        public bool notify { get; set; }
        public string email { get; set; }
    }
}
