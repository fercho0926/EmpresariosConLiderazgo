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
        public DbSet<EmpresariosConLiderazgo.Models.User_contracts> User_contracts { get; set; }
        public DbSet<EmpresariosConLiderazgo.Models.Entities.Balance> Balance { get; set; }
        public DbSet<EmpresariosConLiderazgo.Models.Entities.MovementsByBalance> MovementsByBalance { get; set; }
        public DbSet<EmpresariosConLiderazgo.Models.ReferedByUser> ReferedByUser { get; set; }
        public DbSet<EmpresariosConLiderazgo.Models.ReferedByUserMovement> ReferedByUserMovement { get; set; }
    }
}