using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoJo.Services;

namespace ToDoJo.ViewModels
{
    public partial class SamuraiViewModel : NavigateForViewModel
    {
        [ObservableProperty]
        private IImage? _currentFrame;

        [ObservableProperty]
        private string _loadingText = "Coming soon";

        [ObservableProperty]
        private bool _isAnimating = false;

        private readonly List<Bitmap> _animationFrames = new();
        private int _currentFrameIndex = 0;
        private int _dotCount = 0;
        private bool _samuraiLoopRunning = false;
        private bool _loadingLoopRunning = false;

        public SamuraiViewModel(INavigationService navigationService, IAuthenticationService authenticationService)
            : base(navigationService, authenticationService)
        {
            LoadFrames();
            StartAnimation();
        }

        public SamuraiViewModel() : this(null!, null!) { }

        private async void LoadFrames()
        {
            try
            {
                var framePaths = new List<string>()
                {
                    "/Assets/Samurai-idle1.png",
                    "/Assets/Samurai-idle2.png",
                    "/Assets/Samurai-idle3.png",
                    "/Assets/Samurai-idle4.png",
                    "/Assets/Samurai-idle5.png",
                    "/Assets/Samurai-idle6.png"
                };

                foreach (var path in framePaths)
                {
                    var bitmap = await LoadBitmap(path);
                    if (bitmap != null)
                    {
                        _animationFrames.Add(bitmap);
                    }
                }

                // Устанавливаем первый кадр
                if (_animationFrames.Count > 0)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        CurrentFrame = _animationFrames[0];
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading frames: {ex.Message}");
            }
        }

        private async Task<Bitmap?> LoadBitmap(string path)
        {
            try
            {
                // Убираем начальный слэш для использования в Uri
                path = path.TrimStart('/');
                var uri = new Uri($"avares://ToDoJo/{path}");

                // Используем AssetLoader.Open для загрузки ресурса
                var stream = AssetLoader.Open(uri);
                return new Bitmap(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {path}: {ex.Message}");

                // Пробуем загрузить из файловой системы как fallback
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        return new Bitmap(path);
                    }
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"Also failed to load from filesystem: {ex2.Message}");
                }

                return null;
            }
        }

        private async Task SamuraiAnimationLoop()
        {
            if (_samuraiLoopRunning || _animationFrames.Count == 0) return;
            _samuraiLoopRunning = true;

            try
            {
                while (IsAnimating)
                {
                    await Task.Delay(150);

                    _currentFrameIndex = (_currentFrameIndex + 1) % _animationFrames.Count;

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        CurrentFrame = _animationFrames[_currentFrameIndex];
                    });

                    if (_currentFrameIndex == 0)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            finally
            {
                _samuraiLoopRunning = false;
            }
        }

        private async Task LoadingAnimationLoop()
        {
            if (_loadingLoopRunning) return;
            _loadingLoopRunning = true;

            try
            {
                while (IsAnimating)
                {
                    await Task.Delay(500);

                    _dotCount = (_dotCount + 1) % 4;
                    var text = "Coming soon" + new string('.', _dotCount);

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        LoadingText = text;
                    });
                }
            }
            finally
            {
                _loadingLoopRunning = false;
            }
        }

        [RelayCommand]
        private void StopAnimation()
        {
            IsAnimating = false;
        }

        [RelayCommand]
        private void StartAnimation()
        {
            if (IsAnimating || _animationFrames.Count == 0) return;

            IsAnimating = true;
            _ = SamuraiAnimationLoop();
            _ = LoadingAnimationLoop();
        }

        //protected override void OnDeactivated()
        //{
        //    IsAnimating = false;

        //    // Очищаем ресурсы
        //    foreach (var frame in _animationFrames)
        //    {
        //        frame.Dispose();
        //    }
        //    _animationFrames.Clear();

        //    base.OnDeactivated();
        //}
    }
}