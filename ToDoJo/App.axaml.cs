using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ToDoJo.Data;
using ToDoJo.Extensions;
using ToDoJo.ViewModels;
using ToDoJo.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace ToDoJo
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Отключаем плагин валидации (как у тебя)
                BindingPlugins.DataValidators.RemoveAt(0);

                // Setup DI (как у тебя)
                var collection = new ServiceCollection();
                string connectionString = Environment.GetEnvironmentVariable("TODOLIST_DB")
                                          ?? "server=127.0.0.1;user=root;password=00892204BEkKY!;database=todolist_db;";

                collection.AddCommonServices(connectionString);
                _serviceProvider = collection.BuildServiceProvider();

                // Create main window *сразу*, чтобы UI появился
                var mainWindow = new MainWindow();
                var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
                mainWindow.DataContext = mainViewModel;
                desktop.MainWindow = mainWindow; // окно появится

                // Запускаем инициализацию БД в фоне — и обязательно ловим исключения
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                        await context.Database.EnsureCreatedAsync();
                        System.Diagnostics.Debug.WriteLine("DB init ok");
                    }
                    catch (Exception ex)
                    {
                        // лог в Output — потом разберёмся с ошибкой
                        System.Diagnostics.Debug.WriteLine($"DB init failed: {ex}");
                    }
                });
            }

            base.OnFrameworkInitializationCompleted();
        }

    }
}