using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SteeringAssignment_real.Mangers;
using SteeringAssignment_real.StateMachine;
using System.Collections.Generic;

namespace SteeringAssignment_real.Models
{
    public class Skeleton : Sprite
    {
        private readonly PathManager pathManager;
        private List<Vector2> shortestPath;
        private Vector2 _minPos, _maxPos;
        private readonly Animation frame;
        private readonly AnimationManager _anims = new();
        private readonly AnimationManager _attackAnimation = new();
        private readonly AnimationManager _deadAnimation = new();
        private readonly GameManager _gameManager;
        private readonly Texture2D attackTexture;
        private readonly Texture2D deadTexture;
        public readonly Random random = new();
        private const float pushForce = 600;
        private const float fireballPushForce = 1500;
        private float attackDelayTimer = 0;
        private const float attackDelayDuration = 0.8f;
        private const float attackDamage = 2.0f;
        private int node = 0;
        private const float PathUpdateInterval = 0.5f;
        private float pathUpdateTimer = 0.0f;
        private float radiusSquared;
        public Vector2 wanderDirection;
        public Vector2 turnSpeed;
        public Vector2 skeletonDirection;
        public float rSpeed = 0;
        public bool closeAttack;
        private Object deathAnimationKey;
        private Player _player;
        private Vector2 pushDirection;
        private int Ammo = 4;
        private Texture2D fireBallTexture;
        private FireBall fireBall;
        private float fireBallDamage = 4.0f;
        public bool fireballAttack = false;
        private Vector2 fireBallDirection;

        public Skeleton(Texture2D texture, Vector2 position, GameManager gameManager) : base(texture, position)
        {
            _gameManager = gameManager;
            pathManager = new(gameManager);
            shortestPath = new();
            skeletonDirection = gameManager._player.Position - Position; skeletonDirection.Normalize();
            turnSpeed = new(random.Next(-60, 60), random.Next(-60, 60));
            CurrentState = Wander.Instance();
            velocity = Vector2.Zero;

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

            fireBallTexture = Globals.Content.Load<Texture2D>("Fireball8");

            shortestPath = pathManager.AStar(Position, gameManager._player.Position);
            
            Health = 50f;
            speed = 150;
            AggroRadius = 600;
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            width = frame.frameWidth; 
            height = frame.frameHeight;
            Origin = new Vector2(width / 2, height / 2);
            radiusSquared = (width/2) * (width/2); 

            _minPos = new((-tileSize.X / 2) + width / 4, (-tileSize.Y / 2) + height / 3);
            _maxPos = new(mapSize.X - (tileSize.X / 2) - width / 4, mapSize.Y - (tileSize.X / 2) - height / 3);
        }

        public void Update(Player _player)
        {
            pushDirection = Vector2.Zero;
            this._player = _player;

            if (attackDelayTimer > 0)
            {
                attackDelayTimer -= Globals.Time;
            }

            if (Health <= 0 && CurrentState != Dead.Instance())
            {
                deathAnimationKey = AnimationManager.GetAnimationKey(skeletonDirection);
                ChangeState(Dead.Instance());
            }
            else
            {
                if (CurrentState != Wander.Instance())
                {
                    skeletonDirection = shortestPath[node] - Position; skeletonDirection.Normalize();
                    pathUpdateTimer += Globals.Time;

                    // If player moved and it has been 0.5 seconds then calculate shortest path
                    if (pathUpdateTimer >= PathUpdateInterval)
                    {
                        pathUpdateTimer = 0.0f;
                        shortestPath = pathManager.AStar(Position, _player.Position);
                        node = 0;
                    }

                    if (Vector2.DistanceSquared(Position, shortestPath[node]) < radiusSquared)
                    {
                        if (node < shortestPath.Count-1)
                        {
                            node++;
                        }
                        else
                        {
                            skeletonDirection = _player.Position - Position; skeletonDirection.Normalize();
                        }
                    }
                    
                }
            }
            
            CurrentState?.Execute(this);
            Position = Vector2.Clamp(Position, _minPos, _maxPos);

            if(fireballAttack)
            {
                fireBall.Update(fireBallDirection);
                float squaredDistance = Vector2.DistanceSquared(fireBall.Position, _player.Position);

                // If Fireball hits player, it disappears and damages player
                if (squaredDistance < Math.Pow(fireBall.width/2, 2))
                {
                    fireballAttack = false;
                    Vector2 pushDir = Vector2.Normalize(_player.Position - Position) * fireballPushForce;
                    _player.Health -= fireBallDamage;
                    _player.Position += pushDir * Globals.Time;
                    fireBall.GetLight().Intensity = 0;
                }

                if (IsOutOfBounds(fireBall.Position))
                {
                    fireballAttack = false;
                    fireBall.GetLight().Intensity = 0;
                }
            }

            if (pushDirection != Vector2.Zero && !_player.isDead())
            {
                _player.Health -= attackDamage;
                _player.Position += pushDirection * Globals.Time; 
            }
        }

        public override void Draw()
        {
            if (fireballAttack)
            {
                fireBall.Draw();
            }
            switch (CurrentState)
            {
                case Wander:
                    _anims.Draw(Position - Origin, Color);
                    break;

                case CloseAttack:
                    _attackAnimation.Draw(Position - Origin, Color);
                    break;

                case RangeAttack:
                    _anims.Draw(Position - Origin, Color);
                    break;

                case Aggro:
                    _anims.Draw(Position - Origin, Color);
                    if (InputManager.DebugMode)
                    {
                        foreach (var node in pathManager.GetConsideredNodes())
                        {
                            _gameManager.DrawPositionDebug(node);
                        }
                    }
                    break;

                case Dead:
                    _deadAnimation.Draw(Position - Origin, Color);
                    break;
            }
        } // end draw

        // Method Chooses Attack based on Fuzzy Logic
        public void SelectAttack()
        {
            float distance = Vector2.Distance(Position, _player.Position);

            double BestSoFar = double.MinValue;
            double rangeScore = RangeAttack.Instance().GetDesirability(distance, Health, Ammo);
            double closeScore = CloseAttack.Instance().GetDesirability(distance, Health, Ammo);

            if (rangeScore > BestSoFar)
            {
                BestSoFar = rangeScore;

                closeAttack = false;
            }
            
            if (closeScore > BestSoFar)
            {
                BestSoFar = closeScore;

                closeAttack = true;
            }
        }

        public void FireBallAttack()
        {
            if (Ammo > 0)
            {
                fireballAttack = true;
                fireBall = new(Position, fireBallTexture);
                fireBallDirection = DirectionToPlayer();
                _gameManager.AddLight(fireBall.GetLight());
                Ammo--;
            }
        }

        private bool IsOutOfBounds(Vector2 position)
        {
            // if fireball position is outside map boundary
            if (position.X < 0 || position.X > _gameManager.GetMapSize().X ||
                position.Y < 0 || position.Y > _gameManager.GetMapSize().Y)
            {
                return true; 
            }
            return false; 
        }

        public void ChangeState(State newState)
        {
            CurrentState.Exit(this);

            CurrentState = newState;

            CurrentState.Enter(this);
        }

        public bool ObstacleProx()
        {
            return _gameManager.ObstacleProximity(Position, width/2);
        }
        public bool IsDead()
        {
            return CurrentState == Dead.Instance();
        }
        public float GetAttackTimer() => attackDelayTimer;
        public void SetAttackTimer()
        {
            attackDelayTimer = attackDelayDuration;
        }
        public float GetPushForce() => pushForce;
        public void SetPushDirection(Vector2 newDirection)
        {
            pushDirection = newDirection;
        }

        public Vector2 DirectionToPlayer()
        {
            Vector2 direction = _player.Position - Position; direction.Normalize();
            return direction;
        }

        public void SetCollision()
        {
            EntityCollision = false;
        }
        public Player GetPlayer() => _player;
        public AnimationManager GetWalkingAnim() => _anims;
        public AnimationManager GetCloseAttackAnim() => _attackAnimation;
        public AnimationManager GetDeathAnim() => _deadAnimation;
        public object GetDeathAnimKey() => deathAnimationKey;

    } // end class

}// end namespace
