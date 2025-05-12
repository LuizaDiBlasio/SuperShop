
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class DataContext : IdentityDbContext<User> //DbContext é a classe dos dados, representa a conexão com o banco, gerencia tabelas e consultas
                                                 // IdentityDbContext é a classe dos dados com autenticação
                                                 //DataContext: Classe que herda de DbContext, onde configuro as tabelas
    {

        // A tabela Countries é definida por DbSet
        public DbSet<Product> Products { get; set; }

        // Recebe as configurações através do objeto options (DbContextOptions) e passa para a classe base DbContext,
        // permitindo que o Entity Framework configure e acesse o banco de dados.
        public DataContext(DbContextOptions<DataContext> options) : base (options) //DbOptions: Contém configurações (tipo de banco, string de conexão etc.)
        {
            
        }
    }
}
