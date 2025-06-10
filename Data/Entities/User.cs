using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters length")]
        public string FirstName { get; set; }


        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters length")]
        public string LastName { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters length")]
        public string Address { get; set; } 


        public int CityId { get; set; } //FK


        public City City { get; set; } //objeto de navegação. Devido ao fato de User herdar de IdentityUser, precisa colocar tanto a FK quanto o nav obj


        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
