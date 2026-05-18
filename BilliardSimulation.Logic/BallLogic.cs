using BilliardSimulation.Data;
using BilliardSimulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilliardSimulation.Logic
{
    public class BallLogic : IBallLogic
    {
        private readonly IBallRepository _repository;
        private readonly IDiagnosticLogger _logger;
        private readonly CollisionDetector _collisionDetector = new CollisionDetector();
        private readonly object _creationLock = new object();

        public event EventHandler<SimulationUpdateEventArgs> SimulationUpdated;

        public BallLogic(IBallRepository repository, IDiagnosticLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void CreateRandomBalls(int count, double tableWidth, double tableHeight)
        {
            lock (_creationLock)
            {
                _repository.ClearAllBalls();

                var rnd = new Random();

                for (int i = 0; i < count; i++)
                {
                    double r = Ball.DefaultRadius;

                    double x = r + rnd.NextDouble() * (tableWidth - 2 * r);
                    double y = r + rnd.NextDouble() * (tableHeight - 2 * r);

                    double vx = (rnd.NextDouble() - 0.5) * 190;
                    double vy = (rnd.NextDouble() - 0.5) * 190;

                    _repository.AddBall(new Ball(x, y, vx, vy, r, Ball.DefaultMass));
                }
            }
        }

        public void UpdatePositions(IEnumerable<Ball> balls, double tableWidth, double tableHeight)
        {
            var list = balls.ToList();

            double dt = 0.016; // ~60 FPS (fixed timestep)

            // 1. рух
            Parallel.ForEach(list, ball =>
            {
                ball.ApplyVelocityStep(dt);
            });

            // 2. стінки
            Parallel.ForEach(list, ball =>
            {
                _collisionDetector.ResolveWallCollisions(ball, tableWidth, tableHeight);
            });

            // 3. колізії між кулями
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    _collisionDetector.TryResolveBallCollision(list[i], list[j]);
                }
            }

            // 4. лог
            foreach (var ball in list)
            {
                _logger.LogBallState(ball);
            }
        }

        public async Task UpdatePositionsAsync(double tableWidth, double tableHeight)
        {
            var balls = _repository.GetAllBalls();

            await Task.Run(() => UpdatePositions(balls, tableWidth, tableHeight));

            SimulationUpdated?.Invoke(this,
                new SimulationUpdateEventArgs
                {
                    UpdatedBalls = GetBallModels()
                });
        }

        public IReadOnlyList<BallModel> GetBallModels()
        {
            return _repository.GetAllBalls()
                .Select(BallToModel)
                .ToList()
                .AsReadOnly();
        }

        private BallModel BallToModel(Ball ball)
        {
            ball.GetState(out double x, out double y, out double vx, out double vy, out double r, out double m);
            return new BallModel(x, y, r, vx, vy, m);
        }
    }
}