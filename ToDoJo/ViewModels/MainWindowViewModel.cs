using ToDoJo.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ToDoJo.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    [ObservableProperty] private ViewModelBase _currentViewModel;

    public MainWindowViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        ((NavigationService)navigationService).SetMainViewModel(this);
        navigationService.NavigateTo<LoginViewModel>();
    }

    public MainWindowViewModel() : this(null!) { }
}