using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SteeringAssignment_real.Mangers;
using System.Collections.Generic;
using SharpDX.Direct2D1;

namespace SteeringAssignment_real.Models
{
    public enum SkeletonState
    {
        Idle,
        Attacking,
        Cooldown,
        Dead
    }
    public class Skeleton : Sprite
    {
        private SkeletonState currentState = SkeletonState.Idle;
        private readonly PathManager pathManager;
        private List<Vector2> shortestPath;
        private const float speed = 150;
        private Vector2 _minPos, _maxPos;
        private Animation frame;
        private readonly AnimationManager _anims = new();
        private readonly AnimationManager _attackAnimation = new();
        private readonly AnimationManager _deadAnimation = new();
        private float frameWidth, frameHeight;
        public Vector2 origin;
        Texture2D attackTexture;
        Texture2D deadTexture;
        private const float pushForce = 600;
        private float attackDelayTimer = 0;
        private const float attackDelayDuration = 0.8f;
        private const float attackDamage = 2.0f;
        private int node = 0;
        private const float PathUpdateInterval = 0.5f; 
        private float pathUpdateTimer = 0.0f;
        private float radiusSquared;


        public Skeleton(Texture2D texture, Vector2 position, GameManager gameManager) : base(texture, position)
        {
            pathManager = new PathManager(gameManager);
            shortestPath = new List<Vector2>();

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

            deadTexture = Globals.Content.Load<Texture2D>("skeleton_die");
            _deadAnimation.AddAnimation(new Vector2(0, 1), new Animation(deadTexture, 8, 8, 0.1f, 7));
            _deadAnimation.AddAnimation(new Vector2(-1, 0), new Animation(deadTexture, 8, 8, 0.1f, 1));
            _deadAnimation.AddAnimation(new Vector2(1, 0), new Animation(deadTexture, 8, 8, 0.1f, 5));
            _deadAnimation.AddAnimation(new Vector2(0, -1), new Animation(deadTexture, 8, 8, 0.1f, 3));
            _deadAnimation.AddAnimation(new Vector2(-1, 1), new Animation(deadTexture, 8, 8, 0.1f, 8));
            _deadAnimation.AddAnimation(new Vector2(-1, -1), new Animation(deadTexture, 8, 8, 0.1f, 2));
            _deadAnimation.AddAnimation(new Vector2(1, 1), new Animation(deadTexture, 8, 8, 0.1f, 6));
            _deadAnimation.AddAnimation(new Vector2(1, -1), new Animation(deadTexture, 8, 8, 0.1f, 4));

            shortestPath = pathManager.AStar(Position, gameManager._player.Position);

            Health = 50f;
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            frameWidth = frame.frameWidth;
            frameHeight = frame.frameHeight;
            width = frameWidth;
            height = frameHeight;
            origin = new Vector2(frameWidth / 2, frameHeight / 2);
            radiusSquared = (width) * (width); 

            _minPos = new((-tileSize.X / 2) + frameWidth / 4, (-tileSize.Y / 2) + frameHeight / 3);
            _maxPos = new(mapSize.X - (tileSize.X / 2) - frameWidth / 4, mapSize.Y - (tileSize.X / 2) - frameHeight / 3);
        }

        public void Update(Player _player)
        {
            Vector2 pushDirection = Vector2.Zero;
            Vector2 directionToTarget;
            float distance = 0;

            if (Health <= 0 && currentState != SkeletonState.Dead)
            {
                currentState = SkeletonState.Dead;
                directionToTarget = shortestPath[node] - Position; directionToTarget.Normalize();
            }
            else
            {
                distance = Vector2.Distance(Position, _player.Position);

                pathUpdateTimer += Globals.Time;

                // if player moved and it has been 0.5 seconds then calculate shortest path
                if (pathUpdateTimer >= PathUpdateInterval)
                {
                    pathUpdateTimer = 0.0f;
                    shortestPath = pathManager.AStar(Position, _player.Position);
                    node = 0;
                }
                directionToTarget = shortestPath[node] - Position; directionToTarget.Normalize();

                if (Vector2.DistanceSquared(shortestPath[node], Position) < radiusSquared)
                {
                    if (node + 1 < shortestPath.Count)
                    {
                        node++;
                    }
                    else
                    {
                        directionToTarget = _player.Position - Position; directionToTarget.Normalize();
                    }
                }
                else
                {
                    directionToTarget = shortestPath[node] - Position; directionToTarget.Normalize();
                }
            }

            object animationKey = AnimationManager.GetAnimationKey(directionToTarget);

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
                        _anims.Update(animationKey);

                        // Move towards the target TO-DO: Implement path manager
                        Position += directionToTarget * speed * Globals.Time;
                        Position = Vector2.Clamp(Position, _minPos, _maxPos);
                    }
                    break;

                case SkeletonState.Attacking:
                    _attackAnimation.Update(animationKey);

                    // if in the middle of animation and still close, push the player line 168
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

                case SkeletonState.Dead:
                    
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);

                    if (_deadAnimation.CurrentFrame == _deadAnimation.TotalFrames - 1) // when animation done the skeleton stands back up. Need to stay on last frame
                    {
                        _deadAnimation.UpdateDeath(Vector2.Zero);
                    }
                    else
                    {
                        _deadAnimation.Update(animationKey);
                    }
                    break;
            }

            if (pushDirection != Vector2.Zero && _player.Health > 0)
            {
                _player.Health -= attackDamage;
                _player.Position += pushDirection * Globals.Time; 
                pushDirection = Vector2.Zero;
            }
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
                    foreach (var node in shortestPath)
                    {
                        DrawPositionDebug(node);
                    }
                    break;

                case SkeletonState.Cooldown:
                    _anims.Draw(Position - origin, Color);
                    break;

                case SkeletonState.Dead:
                    _deadAnimation.Draw(Position - origin, Color);
                    break;
            }
        } // end draw

    } // end class

}// end namespace
