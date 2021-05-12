using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Services.EmailServices
{
    public class EmailSender
    {
        public void Send(string toAddress, string subject, string text)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Auctioneer", "auctioneer.proj@gmail.com"));
            message.To.Add(new MailboxAddress("Auctioneer User", toAddress));
            message.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = text;
            message.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("auctioneer.proj@gmail.com", "qwe!@#456");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
