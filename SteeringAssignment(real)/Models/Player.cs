using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Mangers;

namespace SteeringAssignment_real.Models
{
    public enum PlayerState
    {
        Walk,
        FistAttack,
        Dead
    }
    public class Player : Sprite
    {
        private PlayerState currentState = PlayerState.Walk;
        private const float speed = 500;
        private Vector2 _minPos, _maxPos;
        private Animation frame;
        private readonly AnimationManager _anims = new();
        private readonly AnimationManager _fistAttackAnim = new();
        private float frameWidth, frameHeight;
        public Vector2 origin;
        private Texture2D fistAttackTexture;
        private const float punchPushForce = 700;
        private float attackDelayTimer = 0;
        private const float attackDelayDuration = 0f;
        private const float attackDamage = 2.0f;
        private object lastKey;


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
            var result = collisionManager.ClosestEntityInfo(Position);
            float distance = result.distance;
            Vector2 direction = result.direction;
            Vector2 pushDirection = Vector2.Zero;
            float enemyHealth = result.health;
            Vector2 enemyPosition = result.enemyPosition;
            object animationKey = AnimationManager.GetAnimationKey(InputManager.Direction);

            // Check if the spacebar is pressed and the player is in the walking state
            if (InputManager.SpacebarPressed && currentState == PlayerState.Walk && attackDelayTimer <= 0)
            {
                currentState = PlayerState.FistAttack; 
            }

            switch (currentState)
            {
                case PlayerState.Walk:
                    if (InputManager.Moving)
                    {
                        Position += Vector2.Normalize(InputManager.Direction) * speed * Globals.Time;

                        lastKey = AnimationManager.GetAnimationKey(InputManager.LastDirection);
                    }

                    if (Health <= 0)
                    {
                        currentState = PlayerState.Dead; 
                        break;
                    }

                    if (attackDelayTimer > 0)
                    {
                        attackDelayTimer -= Globals.Time;
                    }

                    _anims.Update(animationKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);
                    break;

                case PlayerState.FistAttack:
                    _fistAttackAnim.Update(lastKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);

                    // if in the middle of animation and still close, push the player line 128
                    if (_fistAttackAnim.CurrentFrame == 5 && distance < width + width/2)
                    {
                        pushDirection = direction * punchPushForce;
                    }
                    
                    if (_fistAttackAnim.CurrentFrame == _fistAttackAnim.TotalFrames - 1)
                    {
                        // Attack animation finished, start cooldown
                        currentState = PlayerState.Walk;
                        attackDelayTimer = attackDelayDuration;
                    }
                    break;

                case PlayerState.Dead:

                break;
            }

            if (pushDirection != Vector2.Zero && enemyHealth > 0)
            {
                enemyHealth -= attackDamage;
                collisionManager.setClosestEntityHealth(Position, enemyHealth);
                enemyPosition += pushDirection;
                collisionManager.setClosestEntityPosition(Position, pushDirection);

                pushDirection = Vector2.Zero;
            }
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

                case PlayerState.Dead:
                    _fistAttackAnim.Draw(Position - origin, Color);
                    string message = "YOU ARE DEAD!";
                    Vector2 messageSize = Globals.Font.MeasureString(message);
                    Vector2 messagePosition = new Vector2((Position.X - messageSize.X / 2), (Position.Y - messageSize.Y * 2));
                    Globals.SpriteBatch.DrawString(Globals.Font, message, messagePosition, Color.Red);
                    break;
            }
           
        }
    }
}
