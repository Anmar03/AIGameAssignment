using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SteeringAssignment_real.Mangers
{
    public class PathManager
    {
        private readonly GameManager gameManager;
        private readonly GridMap grid;
        private readonly float radius;
        public Vector2 Destination;
        

        public PathManager(GameManager gameManager) 
        {
            this.gameManager = gameManager;
            grid = gameManager._gridMap;
            radius = gameManager._gridMap.radius;
        }

        public List<Vector2> AStar(Vector2 source, Vector2 destination)
        {
            Destination = destination;
            PriorityQueue<Vector2> openSet = new();
            HashSet<Vector2> closedSet = new(); // Closed set to track visited nodes
            Dictionary<Vector2, Vector2> cameFrom = new(); // store parent nodes for path reconstruction

            source = grid.GetGridPointPosition(grid.GetNearestGridPoint(source));
            openSet.Enqueue(source, 1);

            while (!openSet.IsEmpty)
            {
                // Dequeue node with lowest f-score
                Vector2 current = openSet.Dequeue();
                closedSet.Add(current); 

                if (current == grid.GetGridPointPosition(grid.GetNearestGridPoint(destination)))
                {
                    // Reconstruct and return shortest path
                    return ReconstructPath(cameFrom, current);
                }

                List<Point> neighbouringPoints = grid.GetNeighbouringPoints(current);
                foreach (Point neighbour in neighbouringPoints)
                {
                    Vector2 neighbourPosition = grid.GetGridPointPosition(neighbour);
                    if (gameManager.ObstacleProximity(neighbourPosition, radius) || closedSet.Contains(neighbourPosition))
                    {
                        // Skip this neighbour if it's blocked or already visited
                        continue;
                    }

                    // Calculate tentative g-score (distance from start to neighbour)
                    float tentativeGScore = CalculateGScore(current, neighbourPosition);

                    // If neighbour is not in openSet or new path is better, update openSet and cameFrom
                    if (!openSet.Contains(neighbourPosition) || tentativeGScore < CalculateGScore(neighbourPosition, cameFrom.GetValueOrDefault(neighbourPosition)))
                    {
                        cameFrom[neighbourPosition] = current;
                        float fScore = tentativeGScore + HeuristicCostEstimate(neighbourPosition, destination); // f = g + h
                        openSet.Enqueue(neighbourPosition, fScore);
                    }
                }
            }

            // Failure
            return new List<Vector2> { destination };
        }

        // Method to reconstruct shortest path
        private List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
        {
            List<Vector2> path = new List<Vector2>();
            path.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse(); 
            return path;
        }

        // Method to calculate heuristic cost from a node to destination
        private float HeuristicCostEstimate(Vector2 from, Vector2 to)
        {
            return Vector2.DistanceSquared(from, to);
        }

        // Method to calculate actual cost from start to a node
        private float CalculateGScore(Vector2 current, Vector2 neighbour)
        {
            return Vector2.DistanceSquared(current, neighbour);
        }
    }
}
