using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class ShopModel
    {
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public string phone { get; set; }
        public string bank_account { get; set; }
        public byte[] qr_code { get; set; }
        public TimeSpan open_time { get; set; }
        public TimeSpan close_time { get; set; }
        public int limit_order { get; set; }
        public int delivery_service { get; set; }
    }
}
