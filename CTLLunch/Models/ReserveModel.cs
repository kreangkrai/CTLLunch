﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Models
{
    public class ReserveModel
    {
        public string reserve_id { get; set; }
        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string employee_nickname { get; set; }
        public string shop_id { get; set; }
        public string shop_name { get; set; }
        public double delivery_service { get; set; }
        public double delivery_service_per_person { get; set; }
        public string menu_id { get; set; }
        public string menu_name { get; set; }
        public string category_id { get; set; }
        public string group_id { get; set; }
        public double price { get; set; }
        public double sum_price { get; set; }
        public int amount_order { get; set; }
        public bool extra { get; set; }
        public string note { get; set; }
        public DateTime reserve_date { get; set; }       
        public string remark { get; set; }
        public string status { get; set; }
        public int review { get; set; }
    }
}
