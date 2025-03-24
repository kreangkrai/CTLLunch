using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class MailDataModel
    {
        public string admin { get; set; }
        public string topup { get; set; }
        public string transfer { get; set; }
        public string receiver { get; set; }
        public string payer { get; set; }
        public int amount { get; set; }
        public int balance { get; set; }
        public string url { get; set; }
        public DateTime date { get; set; }
    }
}
