using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class AccountController : Controller //controller responsável por logins
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

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); //buscar user por email

            var model = new ChangeUserViewModel(); //criar modelo para mostrar dados

            if (user != null) //caso user exista, preencher novo modelo com dados do user
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model); //retornar model novo para view
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); //buscar user por email

                if (user != null) //caso user exista, user com propridades registradas no modelo
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName; 

                    var response = await _userHelper.UpdateUserAsync(user); //fazer update do user

                    if(response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description); //pedir a primeira mensagem de erro
                    }
                }
            }
            return View(model); //retornar model novo para view
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid) //se modelo é válido
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); //verificar user
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword); //muda password

                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser"); //redireciona para view ChangeUser
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description); //mensagem de erro
                    }
                }
                else //se for nulo
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            return this.View(model); //retornar model para view caso corra mal
        }
    }
}
