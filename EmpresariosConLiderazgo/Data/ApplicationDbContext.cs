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
        public DbSet<EmpresariosConLiderazgo.Models.Ultima> Ultima { get; set; }
    }
}