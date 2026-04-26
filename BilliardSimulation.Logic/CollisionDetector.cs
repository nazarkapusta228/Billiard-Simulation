using BilliardSimulation.Data;
using System;
using System.Collections.Generic;

namespace BilliardSimulation.Logic
{
    /// <summary>
    /// Handles collision detection and elastic collision physics using proper Wikipedia formulas
    /// </summary>
    public class CollisionDetector
    {
        private const double visualMargin = 1.0;

        /// <summary>
        /// Detects and resolves collisions between two balls using proper elastic collision formulas
        /// </summary>
        public bool TryResolveBallCollision(Ball ball1, Ball ball2)
        {
            ball1.GetState(out double x1, out double y1, out double vx1, out double vy1, out double r1, out double m1);
            ball2.GetState(out double x2, out double y2, out double vx2, out double vy2, out double r2, out double m2);

            // Calculate distance between centers
            double dx = x2 - x1;
            double dy = y2 - y1;
            double distanceSq = dx * dx + dy * dy;
            double minDistance = r1 + r2;
            double minDistanceSq = minDistance * minDistance;

            // Check if balls are close enough to collide
            if (distanceSq > minDistanceSq || distanceSq < 0.0001)
                return false;

            double distance = Math.Sqrt(distanceSq);

            // Normalize collision vector (from ball1 to ball2)
            double nx = dx / distance;
            double ny = dy / distance;

            // Relative velocity (ball2 relative to ball1)
            double dvx = vx2 - vx1;
            double dvy = vy2 - vy1;

            // Relative velocity along the collision normal
            double vn = dvx * nx + dvy * ny;

            // Don't collide if balls are moving apart
            if (vn >= 0)
                return false;

            // Using the proper two-dimensional elastic collision formula from Wikipedia:
            // v1' = v1 - (2*m2/(m1+m2)) * (<v1-v2, x1-x2> / ||x1-x2||²) * (x1-x2)
            // v2' = v2 - (2*m1/(m1+m2)) * (<v2-v1, x2-x1> / ||x2-x1||²) * (x2-x1)

            // Calculate the impulse factor
            double impulseFactor = 2.0 * vn / (m1 + m2);

            // Apply impulse to get new velocities
            double newVx1 = vx1 + (m2 * impulseFactor) * nx;
            double newVy1 = vy1 + (m2 * impulseFactor) * ny;
            double newVx2 = vx2 - (m1 * impulseFactor) * nx;
            double newVy2 = vy2 - (m1 * impulseFactor) * ny;

            // Set new velocities (thread-safe)
            ball1.SetVelocities(newVx1, newVy1);
            ball2.SetVelocities(newVx2, newVy2);

            // Separate balls to prevent overlap - move them apart along collision normal
            double overlap = minDistance - distance;
            if (overlap > 0)
            {
                double totalMass = m1 + m2;
                double separation1 = overlap * (m2 / totalMass);
                double separation2 = overlap * (m1 / totalMass);

                ball1.X -= separation1 * nx;
                ball1.Y -= separation1 * ny;
                ball2.X += separation2 * nx;
                ball2.Y += separation2 * ny;
            }

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