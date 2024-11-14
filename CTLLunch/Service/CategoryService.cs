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
    public class CategoryService : ICategory
    {
        private IConnectAPI API;
        private readonly string URL;
        public CategoryService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> Delete(string category_id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"Category/delete/{category_id}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<CategoryMenuModel>> GetCategories()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Category/getcategories");
            var content = await response.Content.ReadAsStringAsync();
            List<CategoryMenuModel> categories = JsonConvert.DeserializeObject<List<CategoryMenuModel>>(content);
            return categories;
        }

        public async Task<string> GetLastID()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Category/getlastid");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Insert(CategoryMenuModel category)
        {
            var json = JsonConvert.SerializeObject(category);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Category/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Update(CategoryMenuModel category)
        {
            var json = JsonConvert.SerializeObject(category);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Category/update", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
