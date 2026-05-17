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
        private double _radius;
        private double _mass;

        private readonly object _lockObject = new object();

        public event PropertyChangedEventHandler PropertyChanged;

        public Ball(double x, double y, double vx, double vy,
                    double radius = DefaultRadius,
                    double mass = DefaultMass)
        {
            _x = x;
            _y = y;
            _velocityX = vx;
            _velocityY = vy;
            _radius = radius;
            _mass = mass;
        }

        // =========================
        // POSITION
        // =========================

        public double X
        {
            get
            {
                lock (_lockObject)
                    return _x;
            }
            set
            {
                bool changed;

                lock (_lockObject)
                {
                    changed = _x != value;
                    _x = value;
                }

                if (changed)
                    OnPropertyChanged(nameof(Left));
            }
        }

        public double Y
        {
            get
            {
                lock (_lockObject)
                    return _y;
            }
            set
            {
                bool changed;

                lock (_lockObject)
                {
                    changed = _y != value;
                    _y = value;
                }

                if (changed)
                    OnPropertyChanged(nameof(Top));
            }
        }

        // =========================
        // VELOCITY
        // =========================

        public double VelocityX
        {
            get
            {
                lock (_lockObject)
                    return _velocityX;
            }
            set
            {
                lock (_lockObject)
                    _velocityX = value;
            }
        }

        public double VelocityY
        {
            get
            {
                lock (_lockObject)
                    return _velocityY;
            }
            set
            {
                lock (_lockObject)
                    _velocityY = value;
            }
        }

        // =========================
        // PHYSICAL PROPERTIES
        // =========================

        public double Radius
        {
            get
            {
                lock (_lockObject)
                    return _radius;
            }
            set
            {
                bool changed;

                lock (_lockObject)
                {
                    changed = _radius != value;
                    _radius = value;
                }

                if (changed)
                {
                    OnPropertyChanged(nameof(Left));
                    OnPropertyChanged(nameof(Top));
                }
            }
        }

        public double Mass
        {
            get
            {
                lock (_lockObject)
                    return _mass;
            }
            set
            {
                lock (_lockObject)
                    _mass = value;
            }
        }

        // =========================
        // DERIVED UI PROPERTIES
        // =========================

        public double Left => X - Radius;
        public double Top => Y - Radius;

        // =========================
        // THREAD SAFE SNAPSHOT
        // =========================

        public void GetState(out double x, out double y,
                             out double vx, out double vy,
                             out double radius, out double mass)
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

        // =========================
        // VELOCITY UPDATE AFTER COLLISION
        // =========================

        public void SetVelocities(double vx, double vy)
        {
            lock (_lockObject)
            {
                _velocityX = vx;
                _velocityY = vy;
            }
        }

        // =========================
        // PROPERTY CHANGED
        // =========================

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}