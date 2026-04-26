using BilliardSimulation.Data;
using System;
using System.Collections.Generic;

namespace BilliardSimulation.Logic
{
    /// <summary>
    /// Handles collision detection and elastic collision physics
    /// </summary>
    public class CollisionDetector
    {
        private const double visualMargin = 1.0;

        /// <summary>
        /// Detects and resolves collisions between two balls (elastic collision)
        /// </summary>
        public bool TryResolveBallCollision(Ball ball1, Ball ball2)
        {
            ball1.GetState(out double x1, out double y1, out double vx1, out double vy1, out double r1, out double m1);
            ball2.GetState(out double x2, out double y2, out double vx2, out double vy2, out double r2, out double m2);

            // Calculate distance between centers
            double dx = x2 - x1;
            double dy = y2 - y1;
            double distanceSq = dx * dx + dy * dy;
            double minDistanceSq = (r1 + r2) * (r1 + r2);

            // Check if balls are close enough to collide
            if (distanceSq > minDistanceSq || distanceSq < 0.0001)
                return false;

            double distance = Math.Sqrt(distanceSq);

            // Normalize collision vector
            double nx = dx / distance;
            double ny = dy / distance;

            // Relative velocity
            double dvx = vx2 - vx1;
            double dvy = vy2 - vy1;

            // Relative velocity in collision normal direction
            double dvn = dvx * nx + dvy * ny;

            // Don't collide if moving apart (this prevents bounce-back)
            if (dvn >= 0)
                return false;

            // Calculate impulse scalar for elastic collision
            double impulse = -2 * dvn / (1 / m1 + 1 / m2);

            // Apply impulse
            double newVx1 = vx1 + (impulse / m1) * nx;
            double newVy1 = vy1 + (impulse / m1) * ny;
            double newVx2 = vx2 - (impulse / m2) * nx;
            double newVy2 = vy2 - (impulse / m2) * ny;

            // Set new velocities (thread-safe)
            ball1.SetVelocities(newVx1, newVy1);
            ball2.SetVelocities(newVx2, newVy2);

            // Separate balls to prevent overlap - NO EXTRA ENERGY INJECTION
            double overlap = (r1 + r2) - distance;
            double separationX = (overlap / 2) * nx;
            double separationY = (overlap / 2) * ny;

            ball1.X -= separationX;
            ball1.Y -= separationY;
            ball2.X += separationX;
            ball2.Y += separationY;

            return true;
        }

        /// <summary>
        /// Checks and resolves wall collisions for a single ball
        /// </summary>
        public void ResolveWallCollisions(Ball ball, double tableWidth, double tableHeight)
        {
            ball.GetState(out double x, out double y, out double vx, out double vy, out double radius, out double mass);

            bool collided = false;
            double newVx = vx;
            double newVy = vy;
            double newX = x;
            double newY = y;

            // Left wall
            if (x - radius <= visualMargin)
            {
                newX = radius + visualMargin;
                newVx = Math.Abs(vx); // Bounce right
                collided = true;
            }

            // Right wall
            if (x + radius >= tableWidth - visualMargin)
            {
                newX = tableWidth - radius - visualMargin;
                newVx = -Math.Abs(vx); // Bounce left
                collided = true;
            }

            // Top wall
            if (y - radius <= visualMargin)
            {
                newY = radius + visualMargin;
                newVy = Math.Abs(vy); // Bounce down
                collided = true;
            }

            // Bottom wall
            if (y + radius >= tableHeight - visualMargin)
            {
                newY = tableHeight - radius - visualMargin;
                newVy = -Math.Abs(vy); // Bounce up
                collided = true;
            }

            if (collided)
            {
                ball.X = newX;
                ball.Y = newY;
                ball.SetVelocities(newVx, newVy);
            }

            // Final safety clamp
            ball.X = Math.Max(radius + visualMargin, Math.Min(ball.X, tableWidth - radius - visualMargin));
            ball.Y = Math.Max(radius + visualMargin, Math.Min(ball.Y, tableHeight - radius - visualMargin));
        }
    }
}
