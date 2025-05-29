using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Helpers;

namespace SuperShop.Data
{
    public class SeedDb //classe responsável pelo Seed --> funcionalidade para alimentar ábase de dados sempre que elafor iniciada vazia 
    {
        private readonly DataContext _context;

        private readonly IUserHelper _userHelper;

        private Random _random; // atributo de objeto que permite gerar produtos aleatoriamente

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;   
            _random = new Random();

        }

        public async Task SeedAsync() //método para adicionar produtoas à DB
        {
            await _context.Database.EnsureCreatedAsync(); // checa se a BD está criada, caso não esteja, cria uma BD aos moldes do _context

            await _userHelper.CheckRoleAsync("Admin"); //verificar se já existe um role de admin, se não existir cria

            
            await _userHelper.CheckRoleAsync("Customer"); //verificar se já existe um role de customer, se não existir cria

            var user = await _userHelper.GetUserByEmailAsync("Luizabandeira90@gmail.com"); //ver se user já existe 

            if (user == null) // caso não encontre o utilizador 
            {
                user = new User // cria utilizador 
                {
                    FirstName = "Luiza",
                    LastName = "Bandeira",
                    Email = "luizabandeira90@gmail.com",
                    UserName = "luizabandeira90@gmail.com",
                    PhoneNumber = "12345678"
                };

                var result = await _userHelper.AddUserAsync(user, "123456"); //criar utilizador, mandar utilizador e password
                
                if(result != IdentityResult.Success) //se o resultado não for bem sucedido (usa propriedade da classe Identity) 
                {
                    throw new InvalidOperationException("Coud not create the user in seeder"); //pára o programa
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin"); //adiciona role ao user
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin"); //verifica se role foi designado para user existente

            if (!isInRole) //se não estiver o role, colocar
            {
                await _userHelper.CheckRoleAsync("Admin");
            }

            if (!_context.Products.Any()) //se não conter nenhum elemento
            {
                //adiciona produtos em memória

                AddProduct("Iphone X", user); //adicionar nome do produto e user como parametros para adicionar produto
                AddProduct("Magic Mouse", user);
                AddProduct("Iwatch", user);
                AddProduct("Ipad Mini", user);

                await _context.SaveChangesAsync(); //adiciona produtos na base de dados
            }
        }

        private void AddProduct(string name, User user) // cria e adiciona produtos 
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000), //usar objeto random para gerar valores aleatorios até 1000
                IsAvailable = true,
                Stock = _random.Next(100),
                User = user // adicionar user na criação do produto
            });
        }
    }
}
