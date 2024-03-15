using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing.Imaging;
using Point = Microsoft.Xna.Framework.Point;


namespace SteeringAssignment_real
{
    public static class Globals
    {
        public static float Time { get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDevice GraphicsDevice { get; set; }
        public static GraphicsDeviceManager Graphics { get; set; }
        public static Point WindowSize { get; set; }
        public static SpriteFont Font { get; set; }
        public static Texture2D Pixel { get; private set; }

        public static void Initialize()
        {
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new Color[] { Color.White });
        }
        public static void Update(GameTime gt)
        {
            Time = (float)gt.ElapsedGameTime.TotalSeconds;
        }

    }
}
