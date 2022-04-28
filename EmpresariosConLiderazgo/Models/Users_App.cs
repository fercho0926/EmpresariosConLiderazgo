using EmpresariosConLiderazgo.Utils;
using System.ComponentModel.DataAnnotations;

namespace EmpresariosConLiderazgo.Models
{
    public class Users_App : BaseClass
    {
        //Personal Information
        [Required, StringLength(60), RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Use letters only please")]
        public string? LastName { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Identification { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateBirth { get; set; }

        //Contact Information
        [Required]
        public EnumCountries EnumCountries { get; set; }
        [Required]
        [MaxLength(50)]
        public string? City { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Neighborhood { get; set; }
        [Required]
        [MaxLength(80)]
        public string? Address { get; set; }
        [DataType(DataType.PhoneNumber), StringLength(25)]
        public string? phone { get; set; }

        //Connect with aspnetUsers Table
        public string? AspNetUserId { get; set; }



    }
}
