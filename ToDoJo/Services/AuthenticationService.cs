using ToDoJo.Data;
using ToDoJo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace ToDoJo.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        //private readonly AppDBContext _context;

        public AuthenticationService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;
        public User? CurrentUser { get; private set; }

        public async Task<User?> LoginUserAsync(string email, string password)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

                if (user == null || !VerifyPassword(password, user.Password))
                    return null;

                CurrentUser = user;
                return user;
            }
            catch
            {
                return null;
            }
        }
        public void Logout()
        {
            CurrentUser = null;
        }

        private bool VerifyPassword(string password, string? dbPass)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == dbPass;
        }

        public async Task<bool> RegisterUserAsync(string login, string email, string password)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                if(await context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant()))
                    return false;

                var passwordHash= HashPassword(password);
                var user = new User()
                {
                    Login = login,
                    Email = email.ToLowerInvariant(),
                    Password = passwordHash,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(user); 
                await context.SaveChangesAsync();

                CurrentUser = user;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bashBytes);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                return await context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
