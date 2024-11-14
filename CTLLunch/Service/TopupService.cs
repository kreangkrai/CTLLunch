using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace CTLLunch.Service
{
    public class TopupService : ITopup
    {
        private IConnectAPI API;
        private readonly string URL;
        public TopupService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<List<TopupModel>> GetTopupByEmployee(string employee_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Topup/gettopupbyemployee/{employee_id}");
            var content = await response.Content.ReadAsStringAsync();
            List<TopupModel> topups = JsonConvert.DeserializeObject<List<TopupModel>>(content);
            return topups;
        }

        public async Task<List<TopupModel>> GetTopups()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Topup/gettopups");
            var content = await response.Content.ReadAsStringAsync();
            List<TopupModel> topups = JsonConvert.DeserializeObject<List<TopupModel>>(content);
            return topups;
        }

        public async Task<string> Insert(TopupModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Topup/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> UpdateStatus(TopupModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Topup/updatestatus", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
