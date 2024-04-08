using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;
using SteeringAssignment_real.StateMachine;

namespace SteeringAssignment_real.Mangers
{
    public class UIManager
    {
        private readonly Texture2D healthTextureFull;
        private readonly Texture2D glowStickTexture;
        private readonly CollisionManager _collisionManager;
        private readonly Player _player;
        private static Vector2 healthPos;
        private static Vector2 glowPos;
        private static Vector2 healthOrigin;
        private static Vector2 glowStickOrigin;
        private static Matrix _uiTransform;
        private static string debugPrompt;
        private static string currentState;
        private static string state;
        private static string controls1;
        private static string controls2;
        private static string controls3;
        private static string controls4;
        private static string showControls;
        private static string hideControls;
        private static string deathMessage;
        private static Vector2 currentStatePos;
        private static Vector2 currentStateSize;
        private static Vector2 statePos;
        private static Vector2 controls1Pos;
        private static Vector2 controls2Pos;
        private static Vector2 controls3Pos;
        private static Vector2 controls4Pos;
        private static Vector2 debugPromptPos;
        private static Vector2 showControlsPos;
        private static Vector2 hideControlsPos;
        private static Vector2 deathMessagePosition;
        private static Vector2 deathMessageSize;
        private static int margin = 25;
        private static int padding = 30;

        public Color Color { get; set; }

        public UIManager(Player _player, CollisionManager cl) 
        {
            this._player = _player;
            _collisionManager = cl;

            InitPrompts();

            glowStickTexture = Globals.Content.Load<Texture2D>("greenGlowStick");
            glowStickOrigin = new(glowStickTexture.Width / 2, glowStickTexture.Height / 2);

            healthTextureFull = Globals.Content.Load<Texture2D>("heart-full");
            healthOrigin = new(healthTextureFull.Width / 2, healthTextureFull.Height / 2);
            UpdateUITransform();
        }

        private void InitPrompts()
        {
            showControls = "C : Show Controls";
            showControlsPos = new(margin, margin);

            debugPrompt = "T : Debug Mode";
            debugPromptPos = new(margin, margin);

            controls1 = "F : Main Attack";
            controls1Pos = new(margin, margin + padding);

            controls2 = "Spacebar : Second Attack";
            controls2Pos = new(margin, margin + padding*2);

            controls3 = "E : Use Glow Stick";
            controls3Pos = new(margin, margin + padding*3);

            controls4 = "G : Throw Glow Stick";
            controls4Pos = new(margin, margin + padding*4);

            currentState = "Closest Current Skeleton State: ";
            currentStatePos = new(margin, margin + padding*5);
            currentStateSize = Globals.Font.MeasureString(currentState);

            statePos = new(margin + currentStateSize.X * 0.70f, margin + padding*5);

            hideControls = "C : Hide Controls";
            hideControlsPos = new(margin, margin + padding*5);

            deathMessage = "GAME OVER!";
            deathMessageSize = Globals.Font.MeasureString(deathMessage);
            deathMessagePosition = new((Globals.WindowSize.X / 2 - deathMessageSize.X), (Globals.WindowSize.Y / 2 - deathMessageSize.Y * 2));
        }

        private string GetClosestEntityState()
        {
            State CurrentState = _collisionManager.GetClosestEntityState(_player.Position);

            switch (CurrentState)
            {
                case Wander:
                    return "Wander";

                case Aggro:
                    return "Aggro";

                case CloseAttack:
                    return "CloseAttack";

                case RangeAttack:
                    return "RangeAttack";

                case Dead:
                    return "Dead";

                default:
                    return "Invalid";
            }
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _uiTransform);

            if (InputManager.DebugMode)
                state = GetClosestEntityState();

            if (InputManager.ShowControls)
            {
                Globals.SpriteBatch.DrawString(Globals.Font, debugPrompt, debugPromptPos, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                Globals.SpriteBatch.DrawString(Globals.Font, controls1, controls1Pos, Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                Globals.SpriteBatch.DrawString(Globals.Font, controls2, controls2Pos, Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                Globals.SpriteBatch.DrawString(Globals.Font, controls3, controls3Pos, Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                Globals.SpriteBatch.DrawString(Globals.Font, controls4, controls4Pos, Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                if (InputManager.DebugMode)
                {
                    Globals.SpriteBatch.DrawString(Globals.Font, currentState, currentStatePos, Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                    Globals.SpriteBatch.DrawString(Globals.Font, state, statePos, Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                    Globals.SpriteBatch.DrawString(Globals.Font, hideControls, hideControlsPos + new Vector2(0, padding), Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                }
                else
                {
                    Globals.SpriteBatch.DrawString(Globals.Font, hideControls, hideControlsPos, Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                }
            }
            else
            {
                Globals.SpriteBatch.DrawString(Globals.Font, showControls, showControlsPos, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                if (InputManager.DebugMode)
                {
                    Globals.SpriteBatch.DrawString(Globals.Font, currentState, currentStatePos - new Vector2(0, padding*4), Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                    Globals.SpriteBatch.DrawString(Globals.Font, state, statePos - new Vector2(0, padding*4), Color.White, 0F, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                }
            }

            healthPos = new Vector2(Globals.WindowSize.X * 0.8f, Globals.WindowSize.Y * 0.95f);
            glowPos = new Vector2(Globals.WindowSize.X * 0.97f, Globals.WindowSize.Y * 0.85f);

            for (int i = 0; i < _player.Health / 10; i++)
            {
                Globals.SpriteBatch.Draw(healthTextureFull, healthPos, null, Color.White, 0f, healthOrigin, 1f, SpriteEffects.None, 0f);
                healthPos.X += healthTextureFull.Width;
            }
            
            if (_player.isDead())
            {
                Globals.SpriteBatch.DrawString(Globals.Font, deathMessage, deathMessagePosition, Color.Red, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
            }
            else
            {
                for (int j = 0; j < _player.GetGlowStickAmmo(); j++)
                {
                    Globals.SpriteBatch.Draw(glowStickTexture, glowPos, null, Color.White, 0f, glowStickOrigin, 1f, SpriteEffects.None, 0f);
                    glowPos.Y -= glowStickTexture.Height/2;
                }
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