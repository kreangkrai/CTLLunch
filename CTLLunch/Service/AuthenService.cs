using CTLLunch.Interface;
using CTLLunch.Models;
using Newtonsoft.Json;
using System;
using System.DirectoryServices;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class AuthenService : IAuthen
    {
        private IConnectAPI API;
        private readonly string URL;
        public AuthenService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<AuthenModel> ActiveDirectoryAuthenticate(string username, string password)
        {
            AuthenModel authen = new AuthenModel();
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Authen/{username}/{password}");
            var content = await response.Content.ReadAsStringAsync();
            authen = JsonConvert.DeserializeObject<AuthenModel>(content);
            return authen;
        }
    }
}
