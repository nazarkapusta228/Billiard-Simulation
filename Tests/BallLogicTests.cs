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
            double velocityX = 5;
            double velocityY = 0;
            var ball = new Ball(90, 25, velocityX, velocityY, radius: 10);

            var list = new List<Ball> { ball };
            logic.UpdatePositions(list, width, height);

            ball.GetState(out double x, out double y, out double vx, out double vy, out double radius, out double _);

            Assert.AreEqual(width - radius - 1.0, x);
            Assert.IsTrue(vx < 0);
        }

        [TestMethod]
        public void UpdatePositions_BouncesOffLeftWall()
        {
            var logic = new BallLogic(null);
            double width = 100;
            double height = 50;
            double velocityX = -10;
            double velocityY = 0;
            var ball = new Ball(5, 25, velocityX, velocityY, radius: 10);

            var list = new List<Ball> { ball };
            logic.UpdatePositions(list, width, height);

            ball.GetState(out double x, out double y, out double vx, out double vy, out double radius, out double _);

            Assert.AreEqual(radius + 1.0, x);
            Assert.IsTrue(vx > 0);
        }

        [TestMethod]
        public void UpdatePositions_BouncesOffTopWall()
        {
            var logic = new BallLogic(null);
            double width = 100;
            double height = 50;
            double velocityX = 0;
            double velocityY = -8;
            var ballTop = new Ball(50, 5, velocityX, velocityY, radius: 10);

            logic.UpdatePositions(new List<Ball> { ballTop }, width, height);

            ballTop.GetState(out double x, out double y, out double vx, out double vy, out double radius, out double _);

            Assert.AreEqual(radius + 1.0, y);
            Assert.IsTrue(vy > 0);
        }

        [TestMethod]
        public void UpdatePositions_BouncesOffBottomWall()
        {
            var logic = new BallLogic(null);
            double width = 100;
            double height = 50;
            double velocityX = 0;
            double velocityY = 8;
            var ballBottom = new Ball(50, 45, velocityX, velocityY, radius: 10);

            logic.UpdatePositions(new List<Ball> { ballBottom }, width, height);

            ballBottom.GetState(out double x, out double y, out double vx, out double vy, out double radius, out double _);

            Assert.AreEqual(height - radius - 1.0, y);
            Assert.IsTrue(vy < 0);
        }
    }}
