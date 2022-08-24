using System.ComponentModel.DataAnnotations;
using EmpresariosConLiderazgo.Utils;

namespace EmpresariosConLiderazgo.Models
{
    public class ReferedByUserMovement
    {
        [Key]
        [Display(Name = "Id Movimiento")]
        public int MovementId { get; set; }

        [Display(Name = "Fecha movimiento")] public DateTime DateMovement { get; set; }

        [Display(Name = "Mensaje")] public String? Message { get; set; }

        [Display(Name = "Estado")] public EnumStatusBalance Status { get; set; }
        [Display(Name = "Id Referido")] public int ReferedByUserId { get; set; }
    }
}