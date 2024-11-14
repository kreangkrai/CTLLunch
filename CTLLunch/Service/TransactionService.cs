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
    public class TransactionService : ITransaction
    {
        private IConnectAPI API;
        private readonly string URL;
        public TransactionService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<List<TransactionModel>> GetTransactionByDate(DateTime date)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Transaction/gettransactionbydate/{date.ToString("yyyy-MM-dd")}");
            var content = await response.Content.ReadAsStringAsync();
            List<TransactionModel> transactions = JsonConvert.DeserializeObject<List<TransactionModel>>(content);
            return transactions;
        }

        public async Task<List<TransactionModel>> GetTransactionByEmployee(string employee_id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Transaction/gettransactionbyemployee/{employee_id}");
            var content = await response.Content.ReadAsStringAsync();
            List<TransactionModel> transactions = JsonConvert.DeserializeObject<List<TransactionModel>>(content);
            return transactions;
        }

        public async Task<List<TransactionModel>> GetTransactionByMonth(string month)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Transaction/gettransactionbymonth/{month}");
            var content = await response.Content.ReadAsStringAsync();
            List<TransactionModel> transactions = JsonConvert.DeserializeObject<List<TransactionModel>>(content);
            return transactions;
        }

        public async Task<List<TransactionModel>> GetTransactions()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + $"Transaction/gettransactions");
            var content = await response.Content.ReadAsStringAsync();
            List<TransactionModel> transactions = JsonConvert.DeserializeObject<List<TransactionModel>>(content);
            return transactions;
        }

        public async Task<string> Insert(TransactionModel transaction)
        {
            var json = JsonConvert.SerializeObject(transaction);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Transaction/insert", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
