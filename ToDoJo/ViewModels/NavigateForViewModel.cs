using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoJo.Services;

namespace ToDoJo.ViewModels
{
    public abstract partial class NavigateForViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isPaneOpen = true;


        private readonly INavigationService _navigationService;
        private readonly IAuthenticationService _authenticationService;

        [RelayCommand]
        private void OpenPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        public NavigateForViewModel(INavigationService navigationService, IAuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
        }

        public NavigateForViewModel() : this(null!, null!) { }

        [RelayCommand]
        private async Task NavigateToTask() => await _navigationService.NavigateTo<ToDoViewModel>();

        [RelayCommand]
        private async Task NavigateToSamurai() => await _navigationService.NavigateTo<SamuraiViewModel>();

        [RelayCommand]
        private async Task NavigateToProfile() => await _navigationService.NavigateTo<ProfileViewModel>();

        [RelayCommand]
        private async Task LogOut()
        {
            _authenticationService.Logout();
            await _navigationService.NavigateTo<LoginViewModel>();
        }

    }
}
