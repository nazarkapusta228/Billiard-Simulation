using BilliardSimulation.Data;
using System.Collections.Generic;

namespace BilliardSimulation.Logic
{
    public interface IBallLogic
    {
       
        void CreateRandomBalls(int count, double tableWidth, double tableHeight);

        
        void UpdatePositions(IEnumerable<Ball> balls, double tableWidth, double tableHeight);
    }
}