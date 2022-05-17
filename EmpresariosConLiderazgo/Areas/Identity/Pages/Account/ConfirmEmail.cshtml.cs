// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Text;
using EmpresariosConLiderazgo.Data;
using EmpresariosConLiderazgo.Models;
using EmpresariosConLiderazgo.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace EmpresariosConLiderazgo.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);


            var UserApp = new Users_App
            {
                Name = "",
                LastName = "",
                Identification = "",
                DateBirth = DateTime.Today,
                EnumCountries = EnumCountries.PorDefinir,
                City = "",
                Neighborhood = "",
                Address = "",
                phone = "",
                AspNetUserId = user.ToString()
            };

            _context.Users_App.Add(UserApp);
            _context.SaveChanges();

            var vea = _context.Users_App.ToList();
            var registerUserApps =
                StatusMessage = result.Succeeded
                    ? "Registro Exitoso"
                    : "Error confirming your email.";
            return Page();
            //return RedirectToPage("/Index");
        }
    }
}