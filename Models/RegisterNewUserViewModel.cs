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

        [Required]
        [MinLength (6)]
        public string Password { get; set; }   

        [Required]
        [Compare ("Password")] //confirma se campo abaixo é igual ao campo de cima.
        public string Confirm { get; set; }
    }
}
