﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.Mangers
{
    public class UIManager
    {
        private Texture2D healthTextureFull;
        private Texture2D healthTextureHalf;
        private Texture2D healthTextureEmpty;
        private Vector2 healthPos;
        private Vector2 healthOrigin;
        private Player _player;
        private Matrix _uiTransform;
        public Color Color { get; set; }

        public UIManager(Player _player) 
        {
            this._player = _player;

            healthTextureFull = Globals.Content.Load<Texture2D>("heart-full");
            //healthTextureHalf = Globals.Content.Load<Texture2D>("heart-half");
            //healthTextureEmpty = Globals.Content.Load<Texture2D>("heart-empty");

            healthOrigin = new(healthTextureFull.Width / 2, healthTextureFull.Height / 2);
            UpdateUITransform();
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _uiTransform);

            healthPos = new Vector2(Globals.WindowSize.X * 0.8f, Globals.WindowSize.Y * 0.95f);

            for (int i = 0; i < _player.Health / 10; i++)
            {
                Globals.SpriteBatch.Draw(healthTextureFull, healthPos, null, Color.White, 0f, healthOrigin, 1f, SpriteEffects.None, 0f);
                healthPos.X += healthTextureFull.Width;
            }

            if (_player.GetPlayerState() == PlayerState.Dead)
            {
                string message = "YOU ARE DEAD!";
                Vector2 messageSize = Globals.Font.MeasureString(message);
                Vector2 messagePosition = new((Globals.WindowSize.X/2 - messageSize.X / 2), (Globals.WindowSize.Y/2 - messageSize.Y * 2));
                Globals.SpriteBatch.DrawString(Globals.Font, message, messagePosition, Color.Red);
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
