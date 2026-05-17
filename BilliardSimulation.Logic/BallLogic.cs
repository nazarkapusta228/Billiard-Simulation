using BilliardSimulation.Data;
using BilliardSimulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BilliardSimulation.Logic
{
    public class BallLogic : IBallLogic
    {
        private readonly IBallRepository _repository;
        private readonly IDiagnosticLogger _logger;
        private readonly Random _random = new Random();
        private readonly CollisionDetector _collisionDetector = new CollisionDetector();
        private readonly object _creationLock = new object();  // тільки для створення куль

        public event EventHandler<SimulationUpdateEventArgs> SimulationUpdated;


        public BallLogic(
            IBallRepository repository,
            IDiagnosticLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }


        public void CreateRandomBalls(int count, double tableWidth, double tableHeight)
        {
            lock (_creationLock)
            {
                _repository.ClearAllBalls();

                for (int i = 0; i < count; i++)
                {
                    double radius = Ball.DefaultRadius;
                    double mass = Ball.DefaultMass;

                    double x = radius + _random.NextDouble() * Math.Max(0, (tableWidth - 2 * radius));
                    double y = radius + _random.NextDouble() * Math.Max(0, (tableHeight - 2 * radius));

                    double speedFactor = 4.0;
                    double vx = (_random.NextDouble() - 0.5) * speedFactor;
                    double vy = (_random.NextDouble() - 0.5) * speedFactor;

                    var ball = new Ball(x, y, vx, vy, radius, mass);
                    _repository.AddBall(ball);
                }

                Debug.WriteLine($"[BallLogic] Created {count} balls for table {tableWidth}x{tableHeight}");
            }
        }

        public void UpdatePositions(IEnumerable<Ball> balls, double tableWidth, double tableHeight)
        {
            var ballList = balls.ToList();

            Parallel.ForEach(ballList, ball =>
            {
                ball.GetState(out double x, out double y, out double vx, out double vy, out _, out _);
                ball.X = x + vx;
                ball.Y = y + vy;
            });

            Parallel.ForEach(ballList, ball =>
            {
                _collisionDetector.ResolveWallCollisions(ball, tableWidth, tableHeight);
            });

            for (int i = 0; i < ballList.Count; i++)
            {
                for (int j = i + 1; j < ballList.Count; j++)
                {
                    _collisionDetector.TryResolveBallCollision(ballList[i], ballList[j]);
                }
            }


            //3 etap
            foreach (var ball in ballList)
            {
                _logger.LogBallState(ball);
            }
        }

        public async Task UpdatePositionsAsync(double tableWidth, double tableHeight)
        {
            var balls = _repository?.GetAllBalls() ?? new List<Ball>();

            await Task.Run(() => UpdatePositions(balls, tableWidth, tableHeight));

            var models = GetBallModels();
            SimulationUpdated?.Invoke(this, new SimulationUpdateEventArgs { UpdatedBalls = models });
        }

        public IReadOnlyList<BallModel> GetBallModels()
        {
            var ballList = _repository?.GetAllBalls() ?? new List<Ball>();
            return ballList.Select(BallToModel).ToList().AsReadOnly();
        }

        private BallModel BallToModel(Ball ball)
        {
            ball.GetState(out double x, out double y, out double vx, out double vy, out double radius, out double mass);
            return new BallModel(x, y, radius, vx, vy, mass);
        }
    }
}