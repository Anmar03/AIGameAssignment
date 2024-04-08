using SteeringAssignment_real.FuzzyLogic.fuzzyOperators;
using SteeringAssignment_real.FuzzyLogic;
using SteeringAssignment_real.Models;
using SteeringAssignment_real.FuzzyLogic.fuzzyHedges;
using SharpDX.Direct3D9;
using SteeringAssignment_real.Mangers;
using System.Numerics;

namespace SteeringAssignment_real.StateMachine
{
    public class RangeAttack : State
    {
        private static RangeAttack instance = new RangeAttack();
        private object animationKey;

        public static RangeAttack Instance() { return instance; }

        public RangeAttack()
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

            FuzzyVariable Ammunition = fm.CreateFLV("Ammunition");
            FzSet Ammunition_Low = Ammunition.AddLeftShoulderSet("Ammunition_Low", 0, 1, 2);
            FzSet Ammunition_Half = Ammunition.AddTriangularSet("Ammunition_Half", 1, 2, 3);
            FzSet Ammunition_High = Ammunition.AddRightShoulderSet("Ammunition_High", 2, 3, 4);

            fm.AddRule(new FzAND(Target_Far, Health_Low, Ammunition_Low), Desirable);
            fm.AddRule(new FzAND(Target_Medium, Health_Low, Ammunition_Low), Undesirable);
            fm.AddRule(new FzAND(Target_Close, Health_Low, Ammunition_Low), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_Half, Ammunition_Low), Desirable);
            fm.AddRule(new FzAND(Target_Medium, Health_Half, Ammunition_Low), Undesirable);
            fm.AddRule(new FzAND(Target_Close, Health_Half, Ammunition_Low), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_High, Ammunition_Low), Desirable);
            fm.AddRule(new FzAND(Target_Medium, Health_High, Ammunition_Low), Undesirable);
            fm.AddRule(new FzAND(Target_Close, Health_High, Ammunition_Low), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_Low, Ammunition_Half), new FzVery(VeryDesirable));
            fm.AddRule(new FzAND(Target_Medium, Health_Low, Ammunition_Half), Desirable);
            fm.AddRule(new FzAND(Target_Close, Health_Low, Ammunition_Half), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_Half, Ammunition_Half), VeryDesirable);
            fm.AddRule(new FzAND(Target_Medium, Health_Half, Ammunition_Half), Undesirable);
            fm.AddRule(new FzAND(Target_Close, Health_Half, Ammunition_Half), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_High, Ammunition_Half), VeryDesirable);
            fm.AddRule(new FzAND(Target_Medium, Health_High, Ammunition_Half), Undesirable);
            fm.AddRule(new FzAND(Target_Close, Health_High, Ammunition_Half), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_Low, Ammunition_High), new FzVery(VeryDesirable));
            fm.AddRule(new FzAND(Target_Medium, Health_Low, Ammunition_High), Desirable);
            fm.AddRule(new FzAND(Target_Close, Health_Low, Ammunition_High), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_Half, Ammunition_High), new FzVery(VeryDesirable));
            fm.AddRule(new FzAND(Target_Medium, Health_Half, Ammunition_High), Desirable);
            fm.AddRule(new FzAND(Target_Close, Health_Half, Ammunition_High), new FzVery(Undesirable));

            fm.AddRule(new FzAND(Target_Far, Health_High, Ammunition_High), VeryDesirable);
            fm.AddRule(new FzAND(Target_Medium, Health_High, Ammunition_High), Desirable);
            fm.AddRule(new FzAND(Target_Close, Health_High, Ammunition_High), new FzVery(Undesirable));
        }

        public override double GetDesirability(float DistToTarget, float Health, float Ammo)
        {
            if (Ammo == 0)
            {
                lastDesirability = 0;
            }
            else
            {
                fm.Fuzzify("DistToTarget", (double)DistToTarget);
                fm.Fuzzify("Health", (double)Health);
                fm.Fuzzify("Ammunition", (double)Ammo);

                lastDesirability = fm.DeFuzzify("Desirability", FuzzyModule.DefuzzifyType.max_av);
            }

            return lastDesirability;
        }

        public override void Enter(Skeleton skeleton)
        {
            skeleton.FireBallAttack();
            animationKey = AnimationManager.GetAnimationKey(skeleton.DirectionToPlayer());
        }

        public override void Execute(Skeleton skeleton)
        {
            if (skeleton.fireballAttack)
            {
                skeleton.GetWalkingAnim().Update(animationKey);
                animationKey = AnimationManager.GetAnimationKey(Vector2.Zero);
            }
            else
            {
                skeleton.ChangeState(Aggro.Instance());
            }
        }

        public override void Exit(Skeleton skeleton)
        {
            
        }
    }
}
