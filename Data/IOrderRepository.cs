using SuperShop.Data.Entities;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>> GetOrderAsync(string name); //passa uma lista de orders de um determinado user

        Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string name); //passa as orders temporárias de um determinado user

        Task AddItemToOrderAsync(AddItemViewModel model, string username); //adiciona produtos à order

        Task ModifyOrderDetailTempQuantityAsync(int id, int quantity);  //modifica a quantidade total de produtos na order quando novos item são adicionados

        Task DeleteDetailTempAsync(int id); //deleta a order temporária
    }
}
