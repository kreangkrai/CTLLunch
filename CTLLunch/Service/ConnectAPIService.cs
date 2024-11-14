using CTLLunch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class ConnectAPIService : IConnectAPI
    {
        public string ConnectAPI()
        {
            //return "https://localhost:44379/api/";
            return "http://ctracking.contrologic.co.th/lunchapi/api/";
        }
    }
}
