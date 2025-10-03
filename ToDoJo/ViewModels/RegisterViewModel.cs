
using ToDoJo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ToDoJo.Services;
using ToDoJo.ViewModels;
using System.Runtime.InteropServices;

namespace ToDoJo.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        [Required]
        [StringLength(20)]
        private string _login = string.Empty;
        [ObservableProperty]
        [Required]
        [EmailAddress]
        private string _email= string.Empty;
        [ObservableProperty]
        [Required]
        [MinLength(5)]
        private string _password = string.Empty;
        [ObservableProperty]
        [Required]
        [MinLength(5)]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public RegisterViewModel(INavigationService navigationService, IAuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
        }

        public RegisterViewModel() : this(null!, null!) { }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await _navigationService.NavigateTo<LoginViewModel>();
        }

        [RelayCommand]
        private async Task Register()
        {
            ValidateAllProperties();
            if (HasErrors)
                return;

            if(Password != ConfirmPassword)
            {
                ErrorMessage = "Password and confirm password do not match";
                return;
            }

            ErrorMessage = string.Empty;

            try
            {
                IsLoading = true;

                await Task.Delay(2000);
                if(await _authenticationService.UserExistsAsync(Email))
                {
                    ErrorMessage = "A user with that email already exists";
                    return;
                }

                var succes = await _authenticationService.RegisterUserAsync(Login, Email, Password);
                if (succes)
                {
                    await _navigationService.NavigateTo<ToDoViewModel>();
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
