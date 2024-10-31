using System;

namespace CTLLunch.Models
{
    public class TopupModel
    {
        public string topup_id { get; set; }
        public DateTime date { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string receiver_id  { get; set; }
        public string receiver_name { get; set; }
        public int amount { get; set; }
        public string status { get; set; }
        public string note { get; set; }
    }
}
