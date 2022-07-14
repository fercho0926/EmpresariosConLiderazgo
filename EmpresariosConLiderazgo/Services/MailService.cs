using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Settings;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;


namespace EmpresariosConLiderazgo.Services
{
    public class MailService : IMailService
    {
        //private readonly MailSettings _mailSettings;
        //public MailService(MailSettings mailSettings)
        //{
        //    _mailSettings = mailSettings;
        //}


        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("empresariosconliderazgoNotify@gmail.com");
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }

                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("empresariosconliderazgoNotify@gmail.com", "lokhcgulnhmqpeod");
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}