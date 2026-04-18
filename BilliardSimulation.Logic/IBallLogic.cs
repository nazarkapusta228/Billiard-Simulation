using BilliardSimulation.Data;
using BilliardSimulation.Model;
using System.Collections.Generic;

namespace BilliardSimulation.Logic
{
    public interface IBallLogic
    {

        void CreateRandomBalls(int count, double tableWidth, double tableHeight);


        void UpdatePositions(IEnumerable<Ball> balls, double tableWidth, double tableHeight);


        void UpdatePositionsForTable(double tableWidth, double tableHeight);


        IReadOnlyList<BallModel> GetBallModels();
    }
}