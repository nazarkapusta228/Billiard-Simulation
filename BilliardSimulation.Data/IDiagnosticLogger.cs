using System.Threading.Tasks;

namespace BilliardSimulation.Data
{
    public interface IDiagnosticLogger
    {
        void LogBallState(Ball ball);
        Task StopAsync();
    }
}