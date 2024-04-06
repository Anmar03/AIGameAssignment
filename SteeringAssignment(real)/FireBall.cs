using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Mangers;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real
{
    public class FireBall : Sprite
    {
        private readonly Animation frame;
        private readonly AnimationManager fireBallAnim = new();
        private object animKey;
        private Light light;
        private float lightRadius = 450f;

        public FireBall(Vector2 position, Texture2D fireBall) : base(fireBall, position)
        {
            frame = new Animation(fireBall, 14, 8, 0.1f, 7);
            fireBallAnim.AddAnimation(new Vector2(0, 1), frame); // S
            fireBallAnim.AddAnimation(new Vector2(-1, 0), new Animation(fireBall, 14, 8, 0.1f, 1)); // A
            fireBallAnim.AddAnimation(new Vector2(1, 0), new Animation(fireBall, 14, 8, 0.1f, 5)); // D
            fireBallAnim.AddAnimation(new Vector2(0, -1), new Animation(fireBall, 14, 8, 0.1f, 3)); // W
            fireBallAnim.AddAnimation(new Vector2(-1, 1), new Animation(fireBall, 14, 8, 0.1f, 8)); // SA
            fireBallAnim.AddAnimation(new Vector2(-1, -1), new Animation(fireBall, 14, 8, 0.1f, 2)); // WA
            fireBallAnim.AddAnimation(new Vector2(1, 1), new Animation(fireBall, 14, 8, 0.1f, 6)); // SD
            fireBallAnim.AddAnimation(new Vector2(1, -1), new Animation(fireBall, 14, 8, 0.1f, 4)); // WD

            light = new(position, lightRadius, 0.9f, 1);

            width = frame.frameWidth;
            height = frame.frameHeight;
            Origin = new Vector2(width / 2, height / 2);
            speed = 500f;
        }

        public void Update(Vector2 Direction)
        {
            animKey = AnimationManager.GetAnimationKey(Direction); // use 16kk 

            if (animKey != null)
            {
                Position += Direction * speed * Globals.Time;
                light.Position = Position + Origin;
                fireBallAnim.Update(animKey);
            }
        }

        public override void Draw()
        {
            fireBallAnim.Draw(Position - Origin, Color.White);
        }

        public Light GetLight()
        {
            return light;
        }
    }
}
