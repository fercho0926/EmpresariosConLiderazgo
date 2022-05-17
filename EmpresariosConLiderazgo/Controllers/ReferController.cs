using Microsoft.AspNetCore.Mvc;
using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Utils;


namespace EmpresariosConLiderazgo.Controllers
{
    public class ReferController : Controller
    {
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
                //Pending save 

                //Send Mail

                string subject = "Invitacion Empresarios Con Liderazgo";
                string body =
                    $"Hola {refer.Name.ToString()} Empresarios con liderazgo quiere q hagas parte del proyecto, por ende te invitamos a registrarte y ser parte de nuestra comunidad, Visita https://empresariosconliderazgo.org/";

                SendMail.SendMailWithoutattached(subject, refer.Mail.ToString(), body);
            }

            TempData["AlertMessage"] =
                $"Se ha realizado la invitacion a {refer.Name.ToString()} , Muchas gracias por hacer que esta familia crezca";

            return RedirectToAction("Index", "Home");
        }
    }
}