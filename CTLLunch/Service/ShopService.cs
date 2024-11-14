using CTLLunch.Interface;
using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace CTLLunch.Service
{
    public class ShopService : IShop
    {
        private IConnectAPI API;
        private readonly string URL;
        public ShopService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> Delete(string shop_id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"Shop/delete/{shop_id}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> GetLastID()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Shop/getlastid");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<ShopModel>> GetShops()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Shop/getshops");
            var content = await response.Content.ReadAsStringAsync();
            List<ShopModel> shops = JsonConvert.DeserializeObject<List<ShopModel>>(content);
            return shops;
        }

        public async Task<string> Insert(ShopModel shop)
        {
            var json = JsonConvert.SerializeObject(shop);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Shop/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Update(ShopModel shop)
        {
            var json = JsonConvert.SerializeObject(shop);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Shop/update", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
     
        public async Task<string> UpdateCloseTimeShift(string shop_id)
        {
            var client = new HttpClient();
            var response = await client.PutAsync(URL + $"Shop/updateclosetimeshift/{shop_id}", null);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
