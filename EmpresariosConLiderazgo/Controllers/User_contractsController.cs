#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Utils;
using EmpresariosConLiderazgo.Services;
using ExpertPdf.HtmlToPdf;
using SelectPdf;


namespace EmpresariosConLiderazgo.Controllers
{
    public class User_contractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDocumentService _documentService;
        private readonly ICloudwatchLogs _cloudwatchLogs;


        public User_contractsController(ApplicationDbContext context, IDocumentService documentService,
            ICloudwatchLogs cloudwatchLogs)
        {
            _context = context;
            _documentService = documentService;
            _cloudwatchLogs = cloudwatchLogs;
        }

        // GET: User_contracts
        public async Task<IActionResult> Index()
        {
            return View(await _context.User_contracts.ToListAsync());
        }

        [HttpGet]
        public IActionResult Get()
        {
            var pdfFile = _documentService.GeneratePdfFromString();
            return File(pdfFile, "application/octet-stream", "SimplePdf.pdf");
        }


        public IActionResult Ultimo()
        {
            _cloudwatchLogs.InsertLogs("PDF", Request.Path.Value!, "ENTRO");

            HtmlToPdf converter = new HtmlToPdf();

            PdfDocument doc = converter.ConvertUrl("www.google.com");

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "Document.pdf";
            return fileResult;
        }

        public IActionResult Ultimo_old()
        {
            //    //Logica que obtenga los datos de la BD
            //    //Convertir información a HTML
            //    //var htmlCode = 
            //    //    File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Template\\invoice1.html");
            //    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            //    SelectPdf.PdfDocument doc = converter.ConvertHtmlString(
            //        "<html>< head >< meta charset = 'utf-8' />< title > A simple, clean, and responsive HTML invoice template </ title ></head>< body ></body></html> ");
            //    doc.Save(AppDomain.CurrentDomain.BaseDirectory + "Template\\invoice2.pdf");
            //    byte[] data = doc.Save();
            //    var result = Convert.ToBase64String(data);


            //    doc.Close();
            //    //{status = "ok", data = "result"};
            return View();
        }
    }
}