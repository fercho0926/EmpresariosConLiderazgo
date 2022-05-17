using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Utils
{
    public class SendMail
    {
        public static void SendMailWithoutattached(string subject, string to, string mailbody)
        {
            //string to = "milton.vasquez0726@gmail.com"; //To address    
            string from = "EmpresariosConLiderazgo-Notificaciones@gmail.com"; //From address    
            MailMessage message = new MailMessage(from, to);

            //string mailbody = "In this article you will learn how to send a email using Asp.Net & C#";
            message.Subject = subject;
            message.Body = mailbody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
                System.Net.NetworkCredential("empresariosconliderazgoNotify@gmail.com", "Colombia1*");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}