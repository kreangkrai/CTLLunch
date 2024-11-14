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
    public class PlanOutOfIngredientsService : IPlanOutOfIngredients
    {
        private IConnectAPI API;
        private readonly string URL;
        public PlanOutOfIngredientsService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> DeleteById(string id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"PlanOutOfIngredients/deletebyid/{id}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> DeleteByShop(string shop_id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"PlanOutOfIngredients/deletebyshop/{shop_id}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        public async Task<List<PlanOutOfIngredientsModel>> GetPlanOutOfIngredients()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"PlanOutOfIngredients/getplanoutofingredients");
            var content = await response.Content.ReadAsStringAsync();
            List<PlanOutOfIngredientsModel> planOutOfIngredients = JsonConvert.DeserializeObject<List<PlanOutOfIngredientsModel>>(content);
            return planOutOfIngredients;
        }

        public async Task<List<PlanOutOfIngredientsModel>> GetPlanOutOfIngredientsByDate(DateTime now)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"PlanOutOfIngredients/getplanoutofingredientsbydate/{now.ToString("yyyy-MM-dd")}");
            var content = await response.Content.ReadAsStringAsync();
            List<PlanOutOfIngredientsModel> planOutOfIngredients = JsonConvert.DeserializeObject<List<PlanOutOfIngredientsModel>>(content);
            return planOutOfIngredients;
        }

        public async Task<List<PlanOutOfIngredientsModel>> GetPlanOutOfIngredientsByShop(string shop_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"PlanOutOfIngredients/getplanoutofingredientsbyshop/{shop_id}");
            var content = await response.Content.ReadAsStringAsync();
            List<PlanOutOfIngredientsModel> planOutOfIngredients = JsonConvert.DeserializeObject<List<PlanOutOfIngredientsModel>>(content);
            return planOutOfIngredients;
        }

        public async Task<string> Insert(PlanOutOfIngredientsModel plan)
        {
            var json = JsonConvert.SerializeObject(plan);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "PlanOutOfIngredients/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
