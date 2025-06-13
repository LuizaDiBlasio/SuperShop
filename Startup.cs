using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using System.Text;

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

            services.AddIdentity<User, IdentityRole>(cfg => //adicionar serviço de Identiy para ter o user e configurar o serviço
            {
                //configurações inseguras para poder fazer testes, mas em cenário déprodução deve ser o oposto
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider; //buscar token com provider default
                cfg.SignIn.RequireConfirmedEmail = true; //só permitir sign in se confirmar email
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<DataContext>() //Depois do serviço implementado continua a usar o DataContext, aplicar o serviço criado à BD
              .AddDefaultTokenProviders();

            //adicionar servico de autentificação do Token e configurar os parâmetros
            //vai ser utilizado na autenticação do user quando for usar a API dos produtos
            services.AddAuthentication().AddCookie().AddJwtBearer(cfg =>
            {
                //mandar configurações do token
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = this.Configuration["Tokens:Issuer"],
                    ValidAudience = this.Configuration["Tokens:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"]))
                };
            });

            //serviço de conexão que registra o DataContext e indica o uso da connection string escrita no appsettings
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<SeedDb>(); // configuração da injeção de dependências, objeto criado quando serviço for requisitado,
                                             // depois de usado é descartado e só poderá ser criado novamente em uma nova execução da aplicação 
            services.AddScoped<IUserHelper, UserHelper>();

            services.AddScoped<IBlobHelper, BlobHelper>();

            services.AddScoped<IConverterHelper, ConverterHelper>();

            services.AddScoped<IMailHelper, MailHelper>();

            services.AddScoped<IProductRepository, ProductRepository>(); //quando for necessário, instanciar o objeto de Repository
                                                                         // num próximo uso, o objeto antigo será destruído e um novo será instanciado 
                                                                         //O serviço de Interface de repositórios permite trocar repositórios e fazer testes com diversar bases de dados
                                                                         // basta manter a interface base IRepository e trocar o repositório que se comunica com a nova base de dados (Repository)

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<ICountryRepository, CountryRepository>();

            //anula o ReturnUrl no Login (AccountController)
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized"; //ao invés de aparecer página do login, executar a action de NotAuthorized
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });

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
                app.UseExceptionHandler("/Errors/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //executar isso sempre no primeiro controller (Home)
            app.UseStatusCodePagesWithReExecute("/error/{0}"); //não encontra uma certa´página e reexecuta algo, envia por parametro o endpoint do controller que diz como as páginas são executadas

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // usar autenticação, tem que ser na ordem, antes do enpoints e authorizations

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
