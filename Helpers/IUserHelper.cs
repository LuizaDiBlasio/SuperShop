using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Models;

namespace SuperShop.Helpers
{
    public interface IUserHelper //interface dedicada à gestão do User, bypass
    {
        Task<User> GetUserByEmailAsync(string email); //passa o email para buscar user

        Task<IdentityResult> AddUserAsync(User user, string password); //adiciona user na BD

        Task<SignInResult> LoginAsync(LoginViewModel model); //método que devolve tarefa SignInResult (ou tá signed in ou não)

        Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user); //update user na BD

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword); //muda pass do user 

        Task CheckRoleAsync(string roleName); //verifica se existem roles

        Task AddUserToRoleAsync(User user, string roleName); // designa o role ao user

        Task<bool> IsUserInRoleAsync(User user, string roleName); // verifica se user está designado ao role

        Task<SignInResult> ValidatePasswordAsync(User user, string password); //não faz login, só valida a password para acesso à API

        Task<string> GenerateEmailConfirmationTokenAsync(User user); //Gera o email de confirmação e insere o Token 

        Task<IdentityResult> ConfirmEmailAsync(User user, string token); //Valida o email, verifica se o token é valido

        Task<User> GetUserByIdAsync(string id); //recebe um id e devolve o user correspondente

        Task<string> GeneratePasswordResetTokenAsync(User user); //cria token do reset da password, user irá receber token

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password); //Faz o reset da password

    }
}
