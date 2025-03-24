using CTLLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch.Interface
{
    public interface IMail
    {
        Task<List<MailModel>> GetEmailAddress();
        Task<string> SendEmailTransfer(MailDataModel mail);
        Task<string> SendEmailReceiver(MailDataModel mail);
        Task<string> SendEmailAdminTopup(MailDataModel mail);
        Task<string> SendEmailTopup(MailDataModel mail);
        Task<string> SendEmailApproveTopup(MailDataModel mail);
        Task<string> SendEmailCancelTopup(MailDataModel mail);
        Task<string> SendEmailPay(MailDataModel mail);
    }
}
