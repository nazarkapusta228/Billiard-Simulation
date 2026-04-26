using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BilliardSimulation.Data
{
    public class Ball : INotifyPropertyChanged
    {
        public const double DefaultRadius = 15;
        public const double DefaultMass = 5;

        private double _x;
        private double _y;
        private double _velocityX;
        private double _velocityY;
        private double _radius = DefaultRadius;
        private double _mass = DefaultMass;
        private readonly object _lockObject = new object();

        public double X
        {
            get
            {
                lock (_lockObject)
                {
                    return _x;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    if (SetField(ref _x, value))
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left)));
                    }
                }
            }
        }

        public double Y
        {
            get
            {
                lock (_lockObject)
                {
                    return _y;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    if (SetField(ref _y, value))
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Top)));
                    }
                }
            }
        }

        public double VelocityX
        {
            get
            {
                lock (_lockObject)
                {
                    return _velocityX;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    SetField(ref _velocityX, value);
                }
            }
        }

        public double VelocityY
        {
            get
            {
                lock (_lockObject)
                {
                    return _velocityY;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    SetField(ref _velocityY, value);
                }
            }
        }

        public double Radius
        {
            get => _radius;
            set
            {
                if (SetField(ref _radius, value))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Top)));
                }
            }
        }

        public double Mass
        {
            get => _mass;
            set => SetField(ref _mass, value);
        }

        public double Left => X - Radius;
        public double Top => Y - Radius;

        public Ball(double x, double y, double vx, double vy, double radius = DefaultRadius, double mass = DefaultMass)
        {
            _x = x;
            _y = y;
            _velocityX = vx;
            _velocityY = vy;
            _radius = radius;
            _mass = mass;
        }

        // Method to get snapshot of ball state for collision detection (thread-safe)
        public void GetState(out double x, out double y, out double vx, out double vy, out double radius, out double mass)
        {
            lock (_lockObject)
            {
                x = _x;
                y = _y;
                vx = _velocityX;
                vy = _velocityY;
                radius = _radius;
                mass = _mass;
            }
        }

        // Method to set new velocities after collision (thread-safe)
        public void SetVelocities(double vx, double vy)
        {
            lock (_lockObject)
            {
                _velocityX = vx;
                _velocityY = vy;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VelocityX)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VelocityY)));
            }
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

