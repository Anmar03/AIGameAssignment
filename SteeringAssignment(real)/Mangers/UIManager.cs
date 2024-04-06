using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.Mangers
{
    public class UIManager
    {
        private Texture2D healthTextureFull;
        private static Vector2 healthPos;
        private Vector2 healthOrigin;
        private Player _player;
        private Matrix _uiTransform;
        private static string debugPrompt;
        private static Vector2 debugPromptPos;
        private static int margin = 50;
        private static string deathMessage;
        private static Vector2 deathMessageSize;
        private static Vector2 deathMessagePosition;
        public Color Color { get; set; }

        public UIManager(Player _player) 
        {
            this._player = _player;

            InitPrompts();

            healthTextureFull = Globals.Content.Load<Texture2D>("heart-full");
            healthOrigin = new(healthTextureFull.Width / 2, healthTextureFull.Height / 2);
            UpdateUITransform();
        }

        private void InitPrompts()
        {
            debugPrompt = "Press \'T\' Key to enter DebugMode";
            debugPromptPos = new(margin/2, margin/2);

            deathMessage = "GAME OVER!";
            deathMessageSize = Globals.Font.MeasureString(deathMessage);
            deathMessagePosition = new((Globals.WindowSize.X / 2 - deathMessageSize.X), (Globals.WindowSize.Y / 2 - deathMessageSize.Y * 2));
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _uiTransform);

            Globals.SpriteBatch.DrawString(Globals.Font, debugPrompt, debugPromptPos, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

            healthPos = new Vector2(Globals.WindowSize.X * 0.8f, Globals.WindowSize.Y * 0.95f);

            for (int i = 0; i < _player.Health / 10; i++)
            {
                Globals.SpriteBatch.Draw(healthTextureFull, healthPos, null, Color.White, 0f, healthOrigin, 1f, SpriteEffects.None, 0f);
                healthPos.X += healthTextureFull.Width;
            }

            if (_player.isDead())
            {
                Globals.SpriteBatch.DrawString(Globals.Font, deathMessage, deathMessagePosition, Color.Red, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
            }

            Globals.SpriteBatch.End();
        }

        private void UpdateUITransform()
        {
            // Calculate the UI transformation matrix to keep UI fixed on the screen
            float uiScale = 1.0f;
            _uiTransform = Matrix.CreateScale(uiScale);
        }
    }
}
