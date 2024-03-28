using SteeringAssignment_real.FuzzyLogic.setTypes;
using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.FuzzyLogic
{
    public class FzSet : FuzzyTerm
    {
        public setTypes.FuzzySet m_Set;
        public FzSet(FuzzySet fs)
        {
            m_Set = fs;
        }
        // copy ctor
        private FzSet(FzSet con)
        {
            m_Set = con.m_Set;
        }

        public override void ClearDOM()
        {
            m_Set.ClearDOM();
        }

        public override FuzzyTerm Clone()
        {
            return (FuzzyTerm)new FzSet(this);
        }

        public override double GetDOM()
        {
            return m_Set.GetDOM();
        }

        public override void ORwithDOM(double val)
        {
            m_Set.ORwithDOM(val);
        }
    }
}
