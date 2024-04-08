using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SteeringAssignment_real.Mangers
{
    public class PathManager
    {
        private readonly GameManager gameManager;
        private readonly GridMap grid;
        private readonly float radius;
        private PriorityQueue<Vector2> openSet;
        private HashSet<Vector2> closedSet;


        public PathManager(GameManager gameManager) 
        {
            this.gameManager = gameManager;
            grid = gameManager.GetGridMap();
            radius = grid.radius;
        }

        public List<Vector2> AStar(Vector2 source, Vector2 destination)
        {
            openSet = new();
            closedSet = new(); 
            Dictionary<Vector2, Vector2> cameFrom = new(); 

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
                        // Skip neighbour if blocked or already visited
                        continue;
                    }

                    float gScore = GScoreCost(current, neighbourPosition);

                    // If neighbour is not in openSet or new path is better, update openSet and cameFrom
                    if (!openSet.Contains(neighbourPosition) || gScore < GScoreCost(neighbourPosition, cameFrom.GetValueOrDefault(neighbourPosition)))
                    {
                        cameFrom[neighbourPosition] = current;
                        float fScore = gScore + HeuristicCost(neighbourPosition, destination); // f = g + h
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
            List<Vector2> path = new List<Vector2>{ current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse(); 
            return path;
        }

        // heuristic cost from a node to destination
        private float HeuristicCost(Vector2 from, Vector2 to)
        {
            return Vector2.DistanceSquared(from, to);
        }

        // calculate actual cost from start to a node
        private float GScoreCost(Vector2 current, Vector2 neighbour)
        {
            return Vector2.DistanceSquared(current, neighbour);
        }

        public HashSet<Vector2> GetConsideredNodes()
        {
            return closedSet;
        }
    }
}
