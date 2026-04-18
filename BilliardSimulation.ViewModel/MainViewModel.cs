using BilliardSimulation.Logic;
using BilliardSimulation.Model;
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Windows.Input;

namespace BilliardSimulation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IBallLogic _logic;
        private readonly DispatcherTimer _timer;
        private int _tickCount = 0;

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

        public ICommand CreateBallsCommand { get; }
        public ICommand ExitCommand { get; }

        public MainViewModel(IBallLogic logic)
        {
            _logic = logic;

            Balls = new ObservableCollection<BallModel>();

            CreateBallsCommand = new RelayCommand(_ => CreateBalls());
            ExitCommand = new RelayCommand(_ => Exit());

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(16);
            _timer.Tick += OnTimerTick;
            _timer.Start();

            CreateBalls();
        }

        private void CreateBalls()
        {
            _logic.CreateRandomBalls(BallCount, _tableWidth, _tableHeight);
            UpdateBallsCollection();
            var ballModels = _logic.GetBallModels();
            Debug.WriteLine($"[MainViewModel] CreateBalls called. Logic returned {ballModels.Count} balls. ObservableCollection Count={Balls.Count}");
            for (int i = 0; i < ballModels.Count; i++)
            {
                Debug.WriteLine($"[MainViewModel] Ball {i}: X={ballModels[i].X:0.##}, Y={ballModels[i].Y:0.##}, Vx={ballModels[i].VelocityX:0.##}, Vy={ballModels[i].VelocityY:0.##}");
            }
        }

        private void UpdateBallsCollection()
        {
            Balls.Clear();
            foreach (var ballModel in _logic.GetBallModels())
            {
                Balls.Add(ballModel);
            }
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            // Update ball positions through logic
            _logic.UpdatePositionsForTable(_tableWidth, _tableHeight);

            // Refresh the collection with updated positions
            UpdateBallsCollection();

            _tickCount++;
            if (_tickCount % 30 == 0)
            {
                var currentBalls = _logic.GetBallModels();
                if (currentBalls.Count > 0)
                {
                    Debug.WriteLine($"[MainViewModel] Tick {_tickCount}: First ball pos X={currentBalls[0].X:0.##}, Y={currentBalls[0].Y:0.##}, Vx={currentBalls[0].VelocityX:0.##}, Vy={currentBalls[0].VelocityY:0.##}");
                }
                else
                {
                    Debug.WriteLine($"[MainViewModel] Tick {_tickCount}: no balls");
                }
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
            Environment.Exit(0);
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
            add { }
            remove { }
        }
    }
}