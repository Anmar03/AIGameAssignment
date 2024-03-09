using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;
using System;

namespace SteeringAssignment_real
{
    public class Triangle : Sprite
    {
        private const float speed = 250;
        private Vector2 _minPos, _maxPos;
        private Texture2D _texture;
        private float rotation;
        public Triangle(Texture2D texture, Vector2 position) : base(texture, position)
        {
            this._texture = texture;
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            _minPos = new((-tileSize.X / 2) + width, (-tileSize.Y / 2) + height);
            _maxPos = new(mapSize.X - (tileSize.X / 2) - width, mapSize.Y - (tileSize.X / 2) - height);
        }

        public void Update(Vector2 target)
        {
            var directionToTarget = target - Position; directionToTarget.Normalize();

            Position += directionToTarget * speed * Globals.Time;
            Position = Vector2.Clamp(Position, _minPos, _maxPos);
            // Calculate the angle between the sprite position and the target position
            rotation = (float)Math.Atan2(target.Y - Position.Y, target.X - Position.X) + (float)(Math.PI / 2);
        }

        public override void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, Position, null, Color, rotation, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
               //\
              // \\
             //   \\
            // Tri \\
           // angle \\
          //=========\\