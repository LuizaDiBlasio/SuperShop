using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class Repository : IRepository
    // camada intermediária entre o controlador e o modelo de dados. Caso a base de dados mude, este o controlador mantém inalterado pois é independente da base de dados 
    {
        private readonly DataContext _context;
        public Repository(DataContext context)  // ligação com base de dados através da injeção de dependência
        {
            _context = context;
        }

        // CRUD
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.OrderBy(p => p.Name);
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Find(id);
        }

        public void AddProduct(Product product) // adiciona somente em memória
        {
            _context.Products.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
        }

        public void RemoveProduct(Product product)
        {
            _context.Products.Remove(product);
        }

        //Auxiliares

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.Id == id);
        }
    }
}
