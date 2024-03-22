using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;
using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.Mangers
{
    public class GameManager
    {
        private readonly Map _map;
        private readonly CollisionManager _collisionManager;
        public readonly GridMap _gridMap;
        private readonly Lighting _lighting;
        public readonly Player _player;
        private readonly Light _torchLight;
        private readonly List<Skeleton> _skeletons;
        private readonly Obstacle _rocks;
        private readonly UIManager _uiManager;
        public List<Obstacle> _obstacles;
        public List<Sprite> _entities;
        Texture2D pixelTexture;
        private Matrix _translation;
        public Vector2 Center = new(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2);
        private readonly Vector2 rockPos = new(Globals.WindowSize.X / 3, (Globals.WindowSize.Y / 3)*2);

        public GameManager()
        {
            _map = new Map();
            _collisionManager = new CollisionManager(this);

            // Obstacles
            _obstacles = new List<Obstacle>();
            _rocks = new Obstacle(Globals.Content.Load<Texture2D>("Rocks"), rockPos);
            _obstacles.Add(_rocks);

            _gridMap = new GridMap(this, _map);

            _player = new Player(Globals.Content.Load<Texture2D>("walknew"), Center);
            _player.SetBounds(_map.MapSize, _map.TileSize);
            _entities = new List<Sprite>{_player};

            _skeletons = new List<Skeleton>();
            GenerateSkeletons(1);

            _lighting = new Lighting();
            _torchLight = new Light(new Vector2(_player.Position.X - _player.origin.X * 2, _player.Position.Y - _player.origin.Y * 2), 600, 0.9f);
            _lighting.AddLight(_torchLight);

            _uiManager = new UIManager(_player);

            Globals.Font = Globals.Content.Load<SpriteFont>("Font");
        }

        private void CalculateTranslation()
        {
            var dx = Globals.WindowSize.X / 2 - _player.Position.X;
            dx = MathHelper.Clamp(dx, -_map.MapSize.X + Globals.WindowSize.X + _map.TileSize.X / 2, _map.TileSize.X / 2);
            var dy = Globals.WindowSize.Y / 2 - _player.Position.Y;
            dy = MathHelper.Clamp(dy, -_map.MapSize.Y + Globals.WindowSize.Y + _map.TileSize.Y / 2, _map.TileSize.Y / 2);
            _translation = Matrix.CreateTranslation(dx, dy, 0f);
        }

        public void Update()
        {
            InputManager.Update();
            _player.Update(_collisionManager);
            _torchLight.Position = _player.Position + _player.origin + _player.origin / 2; // annoying but works 
            
            foreach (var skeleton in _skeletons)
            {
                skeleton.Update(_player);
            }

            CalculateTranslation();
            _collisionManager.Update();
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _translation);
            _map.Draw(_lighting);
            _gridMap.Draw();

            _rocks.Color = _lighting.CalculateLighting(rockPos);
            _rocks.Draw();

            _player.Color = _lighting.CalculateLighting(_player.Position);
            _player.Draw();
            //DrawPositionDebug(_gridMap.GetGridPointPosition(_gridMap.GetNearestGridPoint(_player.Position)));
            //_player.DrawPositionDebug();

            foreach (var skeleton in _skeletons)
            {
                skeleton.Color = _lighting.CalculateLighting(skeleton.Position);
                skeleton.Draw();
            }

            Globals.SpriteBatch.End();

            _uiManager.Color = _lighting.CalculateLighting(_player.Position);
            _uiManager.Draw();
            
        }

        private void GenerateSkeletons(int count)
        {
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                Vector2 randomPosition = new Vector2(random.Next((int)_map.MapSize.X), random.Next((int)_map.MapSize.Y));
                Skeleton skeleton = new Skeleton(Globals.Content.Load<Texture2D>("skeleton_walk"), randomPosition, this);
                skeleton.SetBounds(_map.MapSize, _map.TileSize);
                _skeletons.Add(skeleton);
                _entities.Add(skeleton);
            }
        }

        // Method to get all obstacles within a radius of a position
        public bool obstacleProximity(Vector2 position, float radius)
        {
            foreach(var obstacle in _obstacles)
            {
                float distanceX = Math.Abs(obstacle.Position.X - position.X);
                float distanceY = Math.Abs(obstacle.Position.Y - position.Y);
                float distance = Vector2.Distance(position, obstacle.Position);

                float minDistanceX = obstacle.width / 2 + radius;
                float minDistanceY = obstacle.height / 2 + radius;

                // Min distance between the centers required to detect a collision
                if (distanceX < minDistanceX && distanceY < minDistanceY)
                {
                    return true;
                }
                
            }
            return false;
        }

        public void DrawPositionDebug(Vector2 Position)
        {
            pixelTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new Color[] { Color.White });

            float radius = 5f;

            Color debugColor = Color.Blue;

            DrawCircle(Position, radius, debugColor);
        }

        // Helper method to draw a circle
        private void DrawCircle(Vector2 position, float radius, Color color)
        {
            int segments = 5; // Increase this for smoother circles
            float angleIncrement = MathHelper.TwoPi / segments;
            Vector2[] circlePoints = new Vector2[segments];

            // Calculate circle points
            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleIncrement;
                float x = position.X + radius * (float)Math.Cos(angle);
                float y = position.Y + radius * (float)Math.Sin(angle);
                circlePoints[i] = new Vector2(x, y);
            }

            // Draw the circle using primitive drawing methods provided by your framework
            for (int i = 0; i < segments - 1; i++)
            {
                // Draw line segments between adjacent circle points
                DrawLine(circlePoints[i], circlePoints[i + 1], color);
            }

            // Connect the last circle point with the first one to close the loop
            DrawLine(circlePoints[segments - 1], circlePoints[0], color);
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            Globals.SpriteBatch.Draw(pixelTexture,
                             new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             SpriteEffects.None,
                             0);

        }
    }
}