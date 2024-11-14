using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace CTLLunch.Service
{
    public class PlanCloseShopService : IPlanCloseShop
    {
        private IConnectAPI API;
        private readonly string URL;
        public PlanCloseShopService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> Delete(string shop_id, DateTime date)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"PlanCloseShop/delete/{shop_id}/{date.ToString("yyyy-MM-dd")}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        public async Task<List<PlanCloseShopModel>> GetPlanCloseShops()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"PlanCloseShop/getplancloseshops");
            var content = await response.Content.ReadAsStringAsync();
            List<PlanCloseShopModel> planCloseShops = JsonConvert.DeserializeObject<List<PlanCloseShopModel>>(content);
            return planCloseShops;
        }

        public async Task<List<PlanCloseShopModel>> GetPlanCloseShopsByDate(DateTime now)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"PlanCloseShop/getplancloseshopsbydate/{now.ToString("yyyy-MM-dd")}");
            var content = await response.Content.ReadAsStringAsync();
            List<PlanCloseShopModel> planCloseShops = JsonConvert.DeserializeObject<List<PlanCloseShopModel>>(content);
            return planCloseShops;
        }

        public async Task<string> Insert(PlanCloseShopModel plan)
        {
            var json = JsonConvert.SerializeObject(plan);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "PlanCloseShop/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
