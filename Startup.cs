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

            services.AddIdentity<User, IdentityRole>(cfg => //adicionar servi�o de Identiy para ter o user e configurar o servi�o
            {
                //configura��es inseguras para poder fazer testes, mas em cen�rio d�produ��o deve ser o oposto
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider; //buscar token com provider default
                cfg.SignIn.RequireConfirmedEmail = true; //s� permitir sign in se confirmar email
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<DataContext>() //Depois do servi�o implementado continua a usar o DataContext, aplicar o servi�o criado � BD
              .AddDefaultTokenProviders();

            //adicionar servico de autentifica��o do Token e configurar os par�metros
            //vai ser utilizado na autentica��o do user quando for usar a API dos produtos
            services.AddAuthentication().AddCookie().AddJwtBearer(cfg =>
            {
                //mandar configura��es do token
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = this.Configuration["Tokens:Issuer"],
                    ValidAudience = this.Configuration["Tokens:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"]))
                };
            });

            //servi�o de conex�o que registra o DataContext e indica o uso da connection string escrita no appsettings
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<SeedDb>(); // configura��o da inje��o de depend�ncias, objeto criado quando servi�o for requisitado,
                                             // depois de usado � descartado e s� poder� ser criado novamente em uma nova execu��o da aplica��o 
            services.AddScoped<IUserHelper, UserHelper>();

            services.AddScoped<IBlobHelper, BlobHelper>();

            services.AddScoped<IConverterHelper, ConverterHelper>();

            services.AddScoped<IMailHelper, MailHelper>();

            services.AddScoped<IProductRepository, ProductRepository>(); //quando for necess�rio, instanciar o objeto de Repository
                                                                         // num pr�ximo uso, o objeto antigo ser� destru�do e um novo ser� instanciado 
                                                                         //O servi�o de Interface de reposit�rios permite trocar reposit�rios e fazer testes com diversar bases de dados
                                                                         // basta manter a interface base IRepository e trocar o reposit�rio que se comunica com a nova base de dados (Repository)

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<ICountryRepository, CountryRepository>();

            //anula o ReturnUrl no Login (AccountController)
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized"; //ao inv�s de aparecer p�gina do login, executar a action de NotAuthorized
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
            app.UseStatusCodePagesWithReExecute("/error/{0}"); //n�o encontra uma certa�p�gina e reexecuta algo, envia por parametro o endpoint do controller que diz como as p�ginas s�o executadas

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // usar autentica��o, tem que ser na ordem, antes do enpoints e authorizations

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
