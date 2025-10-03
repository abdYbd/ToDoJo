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
    public partial class ProfileViewModel : NavigateForViewModel
    {
        public ProfileViewModel(INavigationService navigationService, IAuthenticationService authenticationService) 
            : base(navigationService, authenticationService){}

        public ProfileViewModel() : this(null!, null!) { }
    }
}
