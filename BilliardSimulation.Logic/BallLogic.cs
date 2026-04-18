using BilliardSimulation.Data;
using BilliardSimulation.Model;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace BilliardSimulation.Logic
{
    public class BallLogic : IBallLogic
    {
        private readonly IBallRepository _repository;
        private readonly Random _random = new Random();
        private const double visualMargin = 1.0;


        public BallLogic(IBallRepository repository)
        {
            _repository = repository;
        }

        public void CreateRandomBalls(int count, double tableWidth, double tableHeight)
        {
            
            _repository.ClearAllBalls();

            
            for (int i = 0; i < count; i++)
            {
                double radius = 15; 

                // use NextDouble to avoid clustering on integer boundaries
                double x = radius + _random.NextDouble() * Math.Max(0, (tableWidth - 2 * radius));
                double y = radius + _random.NextDouble() * Math.Max(0, (tableHeight - 2 * radius));

               
                // set reasonable initial speeds (pixels per tick)
                double speedFactor = 6.0;
                double vx = (_random.NextDouble() - 0.5) * speedFactor;
                double vy = (_random.NextDouble() - 0.5) * speedFactor;

               
                var ball = new Ball(x, y, vx, vy);
                ball.Radius = radius;

                
                _repository.AddBall(ball);
            }
            // Diagnostic log
            Debug.WriteLine($"[BallLogic] Created {count} balls for table {tableWidth}x{tableHeight}");
        }

        public void UpdatePositions(IEnumerable<Ball> balls, double tableWidth, double tableHeight)
        {
            foreach (var ball in balls)
            {

                ball.X += ball.VelocityX;
                ball.Y += ball.VelocityY;

                // Перевіряємо зіткнення зі стінами (з урахуванням радіуса та visualMargin)

                if (ball.X - ball.Radius <= visualMargin)
                {
                    ball.X = ball.Radius + visualMargin;
                    ball.VelocityX = -ball.VelocityX;
                }

                if (ball.X + ball.Radius >= tableWidth - visualMargin)
                {
                    ball.X = tableWidth - ball.Radius - visualMargin;
                    ball.VelocityX = -ball.VelocityX;
                }

                if (ball.Y - ball.Radius <= visualMargin)
                {
                    ball.Y = ball.Radius + visualMargin;
                    ball.VelocityY = -ball.VelocityY;
                }

                if (ball.Y + ball.Radius >= tableHeight - visualMargin)
                {
                    ball.Y = tableHeight - ball.Radius - visualMargin;
                    ball.VelocityY = -ball.VelocityY;
                }

                // Final safety clamp to ensure positions stay within bounds
                ball.X = Math.Max(ball.Radius + visualMargin, Math.Min(ball.X, tableWidth - ball.Radius - visualMargin));
                ball.Y = Math.Max(ball.Radius + visualMargin, Math.Min(ball.Y, tableHeight - ball.Radius - visualMargin));
            }
        }

        public IReadOnlyList<BallModel> GetBallModels()
        {
            var ballList = _repository?.GetAllBalls() ?? new List<Ball>();
            return ballList.Select(BallToModel).ToList().AsReadOnly();
        }

        public void UpdatePositionsForTable(double tableWidth, double tableHeight)
        {
            var balls = _repository?.GetAllBalls() ?? new List<Ball>();
            UpdatePositions(balls, tableWidth, tableHeight);
        }

        private BallModel BallToModel(Ball ball)
        {
            return new BallModel(ball.X, ball.Y, ball.Radius, ball.VelocityX, ball.VelocityY);
        }
    }
}