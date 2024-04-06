using Microsoft.Xna.Framework;
using SteeringAssignment_real.FuzzyLogic.fuzzyHedges;
using SteeringAssignment_real.FuzzyLogic.fuzzyOperators;
using SteeringAssignment_real.FuzzyLogic;
using SteeringAssignment_real.Mangers;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.StateMachineF
{
    public class CloseAttack : State
    {
        private object animationKey;
        private Vector2 pushDirection;
        private float distance;
        private static CloseAttack instance = new CloseAttack();

        public static CloseAttack Instance() { return instance; }

        public CloseAttack() 
        {
            FuzzyLogicInit();
        }

        public override void FuzzyLogicInit()
        {
            FuzzyVariable Desirability = fm.CreateFLV("Desirability");
            FzSet VeryDesirable = Desirability.AddRightShoulderSet("VeryDesirable", 50, 75, 100);
            FzSet Desirable = Desirability.AddTriangularSet("Desirable", 25, 50, 75);
            FzSet Undesirable = Desirability.AddLeftShoulderSet("Undesirable", 0, 25, 50);

            FuzzyVariable DistToTarget = fm.CreateFLV("DistToTarget");
            FzSet Target_Close = DistToTarget.AddLeftShoulderSet("Target_Close", 0, 25, 150);
            FzSet Target_Medium = DistToTarget.AddTriangularSet("Target_Medium", 25, 150, 300);
            FzSet Target_Far = DistToTarget.AddRightShoulderSet("Target_Far", 150, 300, 700);

            FuzzyVariable Health = fm.CreateFLV("Health");
            FzSet Health_Low = Health.AddLeftShoulderSet("Health_Low", 0, 2.5, 15);
            FzSet Health_Half = Health.AddTriangularSet("Health_Half", 2.5, 5, 30);
            FzSet Health_High = Health.AddRightShoulderSet("Health_High", 15, 30, 50);

            fm.AddRule(new FzAND(Target_Far, Health_Low), new FzVery(Undesirable));
            fm.AddRule(new FzAND(Target_Medium, Health_Low), Desirable);
            fm.AddRule(new FzAND(Target_Close, Health_Low), VeryDesirable);

            fm.AddRule(new FzAND(Target_Far, Health_Half), Undesirable);
            fm.AddRule(new FzAND(Target_Medium, Health_Half), VeryDesirable);
            fm.AddRule(new FzAND(Target_Close, Health_Half), new FzVery(VeryDesirable));

            fm.AddRule(new FzAND(Target_Far, Health_High), Undesirable);
            fm.AddRule(new FzAND(Target_Medium, Health_High), Desirable);
            fm.AddRule(new FzAND(Target_Close, Health_High), new FzVery(VeryDesirable));
        }

        public override double GetDesirability(float DistToTarget, float Health, float Ammo)
        {
            fm.Fuzzify("DistToTarget", (double)DistToTarget);
            fm.Fuzzify("Health", (double)Health);

            lastDesirability = fm.DeFuzzify("Desirability", FuzzyModule.DefuzzifyType.max_av);

            return lastDesirability;
        }
        public override void Enter(Skeleton skeleton)
        {
            skeleton.GetCloseAttackAnim().Reset();
        }

        public override void Execute(Skeleton skeleton)
        {
            Player _player = skeleton.GetPlayer();
            distance = Vector2.Distance(skeleton.Position, _player.Position);

            pushDirection = Vector2.Zero;
            animationKey = AnimationManager.GetAnimationKey(skeleton.skeletonDirection);
            skeleton.GetCloseAttackAnim().Update(animationKey);

            // if in the middle of animation and still close, push the player line 168
            if (skeleton.GetCloseAttackAnim().CurrentFrame == 3 && distance < skeleton.width)
            {
                pushDirection = Vector2.Normalize(_player.Position - skeleton.Position) * skeleton.GetPushForce();
                skeleton.SetPushDirection(pushDirection);
            }
            else if (skeleton.GetCloseAttackAnim().CurrentFrame == skeleton.GetCloseAttackAnim().TotalFrames - 1)
            {
                // Attack animation finished. start cooldown
                skeleton.ChangeState(Aggro.Instance());
                skeleton.SetAttackTimer();
            }
        }

        public override void Exit(Skeleton skeleton)
        {
            
        }
    }
}
