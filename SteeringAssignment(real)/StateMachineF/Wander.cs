using Microsoft.Xna.Framework;
using SteeringAssignment_real.Mangers;
using SteeringAssignment_real.Models;
using System;

namespace SteeringAssignment_real.StateMachineF
{
    public class Wander : State
    {
        private static Wander instance = new Wander();
        private float distance;
        private float wanderDuration;
        private Object animationKey;

        public static Wander Instance() { return instance; }

        public override void FuzzyLogicInit()
        {
            throw new System.NotImplementedException();
        }
        public override double GetDesirability(float DistToTarget, float Health, float Ammo)
        {
            throw new System.NotImplementedException();
        }

        public override void Enter(Skeleton skeleton)
        {
            skeleton.wanderDirection = Vector2.Zero;
        }

        public override void Execute(Skeleton skeleton)
        {
            Player _player = skeleton.GetPlayer();
            distance = Vector2.Distance(skeleton.Position, _player.Position);

            if (distance < skeleton.AggroRadius && !_player.isDead())
            {
                skeleton.ChangeState(Aggro.Instance());
            }
            else
            {
                if(skeleton.wanderDirection == Vector2.Zero)
                {
                    skeleton.wanderDirection = new Vector2(skeleton.random.Next(-1, 2), skeleton.random.Next(-1, 2));
                    skeleton.wanderDirection.Normalize();
                }

                // Gradually turn towards wander target direction
                if (skeleton.skeletonDirection != skeleton.wanderDirection && skeleton.rSpeed != 0)
                {
                    skeleton.skeletonDirection += skeleton.turnSpeed;
                    skeleton.skeletonDirection.Normalize();
                }

                // Update position based on wander direction and speed
                skeleton.Position += skeleton.skeletonDirection * skeleton.rSpeed * Globals.Time;

                // If wander duration elapsed, reset wander
                if (wanderDuration <= 0)
                {
                    wanderDuration = (float)skeleton.random.NextDouble() * 3.0f;
                    skeleton.wanderDirection = Vector2.Zero;
                    skeleton.turnSpeed = new Vector2(skeleton.random.Next(-10, 10), skeleton.random.Next(-10, 10));

                    // To ensure animation doesn't play if skeleton stands still, this is necessary
                    if (skeleton.random.Next(0, 3) == 0)
                    {
                        skeleton.rSpeed = 0;
                        skeleton.wanderDirection = new Vector2(1, 1);
                        skeleton.skeletonDirection = Vector2.Zero;
                    }
                    else
                    {
                        skeleton.rSpeed = skeleton.speed;
                    }
                }
                else
                {
                    wanderDuration -= Globals.Time;
                }
                animationKey = AnimationManager.GetAnimationKey(skeleton.skeletonDirection);
                skeleton.GetWalkingAnim().Update(animationKey);
            }
        }

        public override void Exit(Skeleton skeleton)
        {

        }
    }
}
