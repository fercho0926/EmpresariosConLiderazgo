#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Utils;
using ExpertPdf.HtmlToPdf;



namespace EmpresariosConLiderazgo.Controllers
{
    public class User_contractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public User_contractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User_contracts
        public async Task<IActionResult> Index()
        {
            return View(await _context.User_contracts.ToListAsync());
        }

        public ActionResult ColombianContract()
        {

            //string[] paths = { @"d:\archives", "2001", "media", "images" };
            //string fullPath = Path.Combine(paths);
            //Console.WriteLine(fullPath);
            //return new Rotativa.AspNetCore.ViewAsPdf("Index", fullPath);







            var uscon = new User_contracts()
            {
                Approved = true,
                Product = "asd"
            };

            GenerateDocumentKeyNUA(uscon);

            return null;
        }



        [HttpPost]
        public IActionResult GenerateDocumentKeyNUA(User_contracts contractInfo)
        {

            if (contractInfo == null)
            {
                throw new ArgumentNullException(nameof(contractInfo));
            }

            GeneratePDF generatePDF = new GeneratePDF();

            string newDocumentFileName = generatePDF.GenerateInvestorDocument(contractInfo);

            if (string.IsNullOrWhiteSpace(newDocumentFileName))
                return BadRequest("Un error ocurrió al crear el archivo.");

            return Ok(newDocumentFileName);

        }










        // GET: User_contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_contracts = await _context.User_contracts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_contracts == null)
            {
                return NotFound();
            }

            return View(user_contracts);
        }

        // GET: User_contracts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User_contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserContract,Version,Product,StartDate,EndDate,S3Route,Approved,Id,Name")] User_contracts user_contracts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user_contracts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user_contracts);
        }

        // GET: User_contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_contracts = await _context.User_contracts.FindAsync(id);
            if (user_contracts == null)
            {
                return NotFound();
            }
            return View(user_contracts);
        }

        // POST: User_contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserContract,Version,Product,StartDate,EndDate,S3Route,Approved,Id,Name")] User_contracts user_contracts)
        {
            if (id != user_contracts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user_contracts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!User_contractsExists(user_contracts.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user_contracts);
        }

        // GET: User_contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user_contracts = await _context.User_contracts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user_contracts == null)
            {
                return NotFound();
            }

            return View(user_contracts);
        }

        // POST: User_contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user_contracts = await _context.User_contracts.FindAsync(id);
            _context.User_contracts.Remove(user_contracts);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool User_contractsExists(int id)
        {
            return _context.User_contracts.Any(e => e.Id == id);
        }
    }
}
