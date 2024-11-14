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
    public class IngredientsService : IIngredients
    {
        private IConnectAPI API;
        private readonly string URL;
        public IngredientsService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> Delete(string ingredients_id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"Ingredients/delete/{ingredients_id}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<IngredientsMenuModel>> GetIngredients()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Ingredients/getingredients");
            var content = await response.Content.ReadAsStringAsync();
            List<IngredientsMenuModel> ingredients = JsonConvert.DeserializeObject<List<IngredientsMenuModel>>(content);
            return ingredients;
        }

        public async Task<string> GetLastID()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Ingredients/getlastid");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Insert(IngredientsMenuModel ingredients)
        {
            var json = JsonConvert.SerializeObject(ingredients);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Ingredients/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Update(IngredientsMenuModel ingredients)
        {
            var json = JsonConvert.SerializeObject(ingredients);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Ingredients/update", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
