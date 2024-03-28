using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SteeringAssignment_real.FuzzyLogic.setTypes
{
    abstract public class FuzzySet
    {
        protected double m_dDOM;

        protected double m_dRepresentativeValue;

        public FuzzySet(double RepVal)
        {
            m_dDOM = 0.0;
            m_dRepresentativeValue = RepVal;
        }

        public abstract double CalculateDOM(double val);

        public void ORwithDOM(double val)
        {
            if (val > m_dDOM)
            {
                m_dDOM = val;
            }
        }

        //accessor methods
        public double GetRepresentativeVal()
        {
            return m_dRepresentativeValue;
        }

        public void ClearDOM()
        {
            m_dDOM = 0.0;
        }

        public double GetDOM()
        {
            return m_dDOM;
        }

        public void SetDOM(double val)
        {
            Debug.Assert(val <= 1 && val >= 0, "<FuzzySet::SetDOM>: invalid value");
            m_dDOM = val;
        }
        public FuzzySet Clone()
        {
            return (FuzzySet)this.MemberwiseClone();
        }
    }
}
