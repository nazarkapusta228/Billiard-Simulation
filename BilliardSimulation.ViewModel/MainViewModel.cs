using BilliardSimulation.Data;
using BilliardSimulation.Logic;
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
        private readonly IBallRepository _repository;
        private readonly IBallLogic _logic;
        private readonly DispatcherTimer _timer;
        private int _tickCount = 0;

        private int _ballCount = 5;
        private double _tableWidth = 780;
        private double _tableHeight = 580;
        private ObservableCollection<Ball> _balls;

        public ObservableCollection<Ball> Balls
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

        public MainViewModel(IBallRepository repository, IBallLogic logic)
        {
            _repository = repository;
            _logic = logic;

            Balls = new ObservableCollection<Ball>();

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
            var balls = _repository.GetAllBalls();
            Debug.WriteLine($"[MainViewModel] CreateBalls called. Repository returned {balls.Count} balls. ObservableCollection Count={Balls.Count}");
            for (int i = 0; i < balls.Count; i++)
            {
                Debug.WriteLine($"[MainViewModel] Ball {i}: X={balls[i].X:0.##}, Y={balls[i].Y:0.##}, Vx={balls[i].VelocityX:0.##}, Vy={balls[i].VelocityY:0.##}");
            }
        }

        private void UpdateBallsCollection()
        {
            Balls.Clear();
            foreach (var ball in _repository.GetAllBalls())
            {
                Balls.Add(ball);
            }
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            
            var balls = _repository.GetAllBalls();
            _logic.UpdatePositions(balls, _tableWidth, _tableHeight);

            
            
            _tickCount++;
            if (_tickCount % 30 == 0)
            {
                if (balls.Count > 0)
                {
                    Debug.WriteLine($"[MainViewModel] Tick {_tickCount}: First ball pos X={balls[0].X:0.##}, Y={balls[0].Y:0.##}, Vx={balls[0].VelocityX:0.##}, Vy={balls[0].VelocityY:0.##}");
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