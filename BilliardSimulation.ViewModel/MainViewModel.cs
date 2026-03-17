using BilliardSimulation.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BilliardSimulation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Ball> Balls { get; set; }

        public MainViewModel()
        {
            Balls = new ObservableCollection<Ball>();

            // Розміщуємо кулі ДАЛЕКО одна від одної
            var ball1 = new Ball(100, 100);
            ball1.Tag = "1";  // Номер кульки

            var ball2 = new Ball(300, 200);
            ball2.Tag = "2";

            var ball3 = new Ball(500, 300);
            ball3.Tag = "3";

            Balls.Add(ball1);
            Balls.Add(ball2);
            Balls.Add(ball3);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}