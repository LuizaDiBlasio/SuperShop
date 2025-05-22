using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;

namespace SuperShop.Helpers
{
    public interface IUserHelper //interface dedicada à gestão do User, bypass
    {
        Task<User> GetUserByEmailAsync(string email); //passa o email para buscar user

        Task<IdentityResult> AddUserAsync(User user, string password); //adiciona user na BD
    }
}
