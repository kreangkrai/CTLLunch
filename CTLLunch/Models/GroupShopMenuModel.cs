using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class GroupShopMenuModel
    {
        public string group_id { get; set; }
        public string group_name { get; set; }
        public List<MenuModel> menus { get; set; }
    }
}
