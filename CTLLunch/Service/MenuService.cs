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
    public class MenuService : IMenu
    {
        private IConnectAPI API;
        private readonly string URL;
        public MenuService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> Delete(string menu_id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"Menu/delete/{menu_id}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> GetLastID()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Menu/getlastid");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<MenuModel> GetMenuByMenu(string menu_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Menu/getmenubymenu/{menu_id}");
            var content = await response.Content.ReadAsStringAsync();
            MenuModel menu = JsonConvert.DeserializeObject<MenuModel>(content);
            return menu;
        }

        public async Task<List<MenuModel>> GetMenuByShop(string shop_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Menu/getmenubyshop/{shop_id}");
            var content = await response.Content.ReadAsStringAsync();
            List<MenuModel> menus = JsonConvert.DeserializeObject<List<MenuModel>>(content);
            return menus;
        }

        public async Task<List<MenuModel>> GetMenus()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Menu/getmenus");
            var content = await response.Content.ReadAsStringAsync();
            List<MenuModel> menus = JsonConvert.DeserializeObject<List<MenuModel>>(content);
            return menus;
        }

        public async Task<string> Insert(MenuModel menu)
        {
            var json = JsonConvert.SerializeObject(menu);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Menu/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<MenuModel>> SearchMenuByShop(string shop_id, string menu)
        {
            if (menu != "" && menu != null)
            {
                var client = new HttpClient();
                var response = await client.GetAsync(URL + $"Menu/searchmenubyshop/{shop_id}/{menu}");
                var content = await response.Content.ReadAsStringAsync();
                List<MenuModel> menus = JsonConvert.DeserializeObject<List<MenuModel>>(content);
                return menus;
            }
            else
            {
                var client = new HttpClient();
                var response = await client.GetAsync(URL + $"Menu/getmenubyshop/{shop_id}");
                var content = await response.Content.ReadAsStringAsync();
                List<MenuModel> menus = JsonConvert.DeserializeObject<List<MenuModel>>(content);
                return menus;
            }
        }

        public async Task<string> Update(MenuModel menu)
        {
            var json = JsonConvert.SerializeObject(menu);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Menu/update", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> UpdateStatus(MenuModel menu)
        {
            var json = JsonConvert.SerializeObject(menu);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Menu/updatestatus", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
