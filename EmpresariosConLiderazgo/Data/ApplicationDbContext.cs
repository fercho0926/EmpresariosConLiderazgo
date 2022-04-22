using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmpresariosConLiderazgo.Models;

namespace EmpresariosConLiderazgo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EmpresariosConLiderazgo.Models.Users_App> Users_App { get; set; }
        public DbSet<EmpresariosConLiderazgo.Models.Logs> User_Logs { get; set; }
    }
}