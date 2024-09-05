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
        public double balance { get; set; }
        public string role { get; set; }
    }
}
