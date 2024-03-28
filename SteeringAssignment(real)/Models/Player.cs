using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Mangers;

namespace SteeringAssignment_real.Models
{
    public enum PlayerState
    {
        Walk,
        FistAttack,
        SwordAttack,
        Dead
    }
    public class Player : Sprite
    {
        private PlayerState currentState = PlayerState.Walk;
        private const float speed = 400;
        private Vector2 _minPos, _maxPos;
        private readonly Animation frame;
        private readonly AnimationManager _anims = new();
        private readonly AnimationManager _fistAttackAnim = new();
        private readonly AnimationManager _swordAttackAnim = new();
        private readonly Texture2D fistAttackTexture;
        private readonly Texture2D swordAttackTexture;

        private float frameWidth, frameHeight;
        public Vector2 origin;
        private const float punchPushForce = 700;
        private const float swordPushForce = 500;
        private const float fistAttackDamage = 2.0f;
        private const float swordAttackDamage = 5.0f;
        private object lastKey;
        private float attackDelayTimer = 0;
        private const float attackDelayDuration = 0.5f;

        public Player(Texture2D texture, Vector2 position) : base(texture, position)
        {
            Health = 50f;

            frame = new Animation(texture, 10, 8, 0.1f, 5);
            _anims.AddAnimation(new Vector2(0, 1), frame); // S
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 10, 8, 0.1f, 7)); // A
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 10, 8, 0.1f, 3)); // D
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 10, 8, 0.1f, 1)); // W
            _anims.AddAnimation(new Vector2(-1, 1), new Animation(texture, 10, 8, 0.1f, 6)); // SA
            _anims.AddAnimation(new Vector2(-1, -1), new Animation(texture, 10, 8, 0.1f, 8)); // WA
            _anims.AddAnimation(new Vector2(1, 1), new Animation(texture, 10, 8, 0.1f, 4)); // SD
            _anims.AddAnimation(new Vector2(1, -1), new Animation(texture, 10, 8, 0.1f, 2)); // WD

            fistAttackTexture = Globals.Content.Load<Texture2D>("fist_attack");
            _fistAttackAnim.AddAnimation(new Vector2(0, 1), new Animation(fistAttackTexture, 9, 8, 0.1f, 5)); // S
            _fistAttackAnim.AddAnimation(new Vector2(-1, 0), new Animation(fistAttackTexture, 9, 8, 0.1f, 7)); // A
            _fistAttackAnim.AddAnimation(new Vector2(1, 0), new Animation(fistAttackTexture, 9, 8, 0.1f, 3)); // D
            _fistAttackAnim.AddAnimation(new Vector2(0, -1), new Animation(fistAttackTexture, 9, 8, 0.1f, 1)); // W
            _fistAttackAnim.AddAnimation(new Vector2(-1, 1), new Animation(fistAttackTexture, 9, 8, 0.1f, 6)); // SA
            _fistAttackAnim.AddAnimation(new Vector2(-1, -1), new Animation(fistAttackTexture, 9, 8, 0.1f, 8)); // WA
            _fistAttackAnim.AddAnimation(new Vector2(1, 1), new Animation(fistAttackTexture, 9, 8, 0.1f, 4)); // SD
            _fistAttackAnim.AddAnimation(new Vector2(1, -1), new Animation(fistAttackTexture, 9, 8, 0.1f, 2)); // WD

            swordAttackTexture = Globals.Content.Load<Texture2D>("shword_attack");
            _swordAttackAnim.AddAnimation(new Vector2(0, 1), new Animation(swordAttackTexture, 9, 8, 0.1f, 5)); // S
            _swordAttackAnim.AddAnimation(new Vector2(-1, 0), new Animation(swordAttackTexture, 9, 8, 0.1f, 7)); // A
            _swordAttackAnim.AddAnimation(new Vector2(1, 0), new Animation(swordAttackTexture, 9, 8, 0.1f, 3)); // D
            _swordAttackAnim.AddAnimation(new Vector2(0, -1), new Animation(swordAttackTexture, 9, 8, 0.1f, 1)); // W
            _swordAttackAnim.AddAnimation(new Vector2(-1, 1), new Animation(swordAttackTexture, 9, 8, 0.1f, 6)); // SA
            _swordAttackAnim.AddAnimation(new Vector2(-1, -1), new Animation(swordAttackTexture, 9, 8, 0.1f, 8)); // WA
            _swordAttackAnim.AddAnimation(new Vector2(1, 1), new Animation(swordAttackTexture, 9, 8, 0.1f, 4)); // SD
            _swordAttackAnim.AddAnimation(new Vector2(1, -1), new Animation(swordAttackTexture, 9, 8, 0.1f, 2)); // WD
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            frameWidth = frame.frameWidth;
            frameHeight = frame.frameHeight;
            width = frameWidth;
            height = frameHeight;
            origin = new Vector2(frameWidth / 2, frameHeight / 2);

            _minPos = new(-tileSize.X / 2 + frameWidth / 3, -tileSize.Y / 2 + frameHeight / 2);
            _maxPos = new(mapSize.X - tileSize.X / 2 - frameWidth / 3, mapSize.Y - tileSize.X / 2 - frameHeight / 2);
        }

        public void Update(CollisionManager collisionManager)
        {
            Vector2 pushDirection = Vector2.Zero;
            object animationKey = AnimationManager.GetAnimationKey(InputManager.Direction);
            float distance = 0;
            Vector2 direction = Vector2.Zero;
            float enemyHealth = 0;

            // Check if the spacebar is pressed and the player is in the walking state
            if (Health <= 0)
            {
                currentState = PlayerState.Dead;
            }

            if (currentState == PlayerState.FistAttack || currentState == PlayerState.SwordAttack)
            {
                var result = collisionManager.ClosestEntityInfo(Position);
                distance = result.distance;
                direction = result.direction;
                enemyHealth = result.health;
            }

            switch (currentState)
            {
                case PlayerState.Walk:
                    if (attackDelayTimer > 0)
                    {
                        attackDelayTimer -= Globals.Time;
                    }

                    if (InputManager.Moving)
                    {
                        Position += Vector2.Normalize(InputManager.Direction) * speed * Globals.Time;
                        lastKey = AnimationManager.GetAnimationKey(InputManager.LastDirection);
                    }
                    else if (InputManager.SpacebarPressed && attackDelayTimer <= 0)
                    {
                        currentState = PlayerState.FistAttack;
                    }
                    else if (InputManager.F_keyPressed && attackDelayTimer <= 0)
                    {
                        currentState = PlayerState.SwordAttack;
                    }

                    _anims.Update(animationKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);
                    break;

                case PlayerState.FistAttack:
                    _fistAttackAnim.Update(lastKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);

                    // if in the middle of animation and still close, push the player line 128 TO-DO: ADD CHECK IF PLAYER IS FACING ENEMY
                    if (_fistAttackAnim.CurrentFrame == 5 && distance < width + width/2)
                    {
                        pushDirection = direction * punchPushForce;
                    }
                    
                    if (_fistAttackAnim.CurrentFrame == _fistAttackAnim.TotalFrames - 1)
                    {
                        currentState = PlayerState.Walk;
                        _fistAttackAnim.Reset();

                        attackDelayTimer = attackDelayDuration;
                    }
                    break;

                case PlayerState.SwordAttack:
                    _swordAttackAnim.Update(lastKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);

                    if (_swordAttackAnim.CurrentFrame == 4 && distance < width * 2)
                    {
                        pushDirection = direction * swordPushForce;
                    }

                    if (_swordAttackAnim.CurrentFrame == _swordAttackAnim.TotalFrames - 1)
                    {
                        currentState = PlayerState.Walk;
                        _swordAttackAnim.Reset();

                        attackDelayTimer = attackDelayDuration;
                    }
                    break;

                case PlayerState.Dead:
                break;
            }

            if (pushDirection != Vector2.Zero && enemyHealth > 0)
            {
                enemyHealth -= GetAttackDamage();
                collisionManager.SetClosestEntityHealth(Position, enemyHealth);
                collisionManager.SetClosestEntityPosition(Position, pushDirection);
            }
        }

        private float GetAttackDamage()
        {
            switch (currentState)
            {
                case PlayerState.FistAttack:
                    return fistAttackDamage;
                    
                case PlayerState.SwordAttack:
                    return swordAttackDamage;
            }
            return -1;
        }

        public override void Draw()
        {
            switch (currentState)
            {
                case PlayerState.Walk:
                    _anims.Draw(Position - origin, Color);
                    break;

                case PlayerState.FistAttack:
                    _fistAttackAnim.Draw(Position - origin, Color);
                    break;

                case PlayerState.SwordAttack:
                    _swordAttackAnim.Draw(Position - origin, Color);
                    break;

                case PlayerState.Dead:
                    _fistAttackAnim.Draw(Position - origin, Color);
                    string message = "YOU ARE DEAD!";
                    Vector2 messageSize = Globals.Font.MeasureString(message);
                    Vector2 messagePosition = new((Position.X - messageSize.X / 2), (Position.Y - messageSize.Y * 2));
                    Globals.SpriteBatch.DrawString(Globals.Font, message, messagePosition, Color.Red);
                    break;
            }
           
        }
    }
}
