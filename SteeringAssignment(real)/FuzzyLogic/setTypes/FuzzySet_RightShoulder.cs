using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.FuzzyLogic.setTypes
{
    public class FuzzySet_RightShoulder : FuzzySet
    {
        //the values that define the shape of this FLV
        private double m_dPeakPoint;
        private double m_dLeftOffset;
        private double m_dRightOffset;
        public FuzzySet_RightShoulder(double peak, double LeftOffset, double RightOffset) : base(((peak + RightOffset) + peak) / 2)
        {
            m_dPeakPoint = peak;
            m_dLeftOffset = LeftOffset;
            m_dRightOffset = RightOffset;
        }

        public override double CalculateDOM(double val)
        {
            // Check for case where the offset may be zero
            if ((m_dLeftOffset == 0 && val == m_dPeakPoint))
            {
                return 1.0;
            }
            // Find DOM if left of center
            if (val <= m_dPeakPoint && val > (m_dPeakPoint - m_dLeftOffset))
            {
                double grad = 1.0 / m_dLeftOffset;
                return grad * (val - (m_dPeakPoint - m_dLeftOffset));
            }
            // Find DOM if right of center
            else if (val > m_dPeakPoint)
            {
                return 1.0;
            }
            // Out of range of this FLV, return zero
            else
            {
                return 0.0;
            }
        }

    }
}
