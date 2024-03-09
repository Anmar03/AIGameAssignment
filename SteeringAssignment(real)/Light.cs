using Microsoft.Xna.Framework;

namespace SteeringAssignment_real
{
    public class Light
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public float Intensity { get; set; }

        public Light(Vector2 position, float radius, float intensity)
        {
            Position = position;
            Radius = radius;
            Intensity = intensity;
        }
    }
}
