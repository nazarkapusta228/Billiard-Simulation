using System;

namespace BilliardSimulation.Data
{
    public class Ball
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Tag { get; set; }
        public Ball(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}