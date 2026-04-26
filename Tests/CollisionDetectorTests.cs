using Microsoft.VisualStudio.TestTools.UnitTesting;
using BilliardSimulation.Logic;
using BilliardSimulation.Data;
using System;

namespace BilliardSimulation.Tests
{
    [TestClass]
    public class CollisionDetectorTests
    {
        private CollisionDetector _detector;

        [TestInitialize]
        public void Setup()
        {
            _detector = new CollisionDetector();
        }

        [TestMethod]
        public void TryResolveBallCollision_NoCollisionWhenFarApart()
        {
            var ball1 = new Ball(50, 50, 0, 0, radius: 10);
            var ball2 = new Ball(200, 200, 0, 0, radius: 10);

            bool collided = _detector.TryResolveBallCollision(ball1, ball2);

            Assert.IsFalse(collided);
        }

        [TestMethod]
        public void TryResolveBallCollision_NoCollisionWhenMovingApart()
        {
            // Balls touching but moving apart
            var ball1 = new Ball(40, 50, -5, 0, radius: 10);
            var ball2 = new Ball(60, 50, 5, 0, radius: 10);

            bool collided = _detector.TryResolveBallCollision(ball1, ball2);

            Assert.IsFalse(collided);
        }

        [TestMethod]
        public void ResolveWallCollisions_BouncesOffLeftWall()
        {
            var ball = new Ball(5, 50, -10, 0, radius: 10);

            _detector.ResolveWallCollisions(ball, 100, 100);

            ball.GetState(out double x, out double y, out double vx, out double vy, out double _, out double _);

            // Should bounce off left wall
            Assert.IsTrue(x > 5); // Position changed
            Assert.IsTrue(vx > 0); // Velocity reversed (now positive)
        }

        [TestMethod]
        public void ResolveWallCollisions_BouncesOffRightWall()
        {
            var ball = new Ball(95, 50, 10, 0, radius: 10);

            _detector.ResolveWallCollisions(ball, 100, 100);

            ball.GetState(out double x, out double y, out double vx, out double vy, out double _, out double _);

            // Should bounce off right wall
            Assert.IsTrue(x < 95); // Position changed
            Assert.IsTrue(vx < 0); // Velocity reversed (now negative)
        }

        [TestMethod]
        public void ResolveWallCollisions_BouncesOffTopWall()
        {
            var ball = new Ball(50, 5, 0, -10, radius: 10);

            _detector.ResolveWallCollisions(ball, 100, 100);

            ball.GetState(out double x, out double y, out double vx, out double vy, out double _, out double _);

            // Should bounce off top wall
            Assert.IsTrue(y > 5); // Position changed
            Assert.IsTrue(vy > 0); // Velocity reversed (now positive)
        }

        [TestMethod]
        public void ResolveWallCollisions_BouncesOffBottomWall()
        {
            var ball = new Ball(50, 95, 0, 10, radius: 10);

            _detector.ResolveWallCollisions(ball, 100, 100);

            ball.GetState(out double x, out double y, out double vx, out double vy, out double _, out double _);

            // Should bounce off bottom wall
            Assert.IsTrue(y < 95); // Position changed
            Assert.IsTrue(vy < 0); // Velocity reversed (now negative)
        }

        [TestMethod]
        public void ResolveWallCollisions_ClampsBallInBounds()
        {
            // Ball way outside bounds
            var ball = new Ball(-100, -100, 0, 0, radius: 10);

            _detector.ResolveWallCollisions(ball, 100, 100);

            ball.GetState(out double x, out double y, out double _, out double _, out double radius, out double _);

            // Should be clamped inside bounds
            Assert.IsTrue(x >= radius + 1.0);
            Assert.IsTrue(y >= radius + 1.0);
        }
    }
}
