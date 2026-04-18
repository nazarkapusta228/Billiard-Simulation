using Microsoft.VisualStudio.TestTools.UnitTesting;
using BilliardSimulation.Data;
using System.Linq;

namespace BilliardSimulation.Tests
{
    [TestClass]
    public class BallRepositoryTests
    {
        [TestMethod]
        public void AddBall_IncreasesCount()
        {
            var repo = new BallRepository();
            var ball = new Ball(10, 20, 1, 1);

            repo.AddBall(ball);

            var all = repo.GetAllBalls();
            Assert.AreEqual(1, all.Count);
            Assert.AreSame(ball, all[0]);
        }

        [TestMethod]
        public void ClearAllBalls_RemovesAll()
        {
            var repo = new BallRepository();
            repo.AddBall(new Ball(0,0,0,0));
            repo.AddBall(new Ball(1,1,0,0));

            repo.ClearAllBalls();

            Assert.AreEqual(0, repo.GetAllBalls().Count);
        }

        [TestMethod]
        public void UpdateBall_UpdatesExisting_WhenSameInstance()
        {
            var repo = new BallRepository();
            var ball = new Ball(5, 5, 0, 0);
            repo.AddBall(ball);

            // modify and call UpdateBall with same reference
            ball.X = 42;
            ball.Y = 43;
            ball.VelocityX = 2;
            ball.VelocityY = 3;

            repo.UpdateBall(ball);

            var stored = repo.GetAllBalls().First();
            Assert.AreEqual(42, stored.X);
            Assert.AreEqual(43, stored.Y);
            Assert.AreEqual(2, stored.VelocityX);
            Assert.AreEqual(3, stored.VelocityY);
        }
    }
}
