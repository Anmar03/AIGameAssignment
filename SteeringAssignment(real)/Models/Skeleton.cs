using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SteeringAssignment_real.Mangers;

namespace SteeringAssignment_real.Models
{
    public enum SkeletonState
    {
        Idle,
        Attacking,
        Cooldown
    }
    public class Skeleton : Sprite
    {
        private SkeletonState currentState = SkeletonState.Idle;
        private const float speed = 150;
        private Vector2 _minPos, _maxPos;
        private Animation frame;
        private readonly AnimationManager _anims = new();
        private readonly AnimationManager _attackAnimation = new();
        private float frameWidth, frameHeight;
        public Vector2 origin;
        Texture2D attackTexture;
        public bool attacking = false;
        private const float pushForce = 600;
        private float attackDelayTimer = 0;
        private const float attackDelayDuration = 0.8f;
        private const float attackDamage = 2.0f;

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

            attackTexture = Globals.Content.Load<Texture2D>("Skeleton_attack");
            _attackAnimation.AddAnimation(new Vector2(0, 1), new Animation(attackTexture, 8, 8, 0.1f, 7));
            _attackAnimation.AddAnimation(new Vector2(-1, 0), new Animation(attackTexture, 8, 8, 0.1f, 1));
            _attackAnimation.AddAnimation(new Vector2(1, 0), new Animation(attackTexture, 8, 8, 0.1f, 5));
            _attackAnimation.AddAnimation(new Vector2(0, -1), new Animation(attackTexture, 8, 8, 0.1f, 3));
            _attackAnimation.AddAnimation(new Vector2(-1, 1), new Animation(attackTexture, 8, 8, 0.1f, 8));
            _attackAnimation.AddAnimation(new Vector2(-1, -1), new Animation(attackTexture, 8, 8, 0.1f, 2));
            _attackAnimation.AddAnimation(new Vector2(1, 1), new Animation(attackTexture, 8, 8, 0.1f, 6));
            _attackAnimation.AddAnimation(new Vector2(1, -1), new Animation(attackTexture, 8, 8, 0.1f, 4));
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
        public void Update(Player _player)
        {
            float distance = Vector2.Distance(Position, _player.Position);
            var directionToTarget = _player.Position - Position; directionToTarget.Normalize();
            object animationKey = GetAnimationKey(directionToTarget);
            Vector2 pushDirection = Vector2.Zero;

            switch (currentState)
            {
                case SkeletonState.Idle:
                    // If close to player, start attacking
                    if (distance < width / 2)
                    {
                        currentState = SkeletonState.Attacking;
                        _attackAnimation.Reset(); // Reset attack animation to start from the beginning
                    }
                    else
                    {
                        // Move towards the target
                        _anims.Update(animationKey);
                        Position += directionToTarget * speed * Globals.Time;
                        Position = Vector2.Clamp(Position, _minPos, _maxPos);
                    }
                    break;

                case SkeletonState.Attacking:
                    _attackAnimation.Update(animationKey);

                    // if in the middle of animation and still close, push the player line 128
                    if (_attackAnimation.CurrentFrame == 3 && distance < width) 
                    {
                        pushDirection = Vector2.Normalize(_player.Position - Position) * pushForce;
                    }
                    else if (_attackAnimation.CurrentFrame == _attackAnimation.TotalFrames - 1)
                    {
                        // Attack animation finished, start cooldown
                        currentState = SkeletonState.Cooldown;
                        attackDelayTimer = attackDelayDuration;
                    }
                    break;

                case SkeletonState.Cooldown:
                    // Handle cooldown after attack
                    if (attackDelayTimer > 0)
                    {
                        if (distance > width / 2)
                        {
                            // Move towards the target
                            _anims.Update(animationKey);
                            Position += directionToTarget * speed * Globals.Time;
                            Position = Vector2.Clamp(Position, _minPos, _maxPos);
                        }

                        attackDelayTimer -= Globals.Time;
                        if (attackDelayTimer <= 0)
                        {
                            attackDelayTimer = 0; 
                            currentState = SkeletonState.Idle; // Return to idle state after cooldown
                        }
                    }
                    break;
            }

            if (pushDirection != Vector2.Zero && _player.health > 0)
            {
                _player.health -= attackDamage;
                _player.Position += pushDirection * Globals.Time; 
                pushDirection = Vector2.Zero;
            }
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
            switch (currentState)
            {
                case SkeletonState.Attacking:
                    _attackAnimation.Draw(Position - origin, Color);
                    break;

                case SkeletonState.Idle:
                    _anims.Draw(Position - origin, Color);
                    break;

                case SkeletonState.Cooldown:
                    _anims.Draw(Position - origin, Color);
                    break;
            }
        } // end draw

    } // end class

}// end namespace
