using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SteeringAssignment_real.Mangers
{
    public class GridMap
    {
        private Map _map;
        private readonly GameManager _gameManager;
        private Point[,] _grid;
        public Point[,] Grid => _grid;
        public Point rowCol { get; private set; }
        private readonly Point emptyPoint = new Point(0, 0);
        float radius;

        public GridMap(GameManager gameManager, Map map)
        {
            _gameManager = gameManager;
            _map = map;
            radius = 20;
            GenerateGrid(30, 36);
        }

        private void GenerateGrid(int rows, int columns)
        {
            rowCol = new Point(rows, columns);
            _grid = new Point[rows, columns];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    // Calculate grid point position based on map size and grid size
                    int posX = x * (_map.MapSize.X / columns);
                    int posY = y * (_map.MapSize.Y / rows);


                    _grid[y, x] = new Point(posX, posY);
                    
                }
            }
        }

        public void Draw()
        {
            for (int y = 0; y < _grid.GetLength(0); y++)
            {
                for (int x = 0; x < _grid.GetLength(1); x++)
                {
                  
                    // Draw horizontal line
                    if (x < _grid.GetLength(1) - 1)
                    {
                        Vector2 start = new Vector2(_grid[y, x].X, _grid[y, x].Y + _map.TileSize.Y / 2);
                        Vector2 end = new Vector2(_grid[y, x + 1].X, _grid[y, x + 1].Y + _map.TileSize.Y / 2);
                        
                        if(!_gameManager.obstacleProximity(start, radius) && !_gameManager.obstacleProximity(end, radius))
                        {
                            DrawLine(start, end, Color.Black);
                        }

                    }

                    // Draw vertical line
                    if (y < _grid.GetLength(0) - 1)
                    {
                        Vector2 start = new Vector2(_grid[y, x].X + _map.TileSize.X / 2, _grid[y, x].Y);
                        Vector2 end = new Vector2(_grid[y + 1, x].X + _map.TileSize.X / 2, _grid[y + 1, x].Y);

                        if (!_gameManager.obstacleProximity(start, radius/2) && !_gameManager.obstacleProximity(end, radius/2))
                        {
                            DrawLine(start, end, Color.Black);
                        }
                            
                    }
                }
            }
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end);
            
            // Ensure that the line is drawn correctly using the provided SpriteBatch
            Globals.SpriteBatch.Draw(Globals.Pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public Vector2 GetGridPointPosition(Point rC)
        {
            int row = rC.X;
            int column = rC.Y;
            // Check if the provided row and column indices are within the valid range
            if (row >= 0 && row < rowCol.Y && column >= 0 && column < rowCol.X)
            {
                // Retrieve and return the position of the specified grid point
                return new Vector2(_grid[row, column].X + _map.TileSize.X / 2, _grid[row, column].Y + _map.TileSize.Y / 2);
            }
            else
            {
                // Return Vector2.Zero or throw an exception to indicate invalid indices
                return Vector2.Zero;
            }
        }

        public Point GetNearestGridPoint(Vector2 position)
        {
            int nearestRow = -1;
            int nearestColumn = -1;
            float minDistanceSquared = float.MaxValue;

            // Iterate through all grid points to find the nearest one
            for (int row = 0; row < _grid.GetLength(0); row++)
            {
                for (int column = 0; column < _grid.GetLength(1); column++)
                {
                    // Calculate the distance squared between the current grid point and the given position
                    float distanceSquared = Vector2.DistanceSquared(position, new Vector2(_grid[row, column].X + _map.TileSize.X / 2, _grid[row, column].Y + _map.TileSize.Y / 2));

                    // Update the nearest grid point if this grid point is closer
                    if (distanceSquared < minDistanceSquared)
                    {
                        minDistanceSquared = distanceSquared;
                        nearestRow = row;
                        nearestColumn = column;
                    }
                }
            }

            // Return the nearest grid point
            return new Point(nearestColumn, nearestRow);
        }

    }
}
