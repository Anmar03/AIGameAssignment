using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SteeringAssignment_real.Mangers
{
    public static class InputManager
    {
        private static Vector2 _direction = Vector2.Zero, _lastDirection;
        public static Vector2 Direction => _direction;
        public static Vector2 LastDirection => _lastDirection;
        public static bool Moving => _direction != Vector2.Zero;
        public static bool SpacebarPressed;
        public static bool F_keyPressed;
        public static bool DebugMode = false;
        public static bool G_keyPressed;
        public static bool E_keyPressed;
        public static bool ShowControls = false;

        public static void Update()
        {
            var keyboardState = Keyboard.GetState();

            if (_direction != Vector2.Zero)
            {
                _lastDirection = _direction;
            }
            
            if (_lastDirection == Vector2.Zero)
            {
                _lastDirection = new(0, 1); // S key as default
            }

            _direction = Vector2.Zero;
            SpacebarPressed = false;
            F_keyPressed = false;
            G_keyPressed = false;
            E_keyPressed = false;

            if (keyboardState.IsKeyDown(Keys.W)) { _direction.Y--; }   // walK
            if (keyboardState.IsKeyDown(Keys.S)) { _direction.Y++; }  //   waLk
            if (keyboardState.IsKeyDown(Keys.A)) { _direction.X--; } //     wAlk
            if (keyboardState.IsKeyDown(Keys.D)) { _direction.X++; }//       Walk
            
            if (keyboardState.IsKeyDown(Keys.Space)) { SpacebarPressed = true; } // Punch Attack
            
            if (keyboardState.IsKeyDown(Keys.F)) { F_keyPressed = true; } // Sword Attack

            if (keyboardState.IsKeyDown(Keys.T)) { DebugMode = !DebugMode; } // Toggle DebugMode

            if (keyboardState.IsKeyDown(Keys.G)) { G_keyPressed = true; } // Throw Glowstick

            if (keyboardState.IsKeyDown(Keys.E)) { E_keyPressed = true; } // activate GlowStick

            if (keyboardState.IsKeyDown(Keys.C)) { ShowControls = !ShowControls; } // Shows Controls

            if (_direction != Vector2.Zero)
            {
                _direction.Normalize();
            }

            if (_lastDirection != Vector2.Zero)
            {
                _lastDirection.Normalize();
            }
        }
    }
}
