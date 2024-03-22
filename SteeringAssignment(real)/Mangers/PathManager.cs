using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SteeringAssignment_real.Mangers
{
    public class PathManager
    {
        private GameManager gameManager;
        private GridMap grid;
        private float radius;
        public Vector2 Destination;
        private List<Vector2> shortestPath;

        public PathManager(GameManager gameManager) 
        {
            this.gameManager = gameManager;
            grid = gameManager._gridMap;
            radius = gameManager._gridMap.radius;
        }

        public List<Vector2> AStar(Vector2 source, Vector2 destination) 
        {
            Destination = destination;
            PriorityQueue<Vector2> openSet = new PriorityQueue<Vector2>();
            shortestPath = new List<Vector2>();
            source = grid.GetGridPointPosition(grid.GetNearestGridPoint(source));
            openSet.Enqueue(source, 1);

            Vector2 current = new Vector2(-1, -1);
            float distanceSquared;

            while (!openSet.IsEmpty)
            {
                current = openSet.Dequeue();

                if (current == grid.GetGridPointPosition(grid.GetNearestGridPoint(destination)))
                {
                    // Return shortest path
                    return shortestPath;
                }
                 
                shortestPath.Add(current);
   
                List<Point> neighbouringPoints;
                neighbouringPoints = grid.GetNeighbouringPoints(current);

                foreach (Point neighbour in neighbouringPoints)
                { 
                    Vector2 neighbourPosition = grid.GetGridPointPosition(neighbour);
                    // If neighbour is blocked or inside closedSet then move to next neighbour
                    if (gameManager.obstacleProximity(neighbourPosition, radius) || shortestPath.Contains(neighbourPosition))
                    {
                        // Move to next neighbour
                    }
                    else
                    {
                        // If neighbour is not in openSet then add it with a priority of distance squared to the destination
                        if (!openSet.Contains(neighbourPosition))
                        {
                            // Euclidean distance
                            distanceSquared = ((destination.X - neighbourPosition.X) * (destination.X - neighbourPosition.X)) + ((destination.Y - neighbourPosition.Y) * (destination.Y - neighbourPosition.Y));
                            openSet.Enqueue(neighbourPosition, distanceSquared);
                            //consideredNodes.Add(neighbourPosition);
                        }
                    }
                }
            }
            
            // Failure
            return new List<Vector2> { destination };
        }

    }
}
