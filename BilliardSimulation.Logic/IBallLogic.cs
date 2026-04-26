using BilliardSimulation.Data;
using BilliardSimulation.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BilliardSimulation.Logic
{
    public interface IBallLogic
    {
        void CreateRandomBalls(int count, double tableWidth, double tableHeight);

        void UpdatePositions(IEnumerable<Ball> balls, double tableWidth, double tableHeight);

        Task UpdatePositionsAsync(double tableWidth, double tableHeight);

        IReadOnlyList<BallModel> GetBallModels();

        // Event for when simulation updates
        event EventHandler<SimulationUpdateEventArgs> SimulationUpdated;
    }

    public class SimulationUpdateEventArgs : EventArgs
    {
        public IReadOnlyList<BallModel> UpdatedBalls { get; set; }
    }
}
