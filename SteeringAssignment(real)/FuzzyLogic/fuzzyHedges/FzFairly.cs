using SteeringAssignment_real.FuzzyLogic.setTypes;
using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.FuzzyLogic.fuzzyHedges
{
    public class FzFairly : FuzzyTerm
    {
        private FuzzySet m_Set;

        //prevent copying and assignment
        private FzFairly(FzFairly inst)
        {
            m_Set = inst.m_Set;
        }

        //private FzFairly& operator=(const FzFairly&);
        public FzFairly(FzSet ft)
        {
            m_Set = ft.m_Set.Clone();
        }

        public override FuzzyTerm Clone()
        {
            return new FzFairly(this);
        }

        public override double GetDOM()
        {
             return Math.Sqrt(m_Set.GetDOM());
        }

        public override void ClearDOM()
        {
            m_Set.ClearDOM();
        }

        public override void ORwithDOM(double val)
        {
            m_Set.ORwithDOM(Math.Sqrt(val));
        }
    }
}
