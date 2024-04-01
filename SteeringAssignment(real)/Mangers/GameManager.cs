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
        private readonly GridMap _gridMap;
        private readonly Lighting _lighting;
        public readonly Player _player;
        private readonly Torch _torch;
        private List<GlowStick> _glowSticks;
        private readonly List<Skeleton> _skeletons;
        private readonly UIManager _uiManager;
        public List<Obstacle> _obstacles;
        public List<Sprite> _entities;
        private Texture2D pixelTexture;
        private Matrix _translation;
        private Vector2 Center = new(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2);
        private Random random;
        private const int VectorLength = 50;
        private bool DebugMode = false;
        private int glowStickAmmo;
        private bool glowstickCooldown = false;
        private float glowstickCooldownDuration = 1.0f; // Adjust as needed, in seconds
        private float glowstickCooldownTimer = 0.0f;

        public GameManager()
        {
            _map = new();
            _collisionManager = new CollisionManager(this);
            random = new();

            // Obstacles
            _obstacles = new List<Obstacle>();
            GenerateObstacles(8);

            _gridMap = new(this, _map);

            _player = new Player(Globals.Content.Load<Texture2D>("walknew"), Center);
            _player.SetBounds(_map.MapSize, _map.TileSize);
            _entities = new List<Sprite>{_player};

            _skeletons = new();
            GenerateSkeletons(4);

            _lighting = new Lighting();

            _torch = new Torch((_player.Position + _player.origin + _player.origin / 2));
            _lighting.AddLight(_torch);

            _glowSticks = new();
            GenerateGlowSticks(3);
            glowStickAmmo = _glowSticks.Count-1;

            Globals.Font = Globals.Content.Load<SpriteFont>("Font");
            _uiManager = new UIManager(_player);
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
            DebugMode = InputManager.DebugMode;

            _player.Update(_collisionManager);

            if (_player.isDead())
            {
                _lighting.AddLight(new Light(Center, Center.X*2, 0.8f, 1));
            }

            _torch.LifeSpan -= Globals.Time;
            _torch.Position = _player.Position + _player.origin + _player.origin / 2; // annoying but works

            if (glowstickCooldown)
            {
                glowstickCooldownTimer -= Globals.Time;
                if (glowstickCooldownTimer <= 0)
                {
                    glowstickCooldown = false;
                }
            }

            if (glowStickAmmo >= 0 && !glowstickCooldown)
            {
                if (InputManager.G_keyPressed && _glowSticks[glowStickAmmo].throwable)
                {
                    _glowSticks[glowStickAmmo].Throw(InputManager.LastDirection);
                    glowStickAmmo--;
                    glowstickCooldown = true;
                    glowstickCooldownTimer = glowstickCooldownDuration;
                }

                if (InputManager.E_keyPressed && _glowSticks[glowStickAmmo].throwable)
                {
                    _glowSticks[glowStickAmmo].Active = true;
                }
            }

            foreach (var glowStick in _glowSticks)
            {
                if (glowStick.throwable)
                {
                    glowStick.Position = _player.Position + _player.origin + _player.origin / 2; // annoying but works 
                }
                glowStick.Update();
            }

            if (_torch.LifeSpan <= 0f)
            {
                _torch.Intensity = 0f;
            }
            else
            {
                // Adjust intensity based on remaining lifespan
                if (_torch.getDefaultIntensity() * (_torch.LifeSpan / 60f) < _torch.getDefaultIntensity())
                {
                    _torch.Intensity = _torch.getDefaultIntensity() * (_torch.LifeSpan / 60f);
                }
            }

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

            foreach (var rock in _obstacles)
            {
                rock.Color = _lighting.CalculateLighting(rock.Position);
                rock.Draw();
            }

            _player.Color = _lighting.CalculateLighting(_player.Position);
            _player.Draw();
            if (!_player.isDead() && DebugMode)
            {
                DrawLine(_player.Position, _player.Position + _player.playerDirection * VectorLength, Color.Red);
            } 

            foreach (var skeleton in _skeletons)
            {
                skeleton.Color = _lighting.CalculateLighting(skeleton.Position);
                skeleton.Draw();
                if (skeleton.currentState != State.Dead && DebugMode)
                {
                    DrawLine(skeleton.Position, skeleton.Position + Vector2.Normalize(skeleton.skeletonDirection) * VectorLength, Color.Red);
                }
            }

            foreach (var glowStick in _glowSticks)
            {
                glowStick.Draw();
            }

            Globals.SpriteBatch.End();

            _uiManager.Draw();
            
        }

        private void GenerateGlowSticks(int count)
        {
            Color red = new(200, 50, 50);
            Color green = new(50, 200, 50);
            Color blue = new(50, 50, 200);
            GlowStick glowStick;
            int randomPicker;

            for (int i = 0; i < count; i++)
            {
                randomPicker = random.Next(0, 3);

                if (randomPicker == 0)
                {
                    glowStick = new((_player.Position), red);
                }
                else if (randomPicker == 1)
                {
                    glowStick = new((_player.Position), green);
                }
                else
                {
                    glowStick = new((_player.Position), blue);
                }
                glowStick.SetBounds(_map.MapSize, _map.TileSize);
                _glowSticks.Add(glowStick);
                //_entities.Add(glowStick.GetEntity()); // causes problems like not allowing player to attack enemys
                _lighting.AddLight(glowStick);
            }
        }

        private void GenerateSkeletons(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 randomPosition = new(random.Next((int)_map.MapSize.X), random.Next((int)_map.MapSize.Y));
                Skeleton skeleton = new(Globals.Content.Load<Texture2D>("skeleton_walk"), randomPosition, this);
                skeleton.SetBounds(_map.MapSize, _map.TileSize);
                _skeletons.Add(skeleton);
                _entities.Add(skeleton);
            }
        }
        private void GenerateObstacles(int count)
        {
            int padding = 100;
            Texture2D texture;
            Vector2 randomPosition;
            int randomPicker;

            for (int i = 0; i < count; i++)
            {
                randomPicker = random.Next(0, 3);
                if (randomPicker == 0)
                {
                    texture = Globals.Content.Load<Texture2D>("Rocks");
                }
                else if(randomPicker == 1)
                {
                    texture = Globals.Content.Load<Texture2D>("tallRock");
                }
                else
                {
                    texture = Globals.Content.Load<Texture2D>("crystalRock");
                }

                do
                {
                    randomPosition = new(random.Next(padding, (int)_map.MapSize.X - padding), random.Next(padding, (int)_map.MapSize.Y - padding));
                } while (ObstacleProximity(randomPosition, texture.Width/2));

                Obstacle obstacle = new(texture, randomPosition);
                _obstacles.Add(obstacle);
            }
        }

        // Method to get all obstacles within a radius of a position
        public bool ObstacleProximity(Vector2 position, float radius)
        {
            foreach(var obstacle in _obstacles)
            {
                float distanceX = Math.Abs(obstacle.Position.X - position.X);
                float distanceY = Math.Abs(obstacle.Position.Y - position.Y);

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
            pixelTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new Color[] { Color.White });

            Globals.SpriteBatch.Draw(pixelTexture,
                             new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             SpriteEffects.None,
                             0);

        }

        public GridMap GetGridMap()
        {
            return _gridMap;
        }

        public void AddLight(Light light)
        {
            _lighting.AddLight(light);
        }
    }
}