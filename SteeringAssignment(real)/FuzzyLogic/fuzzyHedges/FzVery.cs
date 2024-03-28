using SteeringAssignment_real.FuzzyLogic.setTypes;

namespace SteeringAssignment_real.FuzzyLogic.fuzzyHedges
{
    public class FzVery : FuzzyTerm
    {
        private FuzzySet m_Set;

        // prevent copying and assignment by clients
        private FzVery(FzVery inst)
        {
            m_Set = inst.m_Set;
        }

        public FzVery(FzSet ft)
        { 
            m_Set = ft.m_Set.Clone();
        }

        public override void ClearDOM()
        {
            m_Set.ClearDOM();
        }

        public override FuzzyTerm Clone()
        {
            return new FzVery(this);
        }

        public override double GetDOM()
        {
            return m_Set.GetDOM() * m_Set.GetDOM();
        }

        public override void ORwithDOM(double val)
        {
            m_Set.ORwithDOM(val * val);
        }
    }
}
