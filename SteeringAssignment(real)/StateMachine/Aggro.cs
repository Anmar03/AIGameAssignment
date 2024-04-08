using Microsoft.Xna.Framework;
using SteeringAssignment_real.Mangers;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.StateMachine
{
    public class Aggro : State
    {
        private float distance;
        private object animationKey;
        private float fireballTimer = 0f;
        private const float cooldown = 6f;
        

        private static Aggro instance = new Aggro();
        public static Aggro Instance() { return instance; }

        public override void FuzzyLogicInit()
        {
            throw new System.NotImplementedException();
        }
        public override double GetDesirability(float DistToTarget, float Health, float Ammo)
        {
            throw new System.NotImplementedException();
        }
        public override void Enter(Skeleton skeleton) { }

        public override void Execute(Skeleton skeleton)
        {
            Player _player = skeleton.GetPlayer();
            distance = Vector2.Distance(skeleton.Position, _player.Position);

            if (fireballTimer > 0f)
            {
                fireballTimer -= Globals.Time;
            }

            // If out of aggro radius or Player is dead, go back to wandering
            if (distance > skeleton.AggroRadius || _player.isDead())
            {
                skeleton.ChangeState(Wander.Instance());
            }
            else
            {
                // Make a cooldown for this
                skeleton.SelectAttack();

                if (skeleton.closeAttack || fireballTimer > 0)
                {
                    // If close to player, start attacking
                    if (distance < skeleton.width / 2 && skeleton.GetAttackTimer() <= 0)
                    {
                        skeleton.ChangeState(CloseAttack.Instance());
                        skeleton.GetCloseAttackAnim().Reset(); // Reset attack animation to start from beginning
                    }
                    else
                    {
                        animationKey = AnimationManager.GetAnimationKey(skeleton.skeletonDirection);
                        skeleton.GetWalkingAnim().Update(animationKey);
                        skeleton.Position += skeleton.skeletonDirection * skeleton.speed * Globals.Time;
                    }
                }
                else
                {
                    skeleton.ChangeState(RangeAttack.Instance());
                    fireballTimer = cooldown;
                }
            }
        }

        public override void Exit(Skeleton skeleton)
        {
            
        }
    }
}
