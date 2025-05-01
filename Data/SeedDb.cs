using System;
using System.Linq;
using System.Threading.Tasks;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class SeedDb //classe responsável pelo Seed --> funcionalidade para alimentar ábase de dados sempre que elafor iniciada vazia 
    {
        private readonly DataContext _context;

        private Random _random; // atributo de objeto que permite gerar produtos aleatoriamente

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync() //método para adicionar produtoas à DB
        {
            await _context.Database.EnsureCreatedAsync(); // checa se a BD está criada, caso não esteja, cria uma BD aos moldes do _context

            if (!_context.Products.Any()) //se não conter nenhum elemento
            {
                //adiciona produtos em memória

                AddProduct("Iphone X");
                AddProduct("Magic Mouse");
                AddProduct("Iwatch");
                AddProduct("Ipad Mini");

                await _context.SaveChangesAsync(); //adiciona produtos na base de dados
            }
        }

        private void AddProduct(string name) // cria e adiciona produtos 
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000), //usar objeto random para gerar valores aleatorios até 1000
                IsAvailable = true,
                Stock = _random.Next(100)
            });
        }
    }
}
