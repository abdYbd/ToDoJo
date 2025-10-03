using ToDoJo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoJo.Services
{
    public interface INavigationService
    {
        Task NavigateTo<T>() where T : ViewModelBase;
    }
}
