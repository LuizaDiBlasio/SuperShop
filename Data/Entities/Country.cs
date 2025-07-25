﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class Country : IEntity
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
          
        public string Name { get; set; }


        public ICollection<City> Cities { get; set; } //cidades de um país


        //propriedade de leitura que devolve o número de cidades
        [Display(Name = "Number of cities")]
        public int NumberCities => Cities == null ? 0 : Cities.Count;

    }
}
