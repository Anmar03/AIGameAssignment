using Microsoft.Xna.Framework;

namespace SteeringAssignment_real
{
    public class Light
    {
        public Vector2 Position { get; set; }
        public float Radius { get; private set; }
        public float Intensity { get; set; }
        public float LifeSpan { get; set; }
        public Color Color { get; set; }

        public Light(Vector2 position, float radius, float intensity, float lifeSpan = 100, Color color = default)
        {
            Intensity = intensity;
            Position = position;
            Radius = radius;
            LifeSpan = lifeSpan;
            Color = color == default ? Color.White : color;
        }
    }
}
