using Amazon;
using EmpresariosConLiderazgo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using EmpresariosConLiderazgo.Data;
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

            if (completed == null) {
                return RedirectToAction("Login", "Account", new { @mail = UserLogged });
            }   

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

        public IActionResult Working()
        {
            return View();
        }

        public IActionResult Support()
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }

        public IActionResult Transfer()
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }

        public IActionResult WebSite()
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }

        public IActionResult Credential()
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }

        public IActionResult News()
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }

        public IActionResult Crypto()
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

            return View();
        }

        public IActionResult Bancolombia()
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

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