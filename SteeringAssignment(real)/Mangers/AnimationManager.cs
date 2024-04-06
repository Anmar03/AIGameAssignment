using Microsoft.Xna.Framework;
using SteeringAssignment_real.Models;
using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.Mangers
{
    public class AnimationManager
    {
        private readonly Dictionary<object, Animation> _anims = new();
        private object _lastKey;
        public int TotalFrames { get; private set; } = 0;
        public int CurrentFrame { get; private set; } = 0;

        public void AddAnimation(object key, Animation animation)
        {
            _anims.Add(key, animation);
            _lastKey ??= key;
            TotalFrames = animation.FrameCount;
        }

        public void Update(object key)
        {
            key ??= new Vector2(0, 1); // S Key is the default direction incase the player attacks before moving

            if (_anims.TryGetValue(key, out Animation value))
            {
                value.Start();
                _anims[key].Update();
                _lastKey = key;
                CurrentFrame = _anims[key].CurrentFrameIndex;
            }
            else
            {
                _anims[_lastKey].Stop();
                _anims[_lastKey].Reset();
            }
        }

        public void UpdateDeath(object key)
        {
            if (_anims.TryGetValue(key, out Animation value))
            {
                value.Start();
                _anims[key].Update();
                _lastKey = key;
                CurrentFrame = _anims[key].CurrentFrameIndex;
            }
            else
            {
                _anims[_lastKey].Stop();
            }
        }

        public void Draw(Vector2 position, Color lighting)
        {
            _anims[_lastKey].Draw(position, lighting);
        }

        public static Vector2 GetAnimationKey(Vector2 direction)
        {
            if (!float.IsNaN(direction.X) && !float.IsNaN(direction.Y))
            {
                // Round the direction vector components to -1, 0, or 1
                int x = Math.Sign(direction.X);
                int y = Math.Sign(direction.Y);

                return new Vector2(x, y);
            }
            else
            {
                // If direction is NaN
                return Vector2.Zero;
            }
        }

        public static Vector2 Get16AnimationKey(Vector2 direction)
        {
            if (!float.IsNaN(direction.X) && !float.IsNaN(direction.Y))
            {
                // Round direction vector components to closest 0.5, 0, -0.5, -1, or 1
                float x = RoundToNearest(direction.X);
                float y = RoundToNearest(direction.Y);

                return new Vector2(x, y);
            }
            else
            {  
                return Vector2.Zero;
            }
        }

        private static float RoundToNearest(float value)
        {
            float roundedValue = (float)Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2;
            float diffToHalf = (float)Math.Abs(roundedValue - Math.Round(roundedValue));
            if (diffToHalf < 0.25)
            {
                return (float)Math.Round(roundedValue);
            }
            else if (diffToHalf > 0.75)
            {
                return (float)Math.Round(Math.Sign(value) * Math.Ceiling(Math.Abs(value)));
            }
            else
            {
                return roundedValue;
            }
        }

        public void Reset()
        {
            foreach (var animation in _anims.Values)
            {
                animation.Reset();
            }
            CurrentFrame = 0;
        }
    }
}
