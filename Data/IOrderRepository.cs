using SuperShop.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>> GetOrderAsync(string name); //passa uma lista de encomendas de um determinado user

        Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string name); //passa as orders temporárias de um determinado user


    }
}
