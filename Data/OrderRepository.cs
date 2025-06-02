using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;

        private readonly IUserHelper _userHelper;   
        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;   
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string username) //adiciona itens à order, pela primeira vez ou não
        {
            var user = await _userHelper.GetUserByEmailAsync(username); //checar user

            if (user == null)
            {
                return; // para ação se não houver user
            }

            var product = await _context.Products.FindAsync(model.ProductId); // checar se produto está na bd

            if(product == null)
            {
                return; //parar ação se não houver produto
            }

            //se houver produto e user, buscar orderDetaiTemp
            var orderDetailTemp = await _context.OrderDetailsTemp.Where(odt => odt.User == user && odt.Product == product).FirstOrDefaultAsync();

            if (orderDetailTemp == null) //se não houver odt, significa que é a primeira order a ser feita
            {
                orderDetailTemp = new OrderDetailTemp
                {
                    Price = product.Price,
                    Product = product,
                    Quantity = model.Quantity,
                    User = user,    
                };

                _context.OrderDetailsTemp.Add(orderDetailTemp); //adicionar novo objeto em memória
            }
            else //se order já existe
            {
                orderDetailTemp.Quantity += model.Quantity; //adicionar à quantitade antiga
                
                _context.OrderDetailsTemp.Update(orderDetailTemp); //faz update à order em memória
            }

            await _context.SaveChangesAsync(); //gravar na base de dados

        }

        public async Task<bool> ConfirmOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);   //checar user

            if(user == null)
            {
                return false;   
            }

            var orderTmps = await _context.OrderDetailsTemp.Include(o => o.Product).Where(o => o.User == user).ToListAsync(); //buscar todos os produtos de todas as orders do user

            if(orderTmps == null || orderTmps.Count == 0) //se retornar nulo ou vazio
            {
                return false;
            }

            // Criar uma lista de order details com os detalhes das orders temp --> converte cada order temporária em order detail 
            var details = orderTmps.Select(o => new OrderDetail
            {
                Price = o.Price,
                Product = o.Product,    
                Quantity = o.Quantity,

            }). ToList();

            // Criar a order 
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                User = user,
                Items = details
            };

            await CreateAsync(order); //salvar order

            _context.OrderDetailsTemp.RemoveRange(orderTmps); //remover as orders temporárias (RemoveRange remove vários)

            await _context.SaveChangesAsync(); //salvar

            return true;
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id); //buscar order temporária
            
            if(orderDetailTemp == null)
            {
                return; 
            }

            _context.OrderDetailsTemp.Remove(orderDetailTemp); //se exister order, remover
            
            await _context.SaveChangesAsync();  //salvar alterações na bd
        }

        public async Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string name)
        {
           var user = await _userHelper.GetUserByEmailAsync(name); //sempre verificar o user

            if (user == null)
            {
                return null;  //se não houver user retorna lista vazia  
            }

            //se não for nulo:
            //retornar lista de produtos das orders do user ordenada pelo nome do produto (include equivale a um inner join em sql)
            return _context.OrderDetailsTemp
                .Include(p => p.Product)
                .Where(o => o.User == user)
                .OrderBy(o => o.Product.Name); //tabela produtos nesse caso tem ligação direta com OrderDetailsTemp (por isso é Include e não ThenInclude)
        }

        public async Task<IQueryable<Order>> GetOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName); //buscar user
            
            if (user == null)
            {
                return null;  //se não houver user retorna lista vazia  
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Admin")) //se user for admin
            {
                //retornar uma lista com os items da Order (Todas as encomendas do Admin) contendo o produto por ordem descendente de data de encomenda
                return _context.Orders
                    .Include(o => o.User) //incluir users para mostrar quando confirmar as orders
                    .Include(o => o.Items)
                    .ThenInclude(p => p.Product)
                    .OrderByDescending(o => o.OrderDate); //Then Include usado para fazer join com tabelas não adjacentes, sempre vem depois do Include
            }

            //caso não seja admin, buscar todas as orders do user (Ir na lista Items em Orders  buscar orders com nome do produto, ordenar por data da order )
            return _context.Orders.Include(o => o.Items).ThenInclude(p => p.Product).Where(o => o.User == user).OrderByDescending(o => o.OrderDate);  
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, int quantity) //order já foi selecionada, e agora vai ter sua quantidade modificada
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);    

            if(orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;

            if(orderDetailTemp.Quantity > 0) // como pode aumentar ou diminuir quantidade de produtos, tem que checar se é > 0
            {
                _context.OrderDetailsTemp.Update(orderDetailTemp);  
                await _context.SaveChangesAsync(); //salva na base de dados
            }    
        }
    }
}
