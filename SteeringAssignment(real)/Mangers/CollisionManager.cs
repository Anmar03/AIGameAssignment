using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace SteeringAssignment_real.Mangers
{
    public class CollisionManager
    {
        private readonly GameManager gameManager;
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
                    // Distance between the center of the obstacle and the center of the entity
                    float distanceX = Math.Abs(obstacle.Position.X - entity.Position.X);
                    float distanceY = Math.Abs(obstacle.Position.Y - entity.Position.Y);

                    // Min distance between the centers required to detect a collision
                    float minDistanceX = obstacle.width / 3 + entity.width / 3;
                    float minDistanceY = obstacle.height / 2 + entity.height / 4;

                    // If a collision has occurred along both axes
                    if (distanceX < minDistanceX && distanceY < minDistanceY)
                    {
                        // Handle the collision (for example, prevent the entity from moving)
                        // Adjust the entity's position to move it outside the obstacle
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

            // Check collisions between entities
            foreach (var entityA in gameManager._entities)
            {
                foreach (var entityB in gameManager._entities)
                {
                    if (entityA != entityB)
                    {
                        // Calculate distances and minimum distances
                        float distanceX = Math.Abs(entityA.Position.X - entityB.Position.X);
                        float distanceY = Math.Abs(entityA.Position.Y - entityB.Position.Y);
                        float minDistanceX = entityA.width / 4 + entityB.width / 4;
                        float minDistanceY = entityA.height / 4 + entityB.height / 4;

                        // If a collision occurs, adjust positions
                        if (distanceX < minDistanceX && distanceY < minDistanceY)
                        {
                            float overlapX = minDistanceX - distanceX;
                            float overlapY = minDistanceY - distanceY;

                            if (overlapX < overlapY)
                            {
                                // Adjust the positions of the colliding entities along the X-axis
                                if (entityA.Position.X < entityB.Position.X)
                                {
                                    entityA.Position.X -= overlapX / 2;
                                    entityB.Position.X += overlapX / 2;
                                }
                                else
                                {
                                    entityA.Position.X += overlapX / 2;
                                    entityB.Position.X -= overlapX / 2;
                                }
                            }
                            else
                            {
                                // Adjust the positions of the colliding entities along the Y-axis
                                if (entityA.Position.Y < entityB.Position.Y)
                                {
                                    entityA.Position.Y -= overlapY / 2;
                                    entityB.Position.Y += overlapY / 2;
                                }
                                else
                                {
                                    entityA.Position.Y += overlapY / 2;
                                    entityB.Position.Y -= overlapY / 2;
                                }
                            }// e nd else

                        } // end inner if

                    }// end if

                }// end for

            }// end for

        }// end function

        public (float distance, Vector2 direction, float health, Vector2 enemyPosition) ClosestEntityInfo(Vector2 position)
        {
            // Find the closest entity to the given position
            var closestEntity = gameManager._entities
                .OrderBy(entity => Vector2.DistanceSquared(entity.Position, position))
                .Where(entity => entity.Position != position)
                .FirstOrDefault();

            if (closestEntity != null)
            {
                // Calculate the distance between the given position and the closest entity
                float distance = Vector2.Distance(closestEntity.Position, position);

                // Calculate the normalized direction from the given position to the closest entity
                Vector2 direction = Vector2.Normalize(closestEntity.Position - position);

                float health = closestEntity.Health;

                Vector2 enemyPosition = closestEntity.Position;

                // Return the distance and direction as a tuple
                return (distance, direction, health, enemyPosition);
            }

            // If no entities exist, return default values
            return (-1f, Vector2.Zero, 0, Vector2.Zero);
        }

        public void SetClosestEntityHealth(Vector2 position, float health)
        {
            var closestEntity = gameManager._entities
                .OrderBy(entity => Vector2.DistanceSquared(entity.Position, position))
                .Where(entity => entity.Position != position)
                .FirstOrDefault();

            if (closestEntity != null)
            {
                closestEntity.Health = health;
            }
        }

        public void SetClosestEntityPosition(Vector2 position, Vector2 newPosition)
        {
            var closestEntity = gameManager._entities
                .OrderBy(entity => Vector2.DistanceSquared(entity.Position, position))
                .Where(entity => entity.Position != position)
                .FirstOrDefault();

            if (closestEntity != null)
            {
                closestEntity.Position += newPosition * Globals.Time;
            }
        }
    }
}
