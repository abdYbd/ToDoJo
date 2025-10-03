using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ToDoJo.Data;
using ToDoJo.Models;
using ToDoJo.Services;


namespace ToDoJo.ViewModels
{
    public partial class ToDoViewModel : NavigateForViewModel
    {
        private readonly AppDBContext _context;
        private readonly IAuthenticationService _authenticationService;


        public ToDoViewModel(INavigationService navigationService, IAuthenticationService authenticationService, AppDBContext context)
            : base(navigationService, authenticationService) { 
            _context = context;
            _authenticationService = authenticationService;


            _ = LoadTaskFromDatabase();
        }

        [ObservableProperty]
        public int _completeTask;

        [ObservableProperty] 
        public int _noCompleteTask;

        [ObservableProperty]
        public bool _isCompleted;

        [ObservableProperty]
        public string? _descTask;

        [ObservableProperty]
        public DateTime? _deadlineDate;

        [ObservableProperty]
        public TimeSpan? _deadlineTime;

        [ObservableProperty]
        public DateTime _createdAt;

        public ObservableCollection<TodoTask> TodoTask { get; } = new ObservableCollection<TodoTask>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddTaskCommand))]
        private string? _newTaskContent;

        private bool CanAddTask() => !string.IsNullOrWhiteSpace(NewTaskContent) && _authenticationService?.CurrentUser != null;

        [RelayCommand(CanExecute = nameof(CanAddTask))]
        private async Task AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskContent)) return;

            if (_authenticationService == null)
            {
                Debug.WriteLine("AddTask: _authenticationService == null");
                return;
            }

            var currentUser = _authenticationService.CurrentUser;
            if (currentUser == null)
            {
                Debug.WriteLine("AddTask: CurrentUser == null — пользователь не авторизован");
                return;
            }

            if (_context == null)
            {
                Debug.WriteLine("AddTask: _context == null");
                return;
            }

            try
            {
                var newTask = new TodoTask
                {
                    DescTask = NewTaskContent,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    Deadline = GetDeadlineForStorage(),
                    UserId = currentUser.Id
                };

                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync();

                TodoTask.Add(newTask);
                NewTaskContent = null;

                NoCompleteTask++;
                // после изменения NewTaskContent, команда AddTaskCommand выполнит NotifyCanExecuteChanged автоматически
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка добавления задачи: {ex}");
            }
        }

        private async Task LoadTaskFromDatabase()
        {
            if (_context == null)
            {
                Debug.WriteLine("LoadTaskFromDatabaseAsync: _context == null");
                return;
            }
            if (_authenticationService == null)
            {
                Debug.WriteLine("LoadTaskFromDatabaseAsync: _authenticationService == null");
                return;
            }

            var userId = _authenticationService.CurrentUser?.Id;
            if (userId == null)
            {
                Debug.WriteLine("LoadTaskFromDatabaseAsync: CurrentUser == null — пропускаем загрузку");
                return;
            }

            try
            {
                // теперь используем локальную переменную userId — EF нормально переведёт условие
                var tasks = await _context.Tasks
                    .Where(t => t.UserId == userId && !t.IsCompleted)
                    .OrderBy(t => t.CreatedAt)
                    .ToListAsync();

                TodoTask.Clear();
                foreach (var task in tasks) 
                    TodoTask.Add(task);


                NoCompleteTask = tasks.Count;
                CompleteTask = await _context.Tasks.CountAsync(t => t.UserId == userId && t.IsCompleted);
                Debug.WriteLine($"LoadTaskFromDatabaseAsync: загружено {tasks.Count} задач для user {userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadTaskFromDatabaseAsync error: {ex}");
            }
        }
        [RelayCommand]
        public async Task CompleteTaskCommand(TodoTask todoTask)
        {
            if(TodoTask == null) return;

            try
            {
                todoTask.IsCompleted = true;
                _context.Tasks.Update(todoTask);
                await _context.SaveChangesAsync();

                TodoTask.Remove(todoTask);

                CompleteTask++;
                NoCompleteTask--;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"{ex.Message}");
            }
        }

        [RelayCommand]
        private async Task DeleteTask(TodoTask todoTask)
        {
            if(TodoTask == null) return;
            try
            {
                if (_context != null)
                {
                    _context.Tasks.Remove(todoTask);
                    await _context.SaveChangesAsync();
                }
                TodoTask.Remove(todoTask);
                if(!todoTask.IsCompleted)
                    NoCompleteTask--;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private DateTime? GetDeadlineForStorage()
        {
            if(!_deadlineDate.HasValue) return null;
            

            var chosenTime = _deadlineTime ?? new TimeSpan(23,59,00);

            var localDateTime = _deadlineDate.Value + chosenTime;

            var offset = TimeZoneInfo.Local.GetUtcOffset(localDateTime);
            var localDto = new DateTimeOffset(localDateTime, offset);

            return localDto.UtcDateTime;
        }
        [RelayCommand]
        private void ClearDateTime()
        {
            DeadlineDate = null;
            DeadlineTime = null;
        }
    }
}