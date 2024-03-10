using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;
using System.Collections.Generic;

namespace SteeringAssignment_real.Mangers
{
    public class GameManager
    {
        private readonly Map _map;
        private readonly Lighting _lighting;
        private readonly Player _player;
        private readonly Light _torchLight;
        private readonly Skeleton _skeleton;
        private readonly Obstacle _rocks;
        public List<Obstacle> _obstacles;
        public List<Sprite> _entities;
        private Matrix _translation;
        public Vector2 Center = new(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2);
        private readonly Vector2 rockPos = new(Globals.WindowSize.X / 3, (Globals.WindowSize.Y / 3)*2);

        public GameManager()
        {
            _map = new Map();
            _player = new Player(Globals.Content.Load<Texture2D>("playerEight"), Center);
            _player.SetBounds(_map.MapSize, _map.TileSize);
            _entities = new List<Sprite>{_player};

            _skeleton = new Skeleton(Globals.Content.Load<Texture2D>("skeleton_walk"), Center / 2);
            _skeleton.SetBounds(_map.MapSize, _map.TileSize);
            _entities.Add(_skeleton);

            // Obstacles
            _obstacles = new List<Obstacle>();
            _rocks = new Obstacle(Globals.Content.Load<Texture2D>("Rocks"), rockPos);
            _obstacles.Add(_rocks);

            _lighting = new Lighting();
            _torchLight = new Light(new Vector2(_player.Position.X - _player.origin.X * 2, _player.Position.Y - _player.origin.Y * 2), 600, 0.8f);
            _lighting.AddLight(_torchLight);
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
            _player.Update();
            _torchLight.Position = _player.Position + _player.origin + _player.origin / 2; // annoying but works 
            _skeleton.Update(_player);
            CalculateTranslation();
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _translation);
            _map.Draw(_lighting);

            _player.Color = _lighting.CalculateLighting(_player.Position);
            _player.Draw();
            //_player.DrawPositionDebug(graphicsDevice);

            _rocks.Color = _lighting.CalculateLighting(rockPos);
            _rocks.Draw();

            _skeleton.Color = _lighting.CalculateLighting(_skeleton.Position);
            _skeleton.Draw();

            Globals.SpriteBatch.End();
        }
    }
}