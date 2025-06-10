
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SuperShop.Data.Entities;
using System.Linq;

namespace SuperShop.Data
{
    public class DataContext : IdentityDbContext<User> //DbContext é a classe dos dados, representa a conexão com o banco, gerencia tabelas e consultas
                                                 // IdentityDbContext é a classe dos dados com autenticação
                                                 //DataContext: Classe que herda de DbContext, onde configuro as tabelas
    {

        // As tabelas são definidas por DbSet
        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<OrderDetailTemp> OrderDetailsTemp { get; set; }    

        public DbSet<Country> Countries { get; set; } 

        public DbSet<City> Cities { get; set; }

        // Recebe as configurações através do objeto options (DbContextOptions) e passa para a classe base DbContext,
        // permitindo que o Entity Framework configure e acesse o banco de dados.
        public DataContext(DbContextOptions<DataContext> options) : base (options) //DbOptions: Contém configurações (tipo de banco, string de conexão etc.)
        {      
        }

        // Habilita ou desabilita o modo cascata??? fiquei confusa com o Restrict ao final
        //protected override void OnModelCreating(ModelBuilder modelBuilder) //desabilita deletar em cascata direto na base de dados para impedir múltiplos caminhos de deleção em cascata.
        //{
        //    //antes de criar o modelo
        //    var cascadeFKs = modelBuilder.Model
        //         .GetEntityTypes() //buscar todas as entidades
        //         .SelectMany(t => t.GetForeignKeys()) //selecionar todas as chaves estrangeiras 
        //         .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade); // que tenham comportamento em cascata (relações com outras tabelas)

        //    foreach (var fk in cascadeFKs)
        //    {
        //        fk.DeleteBehavior = DeleteBehavior.Restrict; // restringe comportamento ao deletar, se houver entidades filhas, não deleta. Deve ser deletado por código individualmente.
        //    }

        //    base.OnModelCreating(modelBuilder); //devolve o modelo modificado
        //}
    }
}
