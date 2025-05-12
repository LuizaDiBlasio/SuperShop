using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity 
    {
        private readonly DataContext _context;
        public GenericRepository(DataContext context) // injeção de dependencia do DataContext
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();  //AsNoTrackin(); busca registos numa tabela sem manter ligação, Set<T> é uma tabela qualquer
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>()
                .AsNoTracking() // não segue registos
                .FirstOrDefaultAsync(e => e.Id == id); //busca a entidade do id dado por parametro
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity); //adicionar à tabela 

            await SaveAllAsync(); //método da classe mas que não está no interface 
        }

        public async Task UpdateAsync(T entity)
        { 
            _context.Set<T>().Update(entity); //não é async, por que ?

            await SaveAllAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity); //não é async

            await SaveAllAsync();
        }

        public async Task<bool> ExistAsync(int id)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id);  
        }

        private async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        //OBS: técnica de bypass criar seus próprios métodos e colocar os métodos da biblioteca dentro. A princípio parece repetitivo mas casa haja mudanças na framework, é só mudar
        //esses´m´todos na classe, evita ter que fazer modificações ao longo de todo o código
    }
}
