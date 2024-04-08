using System.Collections.Generic;
using System.IO;

namespace SteeringAssignment_real.FuzzyLogic
{
    public class FuzzyModule
    {
        private class varMap : Dictionary<string, FuzzyVariable>
        {
            public varMap()
            {

            }
        }

        
        public enum DefuzzifyType { max_av, centroid };

        public static int NumSamplesToUseForCentroid = 15;

        private varMap m_Variables = new();

        // Vector containing all the fuzzy rules
        private List<FuzzyRule> m_Rules = new();

        private void SetConfidencesOfConsequentsToZero()
        {
            foreach (var curRule in m_Rules)
            {
                curRule.SetConfidenceOfConsequentToZero();
            }
        }
        public FuzzyVariable CreateFLV(string VarName)
        {
            FuzzyVariable newVariable = new FuzzyVariable();
            m_Variables[VarName] = newVariable;
            return newVariable;
        }

        public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            m_Rules.Add(new FuzzyRule(antecedent, consequence));
        }

        public void Fuzzify(string NameOfFLV, double Val)
        {
            // Check if the key exists
            if (m_Variables.TryGetValue(NameOfFLV, out FuzzyVariable variable))
            {
                variable.Fuzzify(Val);
            }
            else
            {
                // Key not found
                throw new KeyNotFoundException("<FuzzyModule::Fuzzify>: key not found");
            }
        }

        public double DeFuzzify(string NameOfFLV, DefuzzifyType method)
        {
            // make sure key exists
            if (!m_Variables.ContainsKey(NameOfFLV))
            {
                throw new KeyNotFoundException("<FuzzyModule::DeFuzzifyMaxAv>: key not found");
            }

            // clear the DOMs of all the consequents of all the rules
            SetConfidencesOfConsequentsToZero();

            // process the rules
            foreach (var curRule in m_Rules)
            {
                curRule.Calculate();
            }
            
            // now defuzzify the resultant conclusion using the specified method
            switch (method)
            {
                case DefuzzifyType.centroid:
                    return m_Variables[NameOfFLV].DeFuzzifyCentroid(NumSamplesToUseForCentroid);
                case DefuzzifyType.max_av:
                    return m_Variables[NameOfFLV].DeFuzzifyMaxAv();
                default:
                    return 0.0;
            }
        }

        // Writes the DOMs of all the variables in the module to an output stream
        public StreamWriter WriteAllDOMs(StreamWriter os)
        {
            os.WriteLine("\n\n");

            foreach (var curVar in m_Variables)
            {
                os.WriteLine("\n--------------------------- " + curVar.Key + " " + curVar.Value.WriteDOMs(os));
                os.WriteLine();
            }

            return os;
        }

    }
}
