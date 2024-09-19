using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class PlanOutOfIngredientsModel
    {
        public int id { get; set; }
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public string ingredients_id { get; set; }
        public string ingredients_name { get; set; }
        public DateTime date { get; set; }
    }
}
