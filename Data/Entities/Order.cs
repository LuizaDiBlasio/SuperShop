using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SuperShop.Data.Entities
{
    public class Order : IEntity
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Order date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)] //formatação da date em ano mes dia ano hora minuto segundo
        public DateTime OrderDate { get; set; }

        [Required]
        [DisplayName("Delivery date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)] //formatação da date em ano mes dia ano hora minuto segundo
        public DateTime DeliveryDate { get; set; }

        [Required]
        public User User { get; set; }

        public IEnumerable<OrderDetail> Items { get; set; } //lista de produtos (cada produto tem um OrderDetail associado) dentro da encomenda (Order)
                                                            //relação de um pra muitos

        [DisplayFormat(DataFormatString ="{0:N0}")]
        public int Lines => Items == null ? 0 : Items.Count(); //se os items forem nulos, são 0 linhas senão, contar os itens


        [DisplayFormat(DataFormatString = "{0:N2}")] //formatar para 2 casas decimais
        public double Quantity => Items == null ? 0 : Items.Sum(i => i.Quantity); //caso não haja itens na encomenda, quantidade igual a zero, se houver itens, buscar a soma de todos
                                                                                  //Obs: usar expressões lambda para propriedades calculadas

        [DisplayFormat(DataFormatString = "{0:C2}")] //formatar para 2 casas decimais
        public decimal Value => Items == null ? 0 : Items.Sum(i => i.Value);

        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime? OrderDateLocal => this.OrderDate == null? null : this.OrderDate.ToLocalTime(); //se a OrderDate for nula, atribuir nulo à propriedade, caso o contrario, converter para a hora local
    }
}
