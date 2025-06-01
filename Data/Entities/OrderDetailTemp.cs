using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class OrderDetailTemp : IEntity
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; }  
        

        [Required]  
        public Product Product { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //formatar para currency 2 casas decimais
        public decimal Price { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)] //formatar para 2 casas decimais
        public double Quantity { get; set; }


        public decimal Value => Price * (decimal)Quantity; //get com lambda, o valor da encomenda é a quantidade vezes o preço
    }
}
