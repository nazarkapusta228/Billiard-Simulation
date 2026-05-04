using System.Collections.Generic;
using System.Linq;

namespace BilliardSimulation.Data
{
    public class BallRepository : IBallRepository
    {
        private readonly List<Ball> _balls = new List<Ball>();
        private readonly object _lock = new object();

        public IReadOnlyList<Ball> GetAllBalls()
        {
            lock (_lock)
            {
                return _balls.ToList();
            }
        }

        public void AddBall(Ball ball)
        {
            lock (_lock)
            {
                _balls.Add(ball);
            }
        }

        public void ClearAllBalls()
        {
            lock (_lock)
            {
                _balls.Clear();
            }
        }

        public void UpdateBall(Ball ball)
        {
            lock (_lock)
            {
                var existing = _balls.FirstOrDefault(b => b == ball);
                if (existing != null)
                {
                    existing.X = ball.X;
                    existing.Y = ball.Y;
                    existing.VelocityX = ball.VelocityX;
                    existing.VelocityY = ball.VelocityY;
                }
            }
        }
    }
}