using System.ComponentModel.DataAnnotations;

namespace EmpresariosConLiderazgo.Models
{
    public class BaseClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string? Name { get; set; }

    }
}
