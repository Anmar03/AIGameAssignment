using Microsoft.Xna.Framework;
using SteeringAssignment_real.Models;
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

        public void Draw(Vector2 position, Color lighting)
        {
            _anims[_lastKey].Draw(position, lighting);
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
