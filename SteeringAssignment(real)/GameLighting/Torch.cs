using Microsoft.Xna.Framework;

namespace SteeringAssignment_real.GameLighting
{
    public class Torch : Light
    {
        public Vector2 position { get; set; }
        private const float defaultRadius = 550f;
        private const float defaultIntensity = 0.9f;
        private const float defaultLifeSpan = 100f;
        public Torch(Vector2 position) : base(position, defaultRadius, defaultIntensity, defaultLifeSpan)
        {
            this.position = position;
        }

        public float getDefaultIntensity()
        {
            return defaultIntensity;
        }
    }
}
