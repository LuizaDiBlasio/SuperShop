using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public interface IGenericRepository<T> where T : class // o genérico aqui é necessariamente uma classe
    {
        IQueryable<T> GetAll(); // devolve todas os objetos de uma dada classe

        Task<T> GetByIdAsync(int id); //Id definido pelo IEntity

        Task CreateAsync(T entity); //cria uma entidade qualquer

        Task UpdateAsync(T entity); //faz update de uma entidade qualquer

        Task DeleteAsync(T entity);  // deleta uma entidade qualquer

        Task<bool> ExistAsync(int id); // ver se existem objetos de uma entitade qualquer

        //não possui salvar pois este repositório é genérico e para salvar precisa saber a estrutura da tabela da entidade específica 
    }
}
