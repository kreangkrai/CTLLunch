using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class TransactionModel
    {
        public int id { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string receiver_id { get; set; }
        public string receiver_name { get; set; }
        public string type { get; set; }
        public double amount { get; set; }
        public DateTime date { get; set; }
        public string note { get; set; }
    }
}
