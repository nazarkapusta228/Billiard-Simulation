using Microsoft.VisualStudio.TestTools.UnitTesting;
using BilliardSimulation.Logic;
using BilliardSimulation.Data;
using System.Collections.Generic;
using System.Linq;

namespace BilliardSimulation.Tests
{
    [TestClass]
    public class BallLogicTests
    {
        private class TestRepository : IBallRepository
        {
            private readonly List<Ball> _balls = new List<Ball>();
            public void AddBall(Ball ball) => _balls.Add(ball);
            public void ClearAllBalls() => _balls.Clear();
            public IReadOnlyList<Ball> GetAllBalls() => _balls;
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

        [TestMethod]
        public void CreateRandomBalls_CreatesCorrectNumber_WithinBounds()
        {
            var repo = new TestRepository();
            var logic = new BallLogic(repo);

            double width = 200;
            double height = 100;
            int count = 10;

            logic.CreateRandomBalls(count, width, height);

            var balls = repo.GetAllBalls();
            Assert.AreEqual(count, balls.Count);
            foreach (var b in balls)
            {
                Assert.IsTrue(b.X >= b.Radius && b.X <= width - b.Radius);
                Assert.IsTrue(b.Y >= b.Radius && b.Y <= height - b.Radius);
            }
        }

        [TestMethod]
        public void UpdatePositions_BouncesOffRightWall()
        {
            var logic = new BallLogic(null);
            double width = 100;
            double height = 50;
            var ball = new Ball(90, 25, 5, 0); // will move beyond right wall
            ball.Radius = 10;

            var list = new List<Ball> { ball };
            logic.UpdatePositions(list, width, height);

            // After update, ball should be placed at width - radius - visualMargin (1.0) and velocityX inverted
            Assert.AreEqual(width - ball.Radius - 1.0, ball.X);
            Assert.AreEqual(-5, ball.VelocityX);
        }

        [TestMethod]
        public void UpdatePositions_BouncesOffLeftWall()
        {
            var logic = new BallLogic(null);
            double width = 100;
            double height = 50;
            var ball = new Ball(5, 25, -10, 0);
            ball.Radius = 10;

            var list = new List<Ball> { ball };
            logic.UpdatePositions(list, width, height);

            Assert.AreEqual(ball.Radius + 1.0, ball.X);
            Assert.AreEqual(10, ball.VelocityX);
        }

        [TestMethod]
        public void UpdatePositions_BouncesOffTopAndBottom()
        {
            var logic = new BallLogic(null);
            double width = 100;
            double height = 50;

            var ballTop = new Ball(50, 5, 0, -8) { Radius = 10 };
            var ballBottom = new Ball(50, 45, 0, 8) { Radius = 10 };

            logic.UpdatePositions(new List<Ball> { ballTop }, width, height);
            logic.UpdatePositions(new List<Ball> { ballBottom }, width, height);

            Assert.AreEqual(ballTop.Radius + 1.0, ballTop.Y);
            Assert.AreEqual(8, ballTop.VelocityY);

            Assert.AreEqual(height - ballBottom.Radius - 1.0, ballBottom.Y);
            Assert.AreEqual(-8, ballBottom.VelocityY);
        }
    }
}
