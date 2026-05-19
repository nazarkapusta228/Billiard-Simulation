using System.ComponentModel;

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

        public double X
        {
            get
            {
                lock (_lockObject)
                    return _x;
            }
            set
            {
                lock (_lockObject)
                    _x = value;
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
                lock (_lockObject)
                    _y = value;
            }
        }

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

        public double Radius
        {
            get
            {
                lock (_lockObject)
                    return _radius;
            }
            set
            {
                lock (_lockObject)
                    _radius = value;
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

        public double Left => X - Radius;
        public double Top => Y - Radius;

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

        public void ApplyVelocityStep(double deltaTime)
        {
            lock (_lockObject)
            {
                _x += _velocityX * deltaTime;
                _y += _velocityY * deltaTime;
            }
        }

        public void SetVelocities(double vx, double vy)
        {
            lock (_lockObject)
            {
                _velocityX = vx;
                _velocityY = vy;
            }
        }
    }
}