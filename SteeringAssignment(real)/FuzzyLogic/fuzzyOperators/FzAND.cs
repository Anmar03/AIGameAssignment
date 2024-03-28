using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.FuzzyLogic.fuzzyOperators
{
    public class FzAND : FuzzyTerm
    {
        private List<FuzzyTerm> m_Terms = new List<FuzzyTerm>(4);

        // copy ctor
     
        public FzAND(FzAND fa)
        {
            foreach(FuzzyTerm term in fa.m_Terms)
            {
                m_Terms.Add(term.Clone());
            }
        }

        // ctors accepting fuzzy terms.
        // ctor using two terms
        public FzAND(FuzzyTerm op1, FuzzyTerm op2)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
        }

        // ctor using three terms
        public FzAND(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
        }


        // ctor using four terms
        public FzAND(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3, FuzzyTerm op4)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
            m_Terms.Add(op4.Clone());
        }

        public override FuzzyTerm Clone()
        {
            return new FzAND(this);
        }

        public override void ClearDOM()
        {
            foreach (FuzzyTerm term in m_Terms)
            {
                term.ClearDOM();
            }
        }

        public override double GetDOM()
        {
            double smallest = double.MaxValue;
            foreach (FuzzyTerm term in m_Terms)
            {
                double dom = term.GetDOM();
                if (dom < smallest)
                {
                    smallest = dom;
                }
            }

            return smallest;
        }

        public override void ORwithDOM(double val)
        {
            foreach (FuzzyTerm term in m_Terms)
            {
                term.ORwithDOM(val);
            }
        }
    }
}
