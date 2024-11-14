using CTLLunch.Interface;
using CTLLunch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CTLLunch.Service
{
    public class EmployeeService : IEmployee
    {
        private IConnectAPI API;
        private readonly string URL;
        public EmployeeService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<List<EmployeeModel>> GetEmployees()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Employee/getemployees");
            var content = await response.Content.ReadAsStringAsync();
            List<EmployeeModel> employees = JsonConvert.DeserializeObject<List<EmployeeModel>>(content);
            return employees;

        }
        public async Task<List<UserModel>> GetUserAD()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Employee/getuserad");
            var content = await response.Content.ReadAsStringAsync();
            List<UserModel> users = JsonConvert.DeserializeObject<List<UserModel>>(content);
            return users;
        }

        public async Task<string> GetLastEmployee()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Employee/getlastemployee");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> Insert(EmployeeModel employee)
        {
            var json = JsonConvert.SerializeObject(employee);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Employee/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> UpdateBalance(EmployeeModel employee)
        {
            var json = JsonConvert.SerializeObject(employee);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Employee/updatebalance", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> UpdateRole(EmployeeModel employee)
        {
            var json = JsonConvert.SerializeObject(employee);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Employee/updaterole", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<EmployeeModel> GetEmployeeCTL()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Employee/getemployeectl");
            var content = await response.Content.ReadAsStringAsync();
            EmployeeModel employee = JsonConvert.DeserializeObject<EmployeeModel>(content);
            return employee;
        }

        public async Task<string> UpdateStatus(EmployeeModel employee)
        {
            var json = JsonConvert.SerializeObject(employee);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(URL + "Employee/updatestatus", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
