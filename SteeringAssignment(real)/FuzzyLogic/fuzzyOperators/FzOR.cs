using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SteeringAssignment_real.FuzzyLogic.fuzzyOperators
{
    public class FzOR : FuzzyTerm
    {
        private List<FuzzyTerm> m_Terms = new(4);

        //copy ctor
        public FzOR(FzOR fa)
        {
            foreach(FuzzyTerm term in fa.m_Terms)
            {
                m_Terms.Add(term.Clone());
            }
        }

        public FzOR(FuzzyTerm op1, FuzzyTerm op2)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
        }

        /**
         * ctor using three terms
         */
        public FzOR(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
        }

        /**
         * ctor using four terms
         */
        public FzOR(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3, FuzzyTerm op4)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
            m_Terms.Add(op4.Clone());
        }

        //virtual ctor
        public override FuzzyTerm Clone()
        {
            return new FzOR(this);
        }

        public override double GetDOM()
        {
            double largest = Double.MinValue;

            foreach (FuzzyTerm term in m_Terms)
            {
                if (term.GetDOM() > largest)
                {
                    largest = term.GetDOM();
                }
            }

            return largest;
        }

        // This method is unused and should not be called
        public override void ClearDOM() => throw new NotImplementedException();

        // This method is unused and should not be called
        public override void ORwithDOM(double val) => throw new NotImplementedException();
    }
}
