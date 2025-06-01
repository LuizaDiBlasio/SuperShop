using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext context) : base(context)
        {
            _context = context; 
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Products.Include(p => p.User); //comando Include equivale ao inner join em SQL, vai buscar os produtos com Users
        }

        public IEnumerable<SelectListItem> GetComboProducts() //método que cria a lista que aparece na combobox
        {
            //linguagem funcional em c#, não precisa de foreach. Para cada produto do repositorio vai criar um objeto correspodente do tipo SelectListItem
            // preenche as propriedades Text com o nome e Value com o Id
            var list = _context.Products.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList(); //ao final converter resultado em uma lista 

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a product...)",
                Value = "0"
            });

            return list;    
        }
    }
}
