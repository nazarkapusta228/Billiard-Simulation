using System.Collections.Generic;

namespace BilliardSimulation.Data
{
    public interface IBallRepository
    {
        IReadOnlyList<Ball> GetAllBalls();
        void AddBall(Ball ball);
        void ClearAllBalls();
        void UpdateBall(Ball ball);
    }
}