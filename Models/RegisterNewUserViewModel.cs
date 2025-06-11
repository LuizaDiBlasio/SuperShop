using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display (Name = "First Name" )]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [DataType (DataType.EmailAddress)] // Obriga a colocar email
        public string Username { get; set; }

       
        [MaxLength(100, ErrorMessage ="The field {0} only can cointain {1} characters.")]
        public string Address { get; set; }


        [MaxLength(20, ErrorMessage = "The field {0} only can cointain {1} characters.")]
        public string PhoneNumber { get; set; }


        [Display(Name ="City")]
        [Range(1, int.MaxValue, ErrorMessage ="You must select a city.")]
        public int CityId { get; set; } 


        public IEnumerable<SelectListItem> Cities { get; set; }


        public IEnumerable<SelectListItem> Countries { get; set; }


        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country.")]
        public int CountryId { get; set; }


        [Required]
        [MinLength (6)]
        public string Password { get; set; }   


        [Required]
        [Compare ("Password")] //confirma se campo abaixo é igual ao campo de cima.
        public string Confirm { get; set; }
    }
}
