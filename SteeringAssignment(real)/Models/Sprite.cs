using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SteeringAssignment_real.Models
{
    public class Sprite
    {
        private readonly Texture2D _texture;
        private Texture2D pixelTexture;

        public Vector2 Position;
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }

        public float width, height;

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

        public void DrawPositionDebug()
        {
            pixelTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new Color[] { Color.White });

            float radius = 5f;

            Color debugColor = Color.Red;

            Vector2 debugPosition = Position;

            DrawCircle(debugPosition, radius, debugColor);
        }

        // Helper method to draw a circle
        private void DrawCircle(Vector2 position, float radius, Color color)
        {
            int segments = 20; // Increase this for smoother circles
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
