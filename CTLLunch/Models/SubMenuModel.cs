using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class SubMenuModel
    {
        public string sub_menu_id { get; set; }
        public string shop_id { get; set; }
        public string sub_menu_name { get; set; }
        public double sub_price { get; set; }
        public byte[] sub_menu_pic { get; set; }
    }
}
