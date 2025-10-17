using Avalonia.Input;
using ToDoJo.Data;
using ToDoJo.Services;
using ToDoJo.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ToDoJo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection, string connectionString)
        {
            collection.AddDbContext<AppDBContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
                ));

            collection.AddSingleton<INavigationService, NavigationService>();
            collection.AddSingleton<IAuthenticationService, AuthenticationService>();
            collection.AddSingleton<ILoggerService, LoggerService>();

            collection.AddTransient<MainWindowViewModel>();
            collection.AddTransient<LoginViewModel>();
            collection.AddTransient<RegisterViewModel>();
            collection.AddTransient<ToDoViewModel>();
            collection.AddTransient<SamuraiViewModel>();
            collection.AddTransient<ProfileViewModel>();
            //collection.AddTransient<NavigateForViewModel>();
        }
    }
}
