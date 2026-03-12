using System;
using System.Collections.Generic;
using System.Text;

namespace BilliardSimulation.Data
{
    public class Ball
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public double Radius { get; set; } = 10; // Default radius

        public Ball(double x, double y, double velocityX = 0, double velocityY = 0)
        {
            X = x;
            Y = y;
            VelocityX = velocityX;
            VelocityY = velocityY;
        }
    }
}
