using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
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

        public async Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string name)
        {
           var user = await _userHelper.GetUserByEmailAsync(name); //sempre verificar o user

            if (user == null)
            {
                return null;  //se não houver user retorna lista vazia  
            }

            //se não for nulo:
            //retornar lista de produtos das orders do user ordenada pelo nome do produto (include equivale a um inner join em sql)
            return _context.OrderDetailsTemp.Include(p => p.Product).Where(o => o.User == user).OrderBy(o => o.Product.Name); //tabela produtos nesse caso tem ligação direta com OrderDetailsTemp (por isso é Include e não ThenInclude)
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
                return _context.Orders.Include(o => o.Items).ThenInclude(p => p.Product).OrderByDescending(o => o.OrderDate); //Then Include usado para fazer join com tableas não adjacentes
            }

            //caso não seja admin, buscar todas as orders do user (Ir na lista Items em Orders  buscar orders com nome do produto, ordenar por data da order )
            return _context.Orders.Include(o => o.Items).ThenInclude(p => p.Product).Where(o => o.User == user).OrderByDescending(o => o.OrderDate);  
        }
    }
}
