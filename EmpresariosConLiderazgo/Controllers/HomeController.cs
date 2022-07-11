using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using EmpresariosConLiderazgo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using EmpresariosConLiderazgo.Data;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.AspNetCore.Identity;
using EmpresariosConLiderazgo.Services;

namespace EmpresariosConLiderazgo.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMailService mailService;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> SignInManager, ApplicationDbContext context, IMailService mailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = SignInManager;
            _context = context;
            this.mailService = mailService;
        }

        public IActionResult Index()
        {
            string UserLogged = User.Identity?.Name.ToString();

            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult SendWithSDK()
        {
            //Amazon.SimpleEmail.AmazonSimpleEmailServiceClient sesClient


            string senderAddress = "empresariosconliderazgo.org@gmail.com";
            string receiverAddress = "fercho0926@hotmail.com";
            string subject = "Titulo  : Bla bla ";
            string textBody = "Amazon SES Test (.NET)\r\n"
                              + "This email was sent through Amazon SES "
                              + "using the AWS SDK for .NET.";

            string htmlBody = @"<html>
<head></head>
<body>
  <h1>Amazon SES Test (AWS SDK for .NET)</h1>
  <p>This email was sent with
    <a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
    <a href='https://aws.amazon.com/sdk-for-net/'>
      AWS SDK for .NET</a>.</p>
</body>
</html>";


            // Replace USWest2 with the AWS Region you're using for Amazon SES.
            // Acceptable values are EUWest1, USEast1, and USWest2.
            using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = senderAddress,
                    Destination = new Destination
                    {
                        ToAddresses =
                            new List<string> { receiverAddress }
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = htmlBody
                            },
                            Text = new Content
                            {
                                Charset = "UTF-8",
                                Data = textBody
                            }
                        }
                    },
                    // If you are not using a configuration set, comment
                    // or remove the following line 
                    // ConfigurationSetName = configSet
                };
                try
                {
                    Console.WriteLine("Sending email using Amazon SES...");
                    var response = client.SendEmailAsync(sendRequest);
                    Task.WaitAll(response);

                    Console.WriteLine("The email was sent successfully." + response.Result.MessageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }


            return View();
        }


        public IActionResult Refer()
        {
            return View();
        }


        public IActionResult FinalRegister()
        {
            return RedirectToPage("~/Users_App/Index");
        }
    }
}