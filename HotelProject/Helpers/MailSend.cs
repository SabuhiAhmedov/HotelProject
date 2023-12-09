using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HotelProject.Helpers
{
    public static class MailSend
    {
        public static async Task SendMailAsync(string messageSubject, string messageBody, string mailTo)
        {
            //Smtp sorgu yarat
            SmtpClient client = new SmtpClient("smtp.yandex.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("sabuhi.a@itbrains.edu.az", "yyjufqfzehqlibfn");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //mesaj yarat
            MailMessage mailMessage = new MailMessage("sabuhi.a@itbrains.edu.az", mailTo);
            mailMessage.Subject = messageSubject;
            mailMessage.Body = messageBody;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {

                throw;
            }
           
        }


    }


}

