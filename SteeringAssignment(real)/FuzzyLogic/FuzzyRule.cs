namespace SteeringAssignment_real.FuzzyLogic
{
    public class FuzzyRule
    {
        private FuzzyTerm m_pAntecedent;
        private FuzzyTerm m_pConsequence;

        private FuzzyRule(FuzzyRule fr) {}
       
        public FuzzyRule(FuzzyTerm ant, FuzzyTerm con)
        {
            m_pAntecedent = ant.Clone();
            m_pConsequence = con.Clone();
        }

        public void SetConfidenceOfConsequentToZero()
        {
            m_pConsequence.ClearDOM();
        }

        public void Calculate()
        {
            m_pConsequence.ORwithDOM(m_pAntecedent.GetDOM());
        }
    }
}
