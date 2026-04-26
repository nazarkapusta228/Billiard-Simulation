using BilliardSimulation.Logic;
using BilliardSimulation.Model;
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BilliardSimulation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IBallLogic _logic;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _simulationTask;
        private bool _isRunning = false;
        private bool _isPaused = false;
        private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);

        private int _ballCount = 5;
        private double _tableWidth = 780;
        private double _tableHeight = 580;
        private ObservableCollection<BallModel> _balls;

        public ObservableCollection<BallModel> Balls
        {
            get => _balls;
            set
            {
                _balls = value;
                OnPropertyChanged();
            }
        }

        public int BallCount
        {
            get => _ballCount;
            set
            {
                _ballCount = value;
                OnPropertyChanged();
            }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                OnPropertyChanged();
                // Update button text based on state
                OnPropertyChanged(nameof(PauseButtonText));
            }
        }

        public string PauseButtonText => _isPaused ? "▶ Resume" : "⏸ Pause";

        public ICommand CreateBallsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand StartSimulationCommand { get; }
        public ICommand StopSimulationCommand { get; }
        public ICommand PauseSimulationCommand { get; }

        public MainViewModel(IBallLogic logic)
        {
            _logic = logic;

            Balls = new ObservableCollection<BallModel>();

            CreateBallsCommand = new RelayCommand(_ => CreateBalls());
            ExitCommand = new RelayCommand(_ => Exit());
            StartSimulationCommand = new RelayCommand(_ => StartSimulation(), _ => !_isRunning);
            StopSimulationCommand = new RelayCommand(_ => StopSimulation(), _ => _isRunning);
            PauseSimulationCommand = new RelayCommand(_ => TogglePause(), _ => _isRunning);

            // Subscribe to simulation updates (reactive programming)
            _logic.SimulationUpdated += OnSimulationUpdated;

            CreateBalls();
        }

        private void CreateBalls()
        {
            _logic.CreateRandomBalls(BallCount, _tableWidth, _tableHeight);
            UpdateBallsCollection();
            var ballModels = _logic.GetBallModels();
            Debug.WriteLine($"[MainViewModel] CreateBalls called. Logic returned {ballModels.Count} balls. ObservableCollection Count={Balls.Count}");
        }

        private void UpdateBallsCollection()
        {
            var ballModels = _logic.GetBallModels();
            if (ballModels.Count != Balls.Count)
            {
                Balls.Clear();
                foreach (var ballModel in ballModels)
                {
                    Balls.Add(ballModel);
                }
            }
            else
            {
                // Update existing items
                for (int i = 0; i < ballModels.Count; i++)
                {
                    Balls[i].X = ballModels[i].X;
                    Balls[i].Y = ballModels[i].Y;
                    Balls[i].VelocityX = ballModels[i].VelocityX;
                    Balls[i].VelocityY = ballModels[i].VelocityY;
                    Balls[i].Mass = ballModels[i].Mass;
                }
            }
        }

        private void OnSimulationUpdated(object sender, SimulationUpdateEventArgs e)
        {
            // This is called from the simulation thread, need to dispatch to UI thread
            var app = System.Windows.Application.Current;
            if (app != null)
            {
                app.Dispatcher.Invoke(() =>
                {
                    UpdateBallsCollection();
                });
            }
        }

        private void StartSimulation()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            IsPaused = false;
            _pauseEvent.Set(); // Ensure pause event is set (not paused)
            _cancellationTokenSource = new CancellationTokenSource();

            _simulationTask = SimulationLoopAsync(_cancellationTokenSource.Token);

            // Update command states
            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged(nameof(StartSimulationCommand));
            OnPropertyChanged(nameof(StopSimulationCommand));
        }

        private async Task SimulationLoopAsync(CancellationToken cancellationToken)
        {
            const int targetFPS = 60;
            const int frameTimeMs = 1000 / targetFPS;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Wait if paused
                    _pauseEvent.Wait(cancellationToken);

                    long elapsedMs = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    // Perform simulation update asynchronously
                    await _logic.UpdatePositionsAsync(_tableWidth, _tableHeight);

                    // Sleep to maintain target FPS
                    long sleepTime = frameTimeMs - elapsedMs;
                    if (sleepTime > 0)
                    {
                        await Task.Delay((int)sleepTime, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
                Debug.WriteLine("[MainViewModel] Simulation cancelled properly");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel] Simulation error: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
                IsPaused = false;
                CommandManager.InvalidateRequerySuggested();
                OnPropertyChanged(nameof(StartSimulationCommand));
                OnPropertyChanged(nameof(StopSimulationCommand));
            }
        }

        private void TogglePause()
        {
            if (!_isRunning)
                return;

            if (_isPaused)
            {
                // Resume
                _pauseEvent.Set();
                IsPaused = false;
                Debug.WriteLine("[MainViewModel] Simulation resumed");
            }
            else
            {
                // Pause
                _pauseEvent.Reset();
                IsPaused = true;
                Debug.WriteLine("[MainViewModel] Simulation paused");
            }
        }

        private void StopSimulation()
        {
            if (_isRunning)
            {
                _cancellationTokenSource?.Cancel();
                _pauseEvent.Set(); // Release pause to allow cancellation to complete
                Debug.WriteLine("[MainViewModel] StopSimulation called - CancellationToken cancelled");
            }
        }

        public void SetTableSize(double width, double height)
        {
            if (width > 0 && height > 0)
            {
                bool wasDefault = _tableWidth == 780 && _tableHeight == 580;
                _tableWidth = width;
                _tableHeight = height;

                if (wasDefault)
                {
                    CreateBalls();
                }
            }
        }

        private void Exit()
        {
            StopSimulation();
            System.Windows.Application.Current?.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}