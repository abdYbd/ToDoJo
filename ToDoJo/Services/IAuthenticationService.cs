using ToDoJo.Models;
using System.Threading.Tasks;

namespace ToDoJo.Services
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUserAsync(string login, string email, string password);
        Task<User?> LoginUserAsync(string email, string password);
        Task<bool> UserExistsAsync(string email);
        User? CurrentUser { get; } 
        void Logout();
    }
}
