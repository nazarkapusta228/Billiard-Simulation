using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BilliardSimulation.Data
{
    public class Ball : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _velocityX;
        private double _velocityY;
        private double _radius = 15;

        public double X
        {
            get => _x;
            set
            {
                if (SetField(ref _x, value))
                {
                    // Left depends on X
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Left)));
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (SetField(ref _y, value))
                {
                    // Top depends on Y
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Top)));
                }
            }
        }

        public double VelocityX
        {
            get => _velocityX;
            set => SetField(ref _velocityX, value);
        }

        public double VelocityY
        {
            get => _velocityY;
            set => SetField(ref _velocityY, value);
        }

        public double Radius
        {
            get => _radius;
            set
            {
                if (SetField(ref _radius, value))
                {
                    // Left/Top depend on Radius
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Left)));
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Top)));
                }
            }
        }

        public double Left => X - Radius;
        public double Top => Y - Radius;

        public Ball(double x, double y, double vx, double vy)
        {
            _x = x;
            _y = y;
            _velocityX = vx;
            _velocityY = vy;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
