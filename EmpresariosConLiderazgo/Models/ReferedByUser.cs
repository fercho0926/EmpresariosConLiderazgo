using System.ComponentModel.DataAnnotations;
using EmpresariosConLiderazgo.Utils;

namespace EmpresariosConLiderazgo.Models
{
    public class ReferedByUser
    {
        [Key] public int Id { get; set; }
        [Display(Name = "Quien refiere")] public string? AspNetUserId { get; set; }
        [Display(Name = "Referido")] public string? ReferedUserId { get; set; }
        [Display(Name = "Fecha")] public DateTime Date { get; set; }
        [Display(Name = "Acepto invitación")] public bool Accepted { get; set; } = false;
        [Display(Name = "Realizo inversión")] public bool InvestDone { get; set; } = false;
        [Display(Name = "Aprobado Admin")] public bool ApproveByAdmin { get; set; } = false;

        [Display(Name = "Comisión x referido")]
        [DisplayFormat(DataFormatString = "{0:N0} ")]
        public decimal AmountToRefer { get; set; }

        [Display(Name = "Estado")] public EnumStatusReferido EnumStatusReferido { get; set; } = 0;
    }
}