using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace SuperShop.Models
{
    public class DeliveryViewModel
    {
        public int Id { get; set; }

        [DisplayName("Delivery date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)] //formatação da date em ano mes dia ano hora minuto segundo
        public DateTime DeliveryDate { get; set; } //opcional
    }
}
