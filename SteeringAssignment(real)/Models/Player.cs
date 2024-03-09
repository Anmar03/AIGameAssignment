using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Mangers;

namespace SteeringAssignment_real.Models
{
    public class Player : Sprite
    {
        private const float speed = 500;
        private Vector2 _minPos, _maxPos;
        private Animation frame;
        private readonly AnimationManager _anims = new();
        private float frameWidth, frameHeight;
        public Vector2 origin;

        public Player(Texture2D texture, Vector2 position) : base(texture, position)
        {
            frame = new Animation(texture, 8, 8, 0.1f, 1);
            _anims.AddAnimation(new Vector2(0, 1), frame);
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 8, 8, 0.1f, 2));
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 8, 8, 0.1f, 3));
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 8, 8, 0.1f, 4));
            _anims.AddAnimation(new Vector2(-1, 1), new Animation(texture, 8, 8, 0.1f, 5));
            _anims.AddAnimation(new Vector2(-1, -1), new Animation(texture, 8, 8, 0.1f, 6));
            _anims.AddAnimation(new Vector2(1, 1), new Animation(texture, 8, 8, 0.1f, 7));
            _anims.AddAnimation(new Vector2(1, -1), new Animation(texture, 8, 8, 0.1f, 8));
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            frameWidth = frame.frameWidth;
            frameHeight = frame.frameHeight;
            width = frameWidth;
            height = frameHeight;
            origin = new Vector2(frameWidth / 2, frameHeight / 2);

            _minPos = new(-tileSize.X / 2 + frameWidth / 2, -tileSize.Y / 2 + frameHeight / 2);
            _maxPos = new(mapSize.X - tileSize.X / 2 - frameWidth / 2, mapSize.Y - tileSize.X / 2 - frameHeight / 2);
        }

        public void Update()
        {
            if (InputManager.Moving)
            {
                Position += Vector2.Normalize(InputManager.Direction) * speed * Globals.Time;
            }

            _anims.Update(InputManager.Direction);
            Position = Vector2.Clamp(Position, _minPos, _maxPos);
        }

        public override void Draw()
        {
            _anims.Draw(Position - origin, Color);
        }
    }
}
