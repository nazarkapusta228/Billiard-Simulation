using System.Collections.Generic;
using System.Linq;

namespace BilliardSimulation.Data
{
    public class BallRepository : IBallRepository
    {
        private readonly List<Ball> _balls = new List<Ball>();

        public IReadOnlyList<Ball> GetAllBalls() => _balls.ToList();

        public void AddBall(Ball ball) => _balls.Add(ball);

        public void ClearAllBalls() => _balls.Clear();

        public void UpdateBall(Ball ball)
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