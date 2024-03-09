
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;

namespace SteeringAssignment_real.Mangers
{
    public class CollisionManager
    {
        GameManager gameManager;
        public CollisionManager(GameManager gameManager) 
        {
            this.gameManager = gameManager;
        }

        public void Update() 
        {
            foreach (var obstacle in gameManager._obstacles)
            {
                foreach (var entity in gameManager._entities)
                {
                    // Calculate the distance between the center of the obstacle and the center of the entity
                    float distanceX = Math.Abs(obstacle.Position.X - entity.Position.X);
                    float distanceY = Math.Abs(obstacle.Position.Y - entity.Position.Y);

                    // Calculate the minimum distance between the centers required to detect a collision
                    float minDistanceX = obstacle.width / 2 + entity.width / 3;
                    float minDistanceY = obstacle.height / 2 + entity.height / 3;

                    // Check if a collision has occurred along both axes
                    if (distanceX < minDistanceX && distanceY < minDistanceY)
                    {
                        // Handle the collision (for example, prevent the entity from moving)
                        // Here, we simply adjust the entity's position to move it outside the obstacle
                        float overlapX = minDistanceX - distanceX;
                        float overlapY = minDistanceY - distanceY;

                        if (overlapX < overlapY)
                        {
                            // Adjust the entity's position along the X-axis
                            if (obstacle.Position.X > entity.Position.X)
                            {
                                entity.Position.X -= overlapX;
                            }
                            else
                            {
                                entity.Position.X += overlapX;
                            }
                        }
                        else
                        {
                            // Adjust the entity's position along the Y-axis
                            if (obstacle.Position.Y > entity.Position.Y)
                            {
                                entity.Position.Y -= overlapY;
                            }
                            else
                            {
                                entity.Position.Y += overlapY;
                            }
                        }
                    }
                }
            }
        }

    }
}
