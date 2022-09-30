using Microsoft.AspNetCore.Mvc;
using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Utils;
using EmpresariosConLiderazgo.Services;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Models.Entities;
using Microsoft.AspNetCore.Authorization;
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
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }

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

                var checkMail = await _context.ReferedByUser.OrderByDescending(x => x.Date).FirstOrDefaultAsync(x => x.ReferedUserId == refer.Mail);
                if (checkMail != null)
                {
                    TempData["ErrorMessage"] =
                        $"El usuario {refer.Name?.ToString()} ,Ya fue invitado a unirse a la plataforma";
                    await SendMail(refer);
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

                CreateMovement(refered.Id, "Se envia Invitación", EnumStatusBalance.PENDIENTE);


                //Send Mail

                await SendMail(refer);
            }

            TempData["AlertMessage"] =
                $"Se ha realizado la invitacion a {refer.Name.ToString()} , Muchas gracias por hacer que esta familia crezca";

            return RedirectToAction("ReferedByMail", "Refer", new { @mail = User.Identity?.Name });
        }

        private async Task SendMail(Refer refer)
        {
            string subject = "Invitacion Empresarios Con Liderazgo";
            string body =
                $"Hola {refer.Name.ToString()} Empresarios con liderazgo quiere q hagas parte del proyecto, por ende te invitamos a registrarte y ser parte de nuestra comunidad, Visita https://www.empresariosconliderazgo.com/Identity/Account/Register?ReturnUrl=%2F";

            var request = new MailRequest();

            request.Body = body;
            request.Subject = subject;
            request.ToEmail = refer.Mail.ToString();

            await mailService.SendEmailAsync(request);
        }

        public async Task<IActionResult> ReferedByMail(string mail)
        {
            string UserLogged = User.Identity?.Name.ToString();
            var completed = _context.Users_App.FirstOrDefault(m => m.AspNetUserId == UserLogged);

            if (completed.Identification == "")
            {
                return RedirectToAction("EditByMail", "Users_App", new { @mail = UserLogged });
            }


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


        public async Task<IActionResult> SaveArray(List<int> data)
        {
            foreach (var id in data)
            {
                var res = await _context.ReferedByUser.SingleOrDefaultAsync(x => x.Id == id);
                res.EnumStatusReferido = EnumStatusReferido.SOLICITUD_RETIRO;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult ApproveRetireCommissions()
        {
            var records = _context.ReferedByUser.Where(x => x.EnumStatusReferido == EnumStatusReferido.SOLICITUD_RETIRO)
                .ToList();


            return View(records);
        }


        public async Task<IActionResult> GetMovementById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.ReferedByUserMovement.Where(x => x.ReferedByUserId == id).ToListAsync();


            return View(result);
        }


        public async Task<IActionResult> ApproveCommissionById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commission = await _context.ReferedByUser.SingleOrDefaultAsync(x => x.Id == id);
            commission!.EnumStatusReferido = EnumStatusReferido.ABONADO_A_CUENTA;


            await _context.SaveChangesAsync();
            CreateMovement(Convert.ToInt32(id), $"Retiro  por {commission.AmountToRefer} Abonado",
                EnumStatusBalance.APROBADO);


            TempData["AlertMessage"] =
                $"Se ha realizado El abono  por {commission.AmountToRefer} , al usuario {commission.AspNetUserId}";

            return RedirectToAction("ApproveRetireCommissions", "Refer", new { @mail = User.Identity?.Name });
        }

        public async Task<IActionResult> TansferMoney(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commission = await _context.ReferedByUser.SingleOrDefaultAsync(x => x.Id == id);
            commission!.EnumStatusReferido = EnumStatusReferido.SOLICITUD_RETIRO;


            await _context.SaveChangesAsync();
            CreateMovement(Convert.ToInt32(id), $"Solicitud de retiro por {commission.AmountToRefer}",
                EnumStatusBalance.SOLICITUD_RETIRO);


            TempData["AlertMessage"] =
                $"Se ha realizado la solicitud de retiro por {commission.AmountToRefer} , Este pago se verá reflejado entre los dias 12 al 15 de cada mes";

            return RedirectToAction("ReferedByMail", "Refer", new { @mail = User.Identity?.Name });
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectCommissionById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var commission = await _context.ReferedByUser.SingleOrDefaultAsync(x => x.Id == id);
            commission!.EnumStatusReferido = EnumStatusReferido.RECHAZADO;


            await _context.SaveChangesAsync();
            CreateMovement(Convert.ToInt32(id), $"Retiro  por {commission.AmountToRefer} RECHAZADO",
                EnumStatusBalance.RECHAZADO);


            TempData["ErrorMessage"] =
                $"Se ha RECHAZADO El abono  por {commission.AmountToRefer} , al usuario {commission.AspNetUserId}";

            return RedirectToAction("ApproveRetireCommissions", "Refer", new { @mail = User.Identity?.Name });
        }


        public void CreateMovement(int referedID, string message, Utils.EnumStatusBalance status)
        {
            var movement = new ReferedByUserMovement()
            {
                ReferedByUserId = referedID,
                Message = message,
                DateMovement = DateTime.Now,
                Status = status
            };
            _context.ReferedByUserMovement.Add(movement);
            _context.SaveChanges();
        }
    }
}