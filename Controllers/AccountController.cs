using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class AccountController : Controller //controller respon´sável por logins
    {
        IUserHelper _userHelper;


        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;

        }

        //action do login para inserir credenciais
        public  IActionResult Login() //precisar criar a view do Login - clicar com o botão direito aqui em cima de Login() Add View
        {
            if (User.Identity.IsAuthenticated) //caso usuário esteja autenticado
            {
                return RedirectToAction("Index", "Home"); //mandar para a view Index que possui o controller Home
            }

            return View(); //se login não funcionar, permanece na View 
        }

        //action do login para enviar credencias para bd, faz o login de fato
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) // se modelo enviado passar na validação
            {
                var result = await _userHelper.LoginAsync(model); //fazer login

                if (result.Succeeded) //se login for bem sucedido
                {
                    //fez login e entrou através de uma Url de retorno (quando não tem permissão para entrar numa View qualquer sem login)
                    if (this.Request.Query.Keys.Contains("ReturnUrl")) // Verifica se o URL atual (o URL da página de login) inclui um parâmetro de query chamado ReturnUrl.                                                                       
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First()); //retorna a primeira Url contendo ReturnUrl e quando faz login entra na View onde tentou entrar e não na Home)
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");

            return View(model); //model retorna pra mesma View
        }

        public async Task<IActionResult> Logout() 
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");   
        }

        public IActionResult Register() //só mostra a view do Register
        {
            return View();  
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterNewUserViewModel model) // registra o user
        {
            if (ModelState.IsValid) //ver se modelo é válido
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username); //buscar user  

                if(user == null) // caso user não exista, registrá-lo
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password); //add user depois de criado

                    if(result != IdentityResult.Success) // caso não consiga criar user
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created");
                        return View(model); //passa modelo de volta para não ficar campos em branco
                    }

                    var loginViewModel = new LoginViewModel //mandar informações de login
                    {
                        Password = model.Password,
                        RememberMe = false,
                        Username = model.Username
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel); //fazer login 

                    if(result2.Succeeded) //se conseguiu logar, redirecionar para Home
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    //se não conseguiu loggar:
                    ModelState.AddModelError(string.Empty, "The user couldn't be logged");
                }   
            }

            return View(model); //passa modelo de volta para não ficar campos em branco
        }

    }
}
