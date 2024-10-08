﻿using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.FuzzyLogic.setTypes
{
    public class FuzzySet_Singleton : FuzzySet
    {
        private double m_dMidPoint;
        private double m_dLeftOffset;
        private double m_dRightOffset;

        public FuzzySet_Singleton(double mid, double lft, double rgt) : base(mid)
        {
            m_dMidPoint = mid;
            m_dLeftOffset = lft;
            m_dRightOffset = rgt;
        }

        public override double CalculateDOM(double val)
        {
            if ((val >= m_dMidPoint - m_dLeftOffset) && (val <= m_dMidPoint + m_dRightOffset))
            {
                return 1.0;
            } //out of range of this FLV, return zero
            else
            {
                return 0.0;
            }
        }
    }
}
