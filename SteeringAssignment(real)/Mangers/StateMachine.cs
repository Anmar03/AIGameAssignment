using Microsoft.Xna.Framework;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.Mangers
{
    public enum State
    {
        Wander,
        Aggro,
        Attacking,
        Flee,
        Dead
    }
    public class StateMachine
    {
        private Vector2 Position;
        private Vector2 Direction;
        private Player player;
        private Sprite entity;
        private float distance;
        
        public StateMachine(Sprite entity)
        {
            Position = entity.Position;
            Direction = new();
            this.entity = entity;
        }

        public void Update(State CurrentState, Player _player)
        {
            this.player = _player;

            Direction = _player.Position - Position; Direction.Normalize();
            distance = Vector2.Distance(Position, _player.Position);

            switch (CurrentState)
            {
                case State.Wander:
                    if (distance < entity.AggroRadius && !_player.isDead())
                    {
                        CurrentState = State.Aggro;
                        break;
                    }

                        break;

                case State.Aggro:

                    break;

                case State.Attacking:

                    break;

                //case State.Flee:

                //    break;

                case State.Dead:

                    break;
            }
        }

        private void Wander()
        {

        }
    }
}
