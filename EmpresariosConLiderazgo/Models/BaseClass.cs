using System.ComponentModel.DataAnnotations;

namespace EmpresariosConLiderazgo.Models
{
    public class BaseClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

    }
}
