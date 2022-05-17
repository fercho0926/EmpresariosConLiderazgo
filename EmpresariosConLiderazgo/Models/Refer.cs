using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpresariosConLiderazgo.Models
{
    public class Refer : BaseClass
    {
        [Required, MaxLength(100), Display(Name = "Correo")]
        [DataType(DataType.EmailAddress), EmailAddress]
        public string Mail { get; set; }
    }
}