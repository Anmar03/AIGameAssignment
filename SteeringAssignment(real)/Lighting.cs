using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SteeringAssignment_real
{
    public class Lighting
    {
        private List<Light> _lights;

        public Lighting()
        {
            _lights = new List<Light>();
        }

        public void AddLight(Light light)
        {
            _lights.Add(light);
        }

        public Color CalculateLighting(Vector2 position)
        {
            float totalIntensity = 0f;
            float maxIntensity = 0f;
            float intensity = 0f;
            Color lightColor = Color.White;

            foreach (var light in _lights)
            {
                float distance = Vector2.Distance(position, light.Position);
                if (distance < light.Radius / 2)
                {
                    intensity = light.Intensity * (1f - distance / light.Radius);
                    totalIntensity += intensity;
                    maxIntensity = MathHelper.Max(maxIntensity, light.Intensity);
                    if (light.Intensity != 0f)
                    {
                        lightColor = light.Color;
                    }

                }
                else if (distance > light.Radius / 2)
                {
                    maxIntensity = MathHelper.Max(maxIntensity, intensity);
                }
            }

            // Calculate brightness based on the total intensity
            float brightness = MathHelper.Clamp(totalIntensity, 0.2f, 1f);

            // Interpolate between black and white based on the maximum intensity
            float t = MathHelper.Clamp(maxIntensity / 1.5f, 0.3f, 1f);
            Color finalColor = Color.Lerp(Color.Black, lightColor, t);

            finalColor *= brightness;

            return finalColor;
        }
    }
}
