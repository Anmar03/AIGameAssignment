

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SteeringAssignment_real.Mangers;

namespace SteeringAssignment_real.Models
{
    public class Skeleton : Sprite
    {
        private const float speed = 150;
        private Vector2 _minPos, _maxPos;
        private Animation frame;
        private readonly AnimationManager _anims = new();
        private float frameWidth, frameHeight;
        public Vector2 origin;
        public Skeleton(Texture2D texture, Vector2 position) : base(texture, position)
        {
            frame = new Animation(texture, 8, 8, 0.1f, 7);
            _anims.AddAnimation(new Vector2(0, 1), frame);
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 8, 8, 0.1f, 1));
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 8, 8, 0.1f, 5));
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 8, 8, 0.1f, 3));
            _anims.AddAnimation(new Vector2(-1, 1), new Animation(texture, 8, 8, 0.1f, 8));
            _anims.AddAnimation(new Vector2(-1, -1), new Animation(texture, 8, 8, 0.1f, 2));
            _anims.AddAnimation(new Vector2(1, 1), new Animation(texture, 8, 8, 0.1f, 6));
            _anims.AddAnimation(new Vector2(1, -1), new Animation(texture, 8, 8, 0.1f, 4));
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            frameWidth = frame.frameWidth;
            frameHeight = frame.frameHeight;
            width = frameWidth; 
            height = frameHeight;
            origin = new Vector2(frameWidth / 2, frameHeight / 2);

            _minPos = new((-tileSize.X / 2) + frameWidth / 2, (-tileSize.Y / 2) + frameHeight / 2);
            _maxPos = new(mapSize.X - (tileSize.X / 2) - frameWidth / 2, mapSize.Y - (tileSize.X / 2) - frameHeight / 2);
        }
        public void Update(Vector2 target) 
        {
            var directionToTarget = target - Position; directionToTarget.Normalize();

            object animationKey = GetAnimationKey(directionToTarget);

            _anims.Update(animationKey);
            Position += directionToTarget * speed * Globals.Time;
            //Position = Vector2.Clamp(Position, _minPos, _maxPos); // Breaks the skeleton for some reason
        }

        // Method to convert the direction vector to an animation key object
        private Vector2 GetAnimationKey(Vector2 direction)
        {
            // Round the direction vector components to -1, 0, or 1
            int x = Math.Sign(direction.X);
            int y = Math.Sign(direction.Y);

            return new Vector2(x, y);
        }



        public override void Draw()
        {
            _anims.Draw(Position - origin, Color);
        }
    }
}
