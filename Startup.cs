using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperShop.Data;

namespace SuperShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //serviço de conexão que registra o DataContext e indica o uso da connection string escrita no appsettings
            services.AddDbContext<DataContext>(config =>
            {
                config.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<SeedDb>(); // configuração da injeção de dependências, objeto criado quando serviço for requisitado,
                                             // depois de usado é descartado e só poderá ser criado novamente em uma nova execução da aplicação 

            services.AddScoped<IProductRepository, ProductRepository>(); //quando for necessário, instanciar o objeto de Repository
                                                           // num próximo uso, o objeto antigo será destruído e um novo será instanciado 
                                                           //O serviço de Interface de repositórios permite trocar repositórios e fazer testes com diversar bases de dados
                                                           // basta manter a interface base IRepository e trocar o repositório que se comunica com a nova base de dados (Repository) 

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
