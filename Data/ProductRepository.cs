using System.Linq;
using Microsoft.EntityFrameworkCore;
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
    }
}
