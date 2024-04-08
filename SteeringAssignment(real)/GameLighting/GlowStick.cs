using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.GameLighting
{
    public class GlowStick : Light
    {
        private const float defaultRadius = 400;
        private const float defaultIntensity = 0.9f;
        private const float defaultLifeSpan = 90;
        private Vector2 _minPos, _maxPos;
        private Texture2D glowStickTexture;
        private Sprite glowSprite;
        private bool thrown = false;
        public bool throwable = true;
        public bool Active = false;
        private Vector2 velocity = Vector2.Zero;
        private const float defaultThrowSpeed = 1500f;
        private Vector2 throwStartPosition;
        private const float throwStopRadius = 200f;

        public GlowStick(Vector2 position, Color color = default) : base(position, defaultRadius, defaultIntensity, defaultLifeSpan, color)
        {
            glowStickTexture = Globals.Content.Load<Texture2D>("glowstick");
            glowSprite = new(glowStickTexture, position);
            glowSprite.EntityCollision = false;
            glowSprite.Color = color;
        }

        public void Update()
        {
            if (Active)
            {
                LifeSpan -= Globals.Time;
                Intensity = defaultIntensity;
                glowSprite.Color = Color.White;

                if (LifeSpan <= 0f)
                {
                    Intensity = 0f;
                }
                else
                {
                    // Adjust intensity based on remaining lifespan
                    if (defaultIntensity * (LifeSpan / 60f) < defaultIntensity)
                    {
                        Intensity = defaultIntensity * (LifeSpan / 60f);
                    }
                }
            }
            else
            {
                Intensity = 0.0f;
                glowSprite.Color = new(0, 0, 0, 0);
            }

            if (thrown)
            {
                // distance from current position to position it was thrown from
                float distanceToThrownFrom = Vector2.Distance(Position, throwStartPosition);

                // If GlowStick is within a certain radius from throw start position
                if (distanceToThrownFrom > throwStopRadius)
                {
                    // Gradually reduce velocity until it comes to a stop
                    velocity *= 0.9f;
                }

                // Update position based on velocity when thrown
                Position += velocity * Globals.Time;

                // if glowstick velocity is very low
                if (velocity.LengthSquared() < 400f)
                {
                    thrown = false;
                }
            }
            Position = Vector2.Clamp(Position, _minPos, _maxPos);
            glowSprite.Position = Position - glowSprite.Origin * 3;
            glowSprite.Position = Vector2.Clamp(glowSprite.Position, _minPos, _maxPos);
        }
        public void Draw()
        {
            glowSprite.Draw();
        }

        public Sprite GetEntity()
        {
            return glowSprite;
        }

        public void Throw(Vector2 direction)
        {
            if (!thrown)
            {
                if (!Active) Active = true;

                thrown = true;
                throwable = false;

                velocity = Vector2.Normalize(direction) * defaultThrowSpeed;

                throwStartPosition = Position;
            }
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            _minPos = new(-tileSize.X / 2 + glowSprite.width / 2, -tileSize.Y / 2 + glowSprite.height / 2);
            _maxPos = new(mapSize.X - tileSize.X / 2 - glowSprite.width / 2, mapSize.Y - tileSize.X / 2 - glowSprite.height / 2);
        }
    }
}
