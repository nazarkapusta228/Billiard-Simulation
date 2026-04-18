using BilliardSimulation.Data;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace BilliardSimulation.Logic
{
    public class BallLogic : IBallLogic
    {
        private readonly IBallRepository _repository;
        private readonly Random _random = new Random();

        
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

                // Перевіряємо зіткнення зі стінами (з урахуванням радіуса)
                
                if (ball.X - ball.Radius <= 0)
                {
                    ball.X = ball.Radius;
                    ball.VelocityX = -ball.VelocityX;
                }
                
                if (ball.X + ball.Radius >= tableWidth)
                {
                    ball.X = tableWidth - ball.Radius;
                    ball.VelocityX = -ball.VelocityX;
                }
                
                if (ball.Y - ball.Radius <= 0)
                {
                    ball.Y = ball.Radius;
                    ball.VelocityY = -ball.VelocityY;
                }
                
                if (ball.Y + ball.Radius >= tableHeight)
                {
                    ball.Y = tableHeight - ball.Radius;
                    ball.VelocityY = -ball.VelocityY;
                }
            }
        }
    }
}