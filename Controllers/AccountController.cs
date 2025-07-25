﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace SuperShop.Controllers
{
    public class AccountController : Controller //controller responsável por logins
    {
        private readonly IUserHelper _userHelper;

        private readonly ICountryRepository _countryRepository;  

        private readonly IConfiguration _configuration; //para aceder o appsettings.json

        private readonly IMailHelper _mailHelper; 


        public AccountController(IUserHelper userHelper, ICountryRepository countryRepository, IConfiguration configuration, IMailHelper mailHelper)
        {
            _userHelper = userHelper;

            _countryRepository = countryRepository; 

            _configuration = configuration;

            _mailHelper = mailHelper;   
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
            var model = new RegisterNewUserViewModel
            {
                Countries = _countryRepository.GetComboCountries(), //preencher combo dos paíse
                Cities = _countryRepository.GetComboCities(0) // preencher combo das cities - mandar Id 0 fora de range para preencher com placeholder em prieiro lugar
            };

            return View(model); //mandar model com combos preenchidas para a View  
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model) // registra o user
        {
            if (ModelState.IsValid) //ver se modelo é válido
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username); //buscar user  

                if(user == null) // caso user não exista, registrá-lo
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId); //buscar a cidade selecionada na combo

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        CityId = model.CityId,
                        City = city 
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password); //add user depois de criado

                    if(result != IdentityResult.Success) // caso não consiga criar user
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created");
                        return View(model); //passa modelo de volta para não ficar campos em branco
                    }

                    //TODO  checar se roles estão corretos pra customer
                    await _userHelper.AddUserToRoleAsync(user, "Customer"); //adiciona role ao user

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //gerar o token

                    // gera um link de confirmção para o email
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new  //Link gerado na Action ConfirmEmail dentro do AccountController, ela recebe 2 parametros (userId e token)
                    {
                        userId = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme); //utiliza o protocolo Http para passar dados de uma action para a outra

                    Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                    $"To allow the user,<br><br><a href = \"{tokenLink}\">Confirm Email</a>"); //Contruir email e enviá-lo com o link 

                    if(response.IsSuccess) //se conseguiu enviar o email
                    {
                        ViewBag.Message = "Confirmation instructions have been sent to your email";

                        return View(model);
                    }

                    //se não conseguiu enviar email:
                    ModelState.AddModelError(string.Empty, "The user couldn't be logged");
                }

                var isInRole = await _userHelper.IsUserInRoleAsync(user, "Customer"); //verifica se role foi designado para user existente

                if (!isInRole) //se não estiver o role, colocar
                {
                    await _userHelper.CheckRoleAsync("Customer");
                }
            }

            return View(model); //passa modelo de volta para não ficar campos em branco
        }

        //GET do ChangeUser
        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); //buscar user por email

            var model = new ChangeUserViewModel(); //criar modelo para mostrar dados

            if (user != null) //caso user exista, preencher novo modelo com dados do user
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;

                var city = await _countryRepository.GetCityAsync(user.CityId);//buscar a cidade do user

                if (city != null) // caso encontrada 
                {
                    var country = await _countryRepository.GetCountyAsync(city); //buscar o país por meio da cidade

                    if (country != null) //caso país seja encontrado
                    {
                        model.CountryId = country.Id; //preencher o país do user
                        model.CityId = user.CityId; // preencher a cidade do user 
                        model.Cities = _countryRepository.GetComboCities(country.Id); //preencher combo com cidade atual do user
                        model.Countries = _countryRepository.GetComboCountries(); //preencher combo
                    }
                }
            }

            model.Cities = _countryRepository.GetComboCities(model.CountryId);
            model.Countries = _countryRepository.GetComboCountries();

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
                    var city = await _countryRepository.GetCityAsync(model.CityId); //buscar cidade

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName; 
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.CityId = model.CityId;
                    user.City = city;
                    

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


        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)// recebe um modelo LoginViewMode que te user e password
        {
            if (this.ModelState.IsValid) //se o modelo for válido 
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username); //verificar se o email existe
                if (user != null)
                {
                    //validar password
                    var result = await _userHelper.ValidatePasswordAsync( 
                        user,
                        model.Password);

                    if (result.Succeeded) //se válido
                    {
                        //criar claims (objetos usados para fazer tolkens, autorizações, autenticações, percurso do usuario na aplicação)
                        var claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email), //registra email
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //criar Guid associado ao email
                };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])); //Encriptação da key no arquivo appsettings
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //algoritmo que gera um token com a key
                        var token = new JwtSecurityToken( //configuração do token
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15), //tempo de validade
                            signingCredentials: credentials);

                        var results = new //gera objeto anônimo de retorno que contém token e validade
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results); //retornar resultado
                    }
                }
            }

            return BadRequest(); // em caso de insucesso, madar bad request
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) //verificar parâmetros
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId); //verificar user

            if(user == null)
            {
                return NotFound();  
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token); //resposta do email, ver se user e token dão match

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }


        public IActionResult RecoverPassword() //direciona para view de recover da password
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model) //recebe modelo com dados recuperar password
        {
            if (this.ModelState.IsValid)//verificar se modelo é válido
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email); //buscar user
                if (user == null)
                {
                    //mensagem de erro caso user não exista
                    ModelState.AddModelError(string.Empty, "The email doesn't correspond to a registered user."); 
                    return View(model);
                }

                // caso exista user prosseguir e gerar o token
                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action( //criar o link de reset da password
                    "ResetPassword",
                    "Account",
                    new { token = myToken },
                    protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Email, "Shop Password Reset", $"<h1>Shop Password Reset</h1>" + //mandar o email
                    $"To reset the password click in this link </br><br/>" +
                    $"<a href = \"{link}\">Reset Password</a>");

                if (response.IsSuccess) //se correr tudo bem
                {
                    this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
                }

                return this.View();
            }

            return this.View(model);
        }


        public IActionResult ResetPassword(string token) //direciona para view de reset da password
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) //recebo modelo preechido com dados para reset da password
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Username); //buscar user

            if (user != null) //caso user exista
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password); //fazer reset da password
                
                if (result.Succeeded) //se tudo correr bem
                {
                    this.ViewBag.Message = "Password reset successful.";
                    return this.View();
                }

                //se não correr bem
                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            //caso não encontro o user
            this.ViewBag.Message = "User not found.";
            return View(model);
        }

        public IActionResult NotAuthorized()
        {
            return View();      
        }

        [HttpPost]
        [Route("Account/GetCitiesAsync")] //mapeamento para o AJAX: quando houver essa URL, executar essa action
        public async Task<JsonResult> GetCitiesAsync(int countryId) //retorna em jason result para poder popular a lista com objetos
        {
            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId); //buscar país com lista das suas cidades

            return this.Json(country.Cities.OrderBy(c => c.Name)); //retornar todas as cidades ordenadas por nome e convertidas para Json
        }

    }
}
