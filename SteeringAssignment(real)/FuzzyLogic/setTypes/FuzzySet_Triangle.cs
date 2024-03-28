using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SteeringAssignment_real.FuzzyLogic.setTypes
{
    public class FuzzySet_Triangle : FuzzySet
    {
        //the values that define the shape of this FLV
        private double m_dPeakPoint;
        private double m_dLeftOffset;
        private double m_dRightOffset;

        public FuzzySet_Triangle(double mid, double lft, double rgt) : base(mid)
        {
            m_dPeakPoint = mid;
            m_dLeftOffset = lft;
            m_dRightOffset = rgt;
        }

        public override double CalculateDOM(double val)
        {
            const double epsilon = 0.00001; // Tolerance for floating-point comparisons

            // Test for the case where the triangle's left or right offsets are zero
            // (to prevent divide by zero errors below)
            if ((Math.Abs(m_dRightOffset - 0.0) < epsilon && Math.Abs(m_dPeakPoint - val) < epsilon) ||
                (Math.Abs(m_dLeftOffset - 0.0) < epsilon && Math.Abs(m_dPeakPoint - val) < epsilon))
            {
                return 1.0;
            }

            // Find DOM if left of center
            if ((val <= m_dPeakPoint) && (val >= (m_dPeakPoint - m_dLeftOffset)))
            {
                double grad = 1.0 / m_dLeftOffset;
                return grad * (val - (m_dPeakPoint - m_dLeftOffset));
            }
            // Find DOM if right of center
            else if ((val > m_dPeakPoint) && (val < (m_dPeakPoint + m_dRightOffset)))
            {
                double grad = 1.0 / -m_dRightOffset;
                return grad * (val - m_dPeakPoint) + 1.0;
            }
            // Out of range of this FLV, return zero
            else
            {
                return 0.0;
            }
        }

    }
}
