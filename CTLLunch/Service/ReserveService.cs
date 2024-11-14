using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace CTLLunch.Service
{
    public class ReserveService : IReserve
    {
        private IConnectAPI API;
        private readonly string URL;
        public ReserveService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> UpdateStatus(string reserve_id, string status)
        {
            var client = new HttpClient();
            var response = await client.PutAsync(URL + $"Reserve/updatestatus/{reserve_id}/{status}", null);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<ReserveModel>> GetReserveByDate(DateTime date)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Reserve/getreservebydate/{date.ToString("yyyy-MM-dd")}");
            var content = await response.Content.ReadAsStringAsync();
            List<ReserveModel> reserves = JsonConvert.DeserializeObject<List<ReserveModel>>(content);
            return reserves;
        }

        public async Task<List<ReserveModel>> GetReserveByDateEmployee(DateTime date, string employee_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Reserve/getreservebydateemployee/{date.ToString("yyyy-MM-dd")}/{employee_id}");
            var content = await response.Content.ReadAsStringAsync();
            List<ReserveModel> reserves = JsonConvert.DeserializeObject<List<ReserveModel>>(content);
            return reserves;
        }

        public async Task<List<ReserveModel>> GetReserveByShopDate(string shop_id, DateTime date)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Reserve/getreservebyshopdate/{shop_id}/{date.ToString("yyyy-MM-dd")}");
            var content = await response.Content.ReadAsStringAsync();
            List<ReserveModel> reserves = JsonConvert.DeserializeObject<List<ReserveModel>>(content);
            return reserves;
        }

        public async Task<List<ReserveModel>> GetReserveByShopDateEmployee(string shop_id, DateTime date, string employee_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Reserve/getreservebyshopdateemployee/{shop_id}/{date.ToString("yyyy-MM-dd")}/{employee_id}");
            var content = await response.Content.ReadAsStringAsync();
            List<ReserveModel> reserves = JsonConvert.DeserializeObject<List<ReserveModel>>(content);
            return reserves;
        }

        public async Task<List<ReserveModel>> GetReserves()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Reserve/getreserves");
            var content = await response.Content.ReadAsStringAsync();
            List<ReserveModel> reserves = JsonConvert.DeserializeObject<List<ReserveModel>>(content);
            return reserves;
        }

        public async Task<string> Insert(ReserveModel reserve)
        {
            var json = JsonConvert.SerializeObject(reserve);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Reserve/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> UpdateReview(string reserve_id, int review)
        {
            var client = new HttpClient();
            var response = await client.PutAsync(URL + $"Reserve/updatereview/{reserve_id}/{review}", null);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> UpdateDelivery(ReserveModel reserve)
        {
            var json = JsonConvert.SerializeObject(reserve);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Reserve/updatedelivery", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<AmountDeliveryBalanceModel> ComputeAmountDeliveryBalance(int delivery_service, int count_reserve, int current_balance)
        {
            var client = new HttpClient();
            var response = await client.PostAsync(URL + $"Reserve/computeamountdeliverybalance/{delivery_service}/{count_reserve}/{current_balance}", null);
            var content = await response.Content.ReadAsStringAsync();
            AmountDeliveryBalanceModel amount = JsonConvert.DeserializeObject<AmountDeliveryBalanceModel>(content);
            return amount;
        }
    }
}
