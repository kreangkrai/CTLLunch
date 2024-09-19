using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class PlanCloseShopModel
    {
        public int id { get; set; }
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public DateTime date { get; set; }
    }
}
