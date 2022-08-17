using Microsoft.AspNetCore.Mvc;
using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Utils;
using EmpresariosConLiderazgo.Services;
using EmpresariosConLiderazgo.Data;
using Microsoft.EntityFrameworkCore;

namespace EmpresariosConLiderazgo.Controllers
{
    public class ReferController : Controller
    {
        private readonly IMailService mailService;
        private readonly ApplicationDbContext _context;

        public ReferController(ApplicationDbContext context, IMailService mailService)
        {
            _context = context;
            this.mailService = mailService;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "Id,Name,Mail")]
            Refer refer)
        {
            if (ModelState.IsValid)
            {
                var validateNewUser = _context.Users_App.Where(x => x.AspNetUserId == refer.Mail).ToList();
                if (validateNewUser.Count > 0)
                {
                    TempData["ErrorMessage"] =
                        $"El usuario {refer.Name?.ToString()} ,Ya existe en la plataforma";
                    return RedirectToAction("Index", "Home");
                }


                var refered = new ReferedByUser
                {
                    AspNetUserId = @User.Identity?.Name,
                    ReferedUserId = refer.Mail,
                    Date = DateTime.Now
                };
                await _context.ReferedByUser.AddAsync(refered);
                await _context.SaveChangesAsync();


                //Send Mail

                string subject = "Invitacion Empresarios Con Liderazgo";
                string body =
                    $"Hola {refer.Name.ToString()} Empresarios con liderazgo quiere q hagas parte del proyecto, por ende te invitamos a registrarte y ser parte de nuestra comunidad, Visita https://www.empresariosconliderazgo.com/Identity/Account/Register?ReturnUrl=%2F";

                var request = new MailRequest();

                request.Body = body;
                request.Subject = subject;
                request.ToEmail = refer.Mail.ToString();

                await mailService.SendEmailAsync(request);
            }

            TempData["AlertMessage"] =
                $"Se ha realizado la invitacion a {refer.Name.ToString()} , Muchas gracias por hacer que esta familia crezca";

            //return RedirectToAction("Index", "Home");
            return RedirectToAction("ReferedByMail", "Refer", new { @mail = User.Identity?.Name });
        }


        public async Task<IActionResult> ReferedByMail(string mail)
        {
            if (mail == null)
            {
                RedirectToPage("Error");
            }

            if (User.Identity?.Name != mail)
            {
                return NotFound();
            }

            var refer = _context.ReferedByUser.Where(x => x.AspNetUserId == mail).ToList();
            if (refer.Count == 0)
            {
                RedirectToPage("Error");
            }

            return View(refer);
        }
    }
}