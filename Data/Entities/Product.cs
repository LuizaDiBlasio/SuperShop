using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class Product : IEntity 
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} allows only {1} characters")] //mensagem não chega a ser mostrada
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] // data anotation para a formatação do preço (mostra 2 casas decimais mas no modo edição pode ter mais casas deciamais)
        public decimal Price { get; set; }

        [Display (Name = "Image")] //como o campo aparece na página web 
        public string ImageURL { get; set; }

        [Display(Name = "Last Purchase")]
        public DateTime? LastPurchase {  get; set; }

        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)] //N se refere ao número, o C acima se refere à currency
        public double Stock {  get; set; }  

        public User User { get; set; }  
    }
}
