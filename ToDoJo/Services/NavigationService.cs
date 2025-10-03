using ToDoJo.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoJo.Services;


namespace ToDoJo.Services
{
    public class NavigationService : INavigationService
    {
        private MainWindowViewModel? _mainWindowViewModel;
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task NavigateTo<T>() where T : ViewModelBase
        {
            if (_mainWindowViewModel == null)
                return Task.CompletedTask;

            var vm = _serviceProvider.GetRequiredService<T>();
            _mainWindowViewModel.CurrentViewModel = vm;
            return Task.CompletedTask;
        }

        public void SetMainViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
    }
}
