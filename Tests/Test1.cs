using BilliardSimulation.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BilliardSimulation.Tests
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void Ball_Created_WithCorrectCoordinates()
        {
            // Arrange
            double expectedX = 100;
            double expectedY = 200;

            // Act
            var ball = new Ball(expectedX, expectedY);

            // Assert
            Assert.AreEqual(expectedX, ball.X);
            Assert.AreEqual(expectedY, ball.Y);
        }
    }
}