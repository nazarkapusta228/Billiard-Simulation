using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BilliardSimulation.Model
{
    public class BallModel : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _velocityX;
        private double _velocityY;
        private readonly double _radius;

        public double X
        {
            get => _x;
            set
            {
                if (SetField(ref _x, value))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Top)));
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

        public double Radius => _radius;

        public double Left => X - Radius;
        public double Top => Y - Radius;

        public event PropertyChangedEventHandler PropertyChanged;

        public BallModel(double x, double y, double radius, double velocityX = 0, double velocityY = 0)
        {
            _x = x;
            _y = y;
            _radius = radius;
            _velocityX = velocityX;
            _velocityY = velocityY;
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
