using EmpresariosConLiderazgo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EmpresariosConLiderazgo.Models
{
    public class Users_App : BaseClass
    {
        //Personal Information
        public string? LastName { get; set; }
        public string? Identification { get; set; }
        public DateTime DateBirth { get; set; }

        //Contact Information
        public EnumCountries EnumCountries { get; set; }
        public int City { get; set; }
        public string? Neighborhood { get; set; }
        public string? Address { get; set; }
        public string? phone { get; set; }

        //Connect with aspnetUsers Table
        public string? AspNetUserId { get; set; }

    }
}
