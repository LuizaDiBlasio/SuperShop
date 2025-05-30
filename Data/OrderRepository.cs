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
                return _context.Orders.Include(o => o.Items).ThenInclude(p => p.Product).OrderByDescending(o => o.OrderDate);
            }

            //caso não seja admin, buscar todas as orders do user (Ir na lista Items em Orders  buscar orders com nome do produto, ordenar por data da order )
            return _context.Orders.Include(o => o.Items).ThenInclude(p => p.Product).Where(o => o.User == user).OrderByDescending(o => o.OrderDate);  
        }
    }
}
