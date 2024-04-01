using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SteeringAssignment_real.Mangers;
using System.Collections.Generic;
using SteeringAssignment_real.FuzzyLogic.fuzzyOperators;
using SteeringAssignment_real.FuzzyLogic;

namespace SteeringAssignment_real.Models
{
    public class Skeleton : Sprite
    {
        public State currentState = State.Wander;
        private readonly PathManager pathManager;
        private readonly StateMachine stateMachine;
        private List<Vector2> shortestPath;
        private const float speed = 150;
        private Vector2 _minPos, _maxPos;
        private readonly Animation frame;
        private readonly AnimationManager _anims = new();
        private readonly AnimationManager _attackAnimation = new();
        private readonly AnimationManager _deadAnimation = new();
        private readonly GameManager _gameManager;
        private float frameWidth, frameHeight;
        public Vector2 origin;
        readonly Texture2D attackTexture;
        readonly Texture2D deadTexture;
        private Random random = new();
        private const float pushForce = 600;
        private float attackDelayTimer = 0;
        private const float attackDelayDuration = 0.8f;
        private const float attackDamage = 2.0f;
        private int node = 0;
        private const float PathUpdateInterval = 0.5f;
        private float pathUpdateTimer = 0.0f;
        private float radiusSquared;
        private Vector2 wanderTargetDirection = Vector2.Zero;
        private Vector2 turnSpeed;
        private float wanderDuration = 0;
        public Vector2 skeletonDirection;

        public void FuzzyLogicInit()
        {
            FuzzyModule fm = new();

            FuzzyVariable Desirability = fm.CreateFLV("Desirability");
            FzSet VeryDesirable = Desirability.AddRightShoulderSet("VeryDesirable", 50, 75, 100);
            FzSet Desirable = Desirability.AddTriangularSet("Desirable", 25, 50, 75);
            FzSet Undesirable = Desirability.AddLeftShoulderSet("Undesirable", 0, 25, 50);

            FuzzyVariable DistToTarget = fm.CreateFLV("DistToTaget");
            FzSet Target_Close = DistToTarget.AddLeftShoulderSet("Target_Close", 0, 25, 150);
            FzSet Target_Medium = DistToTarget.AddTriangularSet("Target_Medium", 25, 50, 300);
            FzSet Target_Far = DistToTarget.AddRightShoulderSet("Target_Far", 150, 300, 500);

            FuzzyVariable Health = fm.CreateFLV("Health");
            FzSet Health_Low = Health.AddLeftShoulderSet("Health_Low", 0, 2.5, 15);
            FzSet Health_Half = Health.AddTriangularSet("Health_Half", 2.5, 5, 30);
            FzSet Health_High = Health.AddRightShoulderSet("Health_High", 15, 30, 50);

            fm.AddRule(new FzAND(Target_Far, Health_Low), Undesirable);
        }

        public Skeleton(Texture2D texture, Vector2 position, GameManager gameManager) : base(texture, position)
        {
            _gameManager = gameManager;
            pathManager = new PathManager(gameManager);
            shortestPath = new List<Vector2>();
            skeletonDirection = gameManager._player.Position - Position; skeletonDirection.Normalize();
            turnSpeed = new(random.Next(-100, 100), random.Next(-100, 100));
            stateMachine = new(this);

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
            AggroRadius = 600;
            FuzzyLogicInit();
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            frameWidth = frame.frameWidth;
            frameHeight = frame.frameHeight;
            width = frameWidth;
            height = frameHeight;
            origin = new Vector2(frameWidth / 2, frameHeight / 2);
            radiusSquared = (width/2) * (width/2); 

            _minPos = new((-tileSize.X / 2) + frameWidth / 4, (-tileSize.Y / 2) + frameHeight / 3);
            _maxPos = new(mapSize.X - (tileSize.X / 2) - frameWidth / 4, mapSize.Y - (tileSize.X / 2) - frameHeight / 3);
        }

        public void Update(Player _player)
        {
            Vector2 pushDirection = Vector2.Zero;
            
            float distance = 0;
            Object animationKey;

            if (Health <= 0 && currentState != State.Dead)
            {
                currentState = State.Dead;
                skeletonDirection = shortestPath[node] - Position; skeletonDirection.Normalize();
            }
            else
            {
                distance = Vector2.Distance(Position, _player.Position);

                if (currentState != State.Wander)
                {
                    pathUpdateTimer += Globals.Time;

                    // If player moved and it has been 0.5 seconds then calculate shortest path
                    if (pathUpdateTimer >= PathUpdateInterval)
                    {
                        pathUpdateTimer = 0.0f;
                        shortestPath = pathManager.AStar(Position, _player.Position);
                        node = 0;
                    }

                    if (shortestPath.Count == 0) // if shortest path fails
                    {
                        skeletonDirection = _player.Position - Position; skeletonDirection.Normalize();
                    }
                    else
                    {
                        skeletonDirection = shortestPath[node] - Position; skeletonDirection.Normalize();

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
            }
            animationKey = AnimationManager.GetAnimationKey(skeletonDirection);

            switch (currentState)
            {
                case State.Wander: 
                    if (distance < AggroRadius && !_player.isDead())
                    {
                        currentState = State.Aggro;
                        break;
                    }
                    else
                    {
                        // If the wander target direction is not set, initialize it to a random direction
                        if (wanderTargetDirection == Vector2.Zero)
                        {
                            wanderTargetDirection = new Vector2(random.Next(-1, 2), random.Next(-1, 2));
                            wanderTargetDirection.Normalize();
                        }

                        // Gradually turn towards the wander target direction
                        if (skeletonDirection != wanderTargetDirection)
                        {
                            skeletonDirection += turnSpeed; skeletonDirection.Normalize();
                        }

                        // Update position based on the wander direction and speed
                        Position += skeletonDirection * speed * Globals.Time;
                        Position = Vector2.Clamp(Position, _minPos, _maxPos);

                        // If wander duration elapsed, reset wander
                        if (wanderDuration <= 0)
                        {
                            wanderDuration = (float)random.NextDouble() * 3.0f;
                            wanderTargetDirection = Vector2.Zero; 
                            turnSpeed = new(random.Next(-10, 10), random.Next(-10, 10));
             
                        }
                        else
                        {
                            wanderDuration -= Globals.Time;
                        }
                        animationKey = AnimationManager.GetAnimationKey(skeletonDirection);
                        _anims.Update(animationKey);
                    }
                    break;


                case State.Aggro:
                    // If out of aggro radius or Player is dead, go back to idling
                    if (distance > AggroRadius || _player.isDead())
                    {
                        currentState = State.Wander;
                        break;
                    }

                    if (attackDelayTimer > 0)
                    {
                        attackDelayTimer -= Globals.Time;
                    }

                    // If close to player, start attacking
                    if (distance < width / 2 && attackDelayTimer <= 0)
                    {
                        currentState = State.Attacking;
                        _attackAnimation.Reset(); // Reset attack animation to start from the beginning
                    }
                    else
                    {
                        _anims.Update(animationKey);

                        Position += skeletonDirection * speed * Globals.Time;
                        Position = Vector2.Clamp(Position, _minPos, _maxPos);
                    }
                    break;

                case State.Attacking:
                    _attackAnimation.Update(animationKey);

                    // if in the middle of animation and still close, push the player line 168
                    if (_attackAnimation.CurrentFrame == 3 && distance < width) 
                    {
                        pushDirection = Vector2.Normalize(_player.Position - Position) * pushForce;
                    }
                    else if (_attackAnimation.CurrentFrame == _attackAnimation.TotalFrames - 1)
                    {
                        // Attack animation finished. start cooldown
                        currentState = State.Aggro;
                        attackDelayTimer = attackDelayDuration;
                    }
                    break;

                case State.Dead:
                    
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);
                    EntityCollision = false;

                    if (_deadAnimation.CurrentFrame == _deadAnimation.TotalFrames - 1) 
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
            }
        }

        public override void Draw()
        {
            switch (currentState)
            {
                case State.Wander:
                    _anims.Draw(Position - origin, Color);
                    break;

                case State.Attacking:
                    _attackAnimation.Draw(Position - origin, Color);
                    break;

                case State.Aggro:
                    _anims.Draw(Position - origin, Color);
                    if (InputManager.DebugMode)
                    {
                        foreach (var node in shortestPath)
                        {
                            _gameManager.DrawPositionDebug(node);
                        }
                    }
                    break;

                case State.Dead:
                    _deadAnimation.Draw(Position - origin, Color);
                    break;
            }
        } // end draw

    } // end class

}// end namespace
