using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class MenuModel
    {
        public string menu_id { get; set; }
        public string  group_id { get; set; }
        public string group_name { get; set; }
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public string ingredients_id { get; set; }
        public string ingredients_name { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }
        public string menu_name { get; set; }
        public double price { get; set; }
        public byte[] menu_pic { get; set; }
        public int extra_price { get; set; }
    }
}
