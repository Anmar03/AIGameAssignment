using Microsoft.Xna.Framework;
using SteeringAssignment_real.Models;
using System.Collections.Generic;

namespace SteeringAssignment_real.Mangers
{
    public class AnimationManager
    {
        private readonly Dictionary<object, Animation> _anims = new();
        private object _lastKey;

        public void AddAnimation(object key, Animation animation)
        {
            _anims.Add(key, animation);
            _lastKey ??= key;
        }

        public void Update(object key)
        {
            if (_anims.TryGetValue(key, out Animation value))
            {
                value.Start();
                _anims[key].Update();
                _lastKey = key;
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
    }
}
