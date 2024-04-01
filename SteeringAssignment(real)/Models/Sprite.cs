﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SteeringAssignment_real.Models
{
    public class Sprite
    {
        private readonly Texture2D _texture;
        public Vector2 Position;
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public bool EntityCollision = true;

        public float width, height;
        public float Health;
        public float AggroRadius;

        public Sprite(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            width = texture.Width; 
            height = texture.Height;
            Position = position;
            Origin = new(_texture.Width / 2, _texture.Height / 2);
            Color = Color.White;
        }

        public virtual void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, Position, null, Color, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
