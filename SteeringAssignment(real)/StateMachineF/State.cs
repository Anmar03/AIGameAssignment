using SteeringAssignment_real.FuzzyLogic;
using SteeringAssignment_real.Models;

namespace SteeringAssignment_real.StateMachineF
{
    public abstract class State
    {
        protected FuzzyModule fm = new();
        protected double lastDesirability;

        abstract public void FuzzyLogicInit();
        abstract public double GetDesirability(float DistToTarget, float Health, float Ammo);
        abstract public void Enter(Skeleton skeleton);

        abstract public void Execute(Skeleton skeleton);

        abstract public void Exit(Skeleton skeleton);
    }
}