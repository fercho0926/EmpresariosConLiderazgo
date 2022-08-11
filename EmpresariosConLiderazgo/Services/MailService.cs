using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Settings;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Newtonsoft.Json.Linq;


namespace EmpresariosConLiderazgo.Services
{
    public class MailService : IMailService
    {
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var password = "";
            var userC = "";
            var smttp = "";
            var port = "";


            var client = new AmazonSecretsManagerClient();


            var response = await client.GetSecretValueAsync(new GetSecretValueRequest()
            {
                SecretId = "Mail"
            });

            var secretValues = JObject.Parse(response.SecretString);
            if (secretValues != null)
            {
                password = secretValues["Mail_Password"].ToString();
                userC = secretValues["Mail_From"].ToString();
                smttp = secretValues["Mail_Smtp"].ToString();
                port = secretValues["Mail_Port"].ToString();
            }


            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(userC);
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
            smtp.Connect(smttp, int.Parse(port), SecureSocketOptions.StartTls);
            smtp.Authenticate(userC, password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}