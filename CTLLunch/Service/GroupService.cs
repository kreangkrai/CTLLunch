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
    public class GroupService : IGroup
    {
        private IConnectAPI API;
        private readonly string URL;
        public GroupService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<string> Delete(string group_id)
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync(URL + $"Group/delete/{group_id}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<List<GroupMenuModel>> GetGroups()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Group/getgroups");
            var content = await response.Content.ReadAsStringAsync();
            List<GroupMenuModel> groups = JsonConvert.DeserializeObject<List<GroupMenuModel>>(content);
            return groups;
        }

        public async Task<string> GetLastID()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Group/getlastid");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Insert(GroupMenuModel group)
        {
            var json = JsonConvert.SerializeObject(group);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Group/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Update(GroupMenuModel group)
        {
            var json = JsonConvert.SerializeObject(group);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Group/update", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
