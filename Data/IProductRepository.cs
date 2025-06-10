using Microsoft.EntityFrameworkCore;
using System.Linq;
using SuperShop.Data.Entities;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace SuperShop.Data
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IQueryable GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboProducts(); //busca lista de produtos para colocar na combobox no formato SelectListItem de html
    }
}
