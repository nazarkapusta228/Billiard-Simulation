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
        private readonly CollisionDetector _collisionDetector =
            new CollisionDetector();

        private readonly object _creationLock = new object();

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private double _lastTimeSeconds;

        public event EventHandler<SimulationUpdateEventArgs> SimulationUpdated;

        public BallLogic(
            IBallRepository repository,
            IDiagnosticLogger logger)
        {
            _repository = repository;
            _logger = logger;

            _stopwatch.Start();
        }

        public void CreateRandomBalls(
            int count,
            double tableWidth,
            double tableHeight)
        {
            lock (_creationLock)
            {
                _repository.ClearAllBalls();

                Random rnd = new Random();

                for (int i = 0; i < count; i++)
                {
                    double radius = Ball.DefaultRadius;

                    double x =
                        radius +
                        rnd.NextDouble() *
                        (tableWidth - 2 * radius);

                    double y =
                        radius +
                        rnd.NextDouble() *
                        (tableHeight - 2 * radius);

                    double vx =
                        (rnd.NextDouble() - 0.5) * 300;

                    double vy =
                        (rnd.NextDouble() - 0.5) * 300;

                    Ball ball = new Ball(
                        x,
                        y,
                        vx,
                        vy,
                        radius,
                        Ball.DefaultMass);

                    _repository.AddBall(ball);
                }
            }
        }

        public void UpdatePositions(
    IEnumerable<Ball> balls,
    double tableWidth,
    double tableHeight)
        {
            List<Ball> list = balls.ToList();

            var pairs = new List<(int i, int j)>();

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    pairs.Add((i, j));
                }
            }

            double currentTime =
                _stopwatch.Elapsed.TotalSeconds;

            double deltaTime =
                currentTime - _lastTimeSeconds;

            _lastTimeSeconds = currentTime;

            // clamp żeby uniknąć huge lag spike
            deltaTime = Math.Min(deltaTime, 0.03);

            Parallel.ForEach(list, ball =>
            {
                ball.ApplyVelocityStep(deltaTime);
            });

            Parallel.ForEach(list, ball =>
            {
                _collisionDetector.ResolveWallCollisions(
                    ball,
                    tableWidth,
                    tableHeight);
            });

            Parallel.ForEach(pairs, pair =>
            {
                _collisionDetector.TryResolveBallCollision(
                    list[pair.i],
                    list[pair.j]);
            });

            foreach (Ball ball in list)
            {
                _logger.LogBallState(ball);
            }
        }


        public async Task UpdatePositionsAsync(
            double tableWidth,
            double tableHeight)
        {
            var balls = _repository.GetAllBalls();

            await Task.Run(() =>
                UpdatePositions(
                    balls,
                    tableWidth,
                    tableHeight));

            SimulationUpdated?.Invoke(
                this,
                new SimulationUpdateEventArgs
                {
                    UpdatedBalls = GetBallModels()
                });
        }

        public IReadOnlyList<BallModel> GetBallModels()
        {
            return _repository
                .GetAllBalls()
                .Select(BallToModel)
                .ToList()
                .AsReadOnly();
        }

        private BallModel BallToModel(Ball ball)
        {
            ball.GetState(
                out double x,
                out double y,
                out double vx,
                out double vy,
                out double radius,
                out double mass);

            return new BallModel(
                x,
                y,
                radius,
                vx,
                vy,
                mass);
        }
    }
}