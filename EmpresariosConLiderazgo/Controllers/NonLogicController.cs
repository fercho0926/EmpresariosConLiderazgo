using Microsoft.AspNetCore.Mvc;

namespace EmpresariosConLiderazgo.Controllers
{
    public class NonLogicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Packages()
        {
            return View("Packages");
        }
    }
}
