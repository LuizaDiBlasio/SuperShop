using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Models
{
    public class AddItemViewModel
    {
        [Display(Name="Product")]
        [Range(1,int.MaxValue, ErrorMessage ="You must select a product")] //tem que selecionar um produto da combobox
        public int ProductId { get; set; }


        [Range(0.0001, double.MaxValue, ErrorMessage = "The quantity must be a positive number")] //tem que selecionar uma quantidade da combobox
        public double Quantity { get; set; }

        public IEnumerable<SelectListItem> Products { get; set; } // SelectListItem - classe de items de uma lista html, neste caso essa propriedade mostra a lista de produtos  

    }
}
