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
        private readonly ILoggerService _logger;
        private int _currentFrameIndex = 0;
        private int _dotCount = 0;
        private bool _samuraiLoopRunning = false;
        private bool _loadingLoopRunning = false;

        public SamuraiViewModel(INavigationService navigationService,
                              IAuthenticationService authenticationService,
                              ILoggerService loggerService)
            : base(navigationService, authenticationService, loggerService)
        {
            _logger = loggerService;
            _logger.Info("SamuraiViewModel initialized");
            LoadFrames();
            StartAnimation();
        }

        public SamuraiViewModel() : this(null!, null!, null!) { }

        private async void LoadFrames()
        {
            try
            {
                _logger.Info("Loading samurai animation frames");

                var framePaths = new List<string>()
                {
                    "/Assets/Samurai-idle1.png",
                    "/Assets/Samurai-idle2.png",
                    "/Assets/Samurai-idle3.png",
                    "/Assets/Samurai-idle4.png",
                    "/Assets/Samurai-idle5.png",
                    "/Assets/Samurai-idle6.png"
                };

                int loadedFrames = 0;
                foreach (var path in framePaths)
                {
                    var bitmap = await LoadBitmap(path);
                    if (bitmap != null)
                    {
                        _animationFrames.Add(bitmap);
                        loadedFrames++;
                    }
                }

                _logger.Info("Loaded {LoadedFrames}/{TotalFrames} animation frames", loadedFrames, framePaths.Count);

                if (_animationFrames.Count > 0)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        CurrentFrame = _animationFrames[0];
                    });
                    _logger.Info("Set initial animation frame");
                }
                else
                {
                    _logger.Warn("No animation frames were loaded successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load animation frames");
            }
        }

        private async Task<Bitmap?> LoadBitmap(string path)
        {
            try
            {
                path = path.TrimStart('/');
                var uri = new Uri($"avares://ToDoJo/{path}");

                var stream = AssetLoader.Open(uri);
                _logger.Info("Successfully loaded bitmap: {Path}", path);
                return new Bitmap(stream);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load bitmap from resources: {Path}", path);

                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        _logger.Info("Trying filesystem fallback for: {Path}", path);
                        return new Bitmap(path);
                    }
                }
                catch (Exception ex2)
                {
                    _logger.Error(ex2, "Also failed to load from filesystem: {Path}", path);
                }

                return null;
            }
        }

        private async Task SamuraiAnimationLoop()
        {
            if (_samuraiLoopRunning || _animationFrames.Count == 0)
            {
                _logger.Info("Samurai animation loop not started - already running or no frames");
                return;
            }

            _samuraiLoopRunning = true;
            _logger.Info("Samurai animation loop started");

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
                _logger.Info("Samurai animation loop stopped");
            }
        }

        private async Task LoadingAnimationLoop()
        {
            if (_loadingLoopRunning)
            {
                _logger.Info("Loading animation loop already running");
                return;
            }

            _loadingLoopRunning = true;
            _logger.Info("Loading animation loop started");

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
                _logger.Info("Loading animation loop stopped");
            }
        }

        [RelayCommand]
        private void StopAnimation()
        {
            _logger.Info("Stopping samurai animation");
            IsAnimating = false;
        }

        [RelayCommand]
        private void StartAnimation()
        {
            if (IsAnimating || _animationFrames.Count == 0)
            {
                _logger.Info("Animation not started - already running or no frames available");
                return;
            }

            _logger.Info("Starting samurai animation");
            IsAnimating = true;
            _ = SamuraiAnimationLoop();
            _ = LoadingAnimationLoop();
        }
    }
}