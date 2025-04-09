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
    public class MailService : IMail
    {
        private IConnectAPI API;
        private readonly string URL;
        public MailService(IConnectAPI _API)
        {
            API = _API;
            URL = API.ConnectAPI();
        }
        public async Task<List<MailModel>> GetEmailAddress()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(URL + "Mail/gets");
            var content = await response.Content.ReadAsStringAsync();
            List<MailModel> mails = JsonConvert.DeserializeObject<List<MailModel>>(content);
            return mails;
        }

        public async Task<string> SendEmailAdminTopup(MailDataModel mail)
        {
            var json = JsonConvert.SerializeObject(mail);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Mail/admintopup", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> SendEmailApproveTopup(MailDataModel mail)
        {
            var json = JsonConvert.SerializeObject(mail);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Mail/approvetopup", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> SendEmailPay(MailDataModel mail)
        {
            var json = JsonConvert.SerializeObject(mail);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Mail/pay", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> SendEmailReceiver(MailDataModel mail)
        {
            var json = JsonConvert.SerializeObject(mail);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Mail/receiver", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> SendEmailTopup(MailDataModel mail)
        {
            var json = JsonConvert.SerializeObject(mail);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Mail/topup", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> SendEmailTransfer(MailDataModel mail)
        {
            var json = JsonConvert.SerializeObject(mail);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Mail/transfer", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> SendEmailCancelTopup(MailDataModel mail)
        {
            var json = JsonConvert.SerializeObject(mail);
            HttpClient client = new HttpClient();
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(URL + "Mail/canceltopup", byteContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
