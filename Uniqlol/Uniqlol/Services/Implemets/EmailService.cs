using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Uniqlol.Helpers;
using Uniqlol.Services.Abstracts;
using Microsoft.AspNetCore.Identity;

namespace Uniqlol.Services.Implemets
{
    public class EmailService : IEmailService
    {
        readonly SmtpClient _client;
        readonly MailAddress _from;
        readonly HttpContext Context;
        public EmailService(IOptions<SmtpOptions> option, IHttpContextAccessor acc)
        {
            var opt = option.Value;
            _client = new(opt.Host, opt.Port);
            _client.Credentials = new NetworkCredential(opt.Sender, opt.Password);
            _client.EnableSsl = true;
            _from = new MailAddress(opt.Sender, "Uniqlo");
            Context = acc.HttpContext;
        }

        public void SendEmailConfirmation(string reciever, string name, string token)
        {
            MailAddress to = new(reciever);
            MailMessage msg = new MailMessage(_from,to);
            msg.IsBodyHtml = true;
            msg.Subject = "Confirm your email adress";
            string url = Context.Request.Scheme + "://" + Context.Request.Host + "/Account/VerifyEmail?token=" + token + "&user=" + name;
            msg.Body = EmailTemplates.VerifyEmail.Replace("__$name", name).Replace("__$link", url);
            _client.Send(msg);
        }
        public void ResetPassword(string reciever,string name,string token)
        {
            MailAddress to = new(reciever);
            MailMessage msg = new MailMessage(_from, to);
            msg.IsBodyHtml = true;
            msg.Subject = "Reset password";
            string url = Context.Request.Scheme + "://" + Context.Request.Host + "/Account/ResetPassword?token=" + token + "&user=" + name;
           
            _client.Send(msg);
        }
    }


}

