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
        private readonly Random _random = new Random();
        private readonly CollisionDetector _collisionDetector = new CollisionDetector();
        private readonly object _lockObject = new object();
        private readonly HashSet<(int, int)> _processedCollisions = new HashSet<(int, int)>();

        public event EventHandler<SimulationUpdateEventArgs> SimulationUpdated;

        public BallLogic(IBallRepository repository)
        {
            _repository = repository;
        }

        public void CreateRandomBalls(int count, double tableWidth, double tableHeight)
        {
            lock (_lockObject)
            {
                _repository.ClearAllBalls();

                for (int i = 0; i < count; i++)
                {
                    double radius = Ball.DefaultRadius;
                    double mass = Ball.DefaultMass;

                    // Random position within bounds
                    double x = radius + _random.NextDouble() * Math.Max(0, (tableWidth - 2 * radius));
                    double y = radius + _random.NextDouble() * Math.Max(0, (tableHeight - 2 * radius));

                    // Random initial velocities
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
            lock (_lockObject)
            {
                var ballList = balls.ToList();

                // Update positions
                foreach (var ball in ballList)
                {
                    ball.GetState(out double x, out double y, out double vx, out double vy, out double _, out double _);
                    ball.X = x + vx;
                    ball.Y = y + vy;
                }

                // Resolve wall collisions
                foreach (var ball in ballList)
                {
                    _collisionDetector.ResolveWallCollisions(ball, tableWidth, tableHeight);
                }

                // Clear collision tracking for this frame
                _processedCollisions.Clear();

                // Resolve ball-to-ball collisions
                for (int i = 0; i < ballList.Count; i++)
                {
                    for (int j = i + 1; j < ballList.Count; j++)
                    {
                        var pairKey = (i, j);
                        // Only process each pair once per frame
                        if (!_processedCollisions.Contains(pairKey))
                        {
                            if (_collisionDetector.TryResolveBallCollision(ballList[i], ballList[j]))
                            {
                                _processedCollisions.Add(pairKey);
                            }
                        }
                    }
                }
            }
        }

        public async Task UpdatePositionsAsync(double tableWidth, double tableHeight)
        {
            var balls = _repository?.GetAllBalls() ?? new List<Ball>();
            await Task.Run(() => UpdatePositions(balls, tableWidth, tableHeight));

            // Notify subscribers about update
            var models = GetBallModels();
            SimulationUpdated?.Invoke(this, new SimulationUpdateEventArgs { UpdatedBalls = models });
        }

        public IReadOnlyList<BallModel> GetBallModels()
        {
            lock (_lockObject)
            {
                var ballList = _repository?.GetAllBalls() ?? new List<Ball>();
                return ballList.Select(BallToModel).ToList().AsReadOnly();
            }
        }

        private BallModel BallToModel(Ball ball)
        {
            ball.GetState(out double x, out double y, out double vx, out double vy, out double radius, out double mass);
            return new BallModel(x, y, radius, vx, vy, mass);
        }
    }
}