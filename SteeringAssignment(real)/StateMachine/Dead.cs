using Microsoft.Xna.Framework;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.StateMachine
{
    public class Dead : State
    {
        private static Dead instance = new Dead();

        public static Dead Instance() { return instance; }


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
            skeleton.SetCollision(); // sets collision to false

            if (skeleton.GetDeathAnim().CurrentFrame == skeleton.GetDeathAnim().TotalFrames - 1)
            {
                skeleton.GetDeathAnim().UpdateDeath(Vector2.Zero);
            }
            else
            {
                skeleton.GetDeathAnim().Update(skeleton.GetDeathAnimKey());
            }
        }

        public override void Exit(Skeleton skeleton)
        {
           
        }
    }
}
