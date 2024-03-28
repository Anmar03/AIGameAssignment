using System;

namespace SteeringAssignment_real.FuzzyLogic.setTypes
{
    public class FuzzySet_LeftShoulder : FuzzySet
    {
        //the values that define the shape of this FLV
        private double m_dPeakPoint;
        private double m_dRightOffset;
        private double m_dLeftOffset;

        public FuzzySet_LeftShoulder(double peak, double LeftOffset, double RightOffset) : base(((peak - LeftOffset) + peak) / 2)
        {
            m_dPeakPoint = peak;
            m_dLeftOffset = LeftOffset;
            m_dRightOffset = RightOffset;
        }

        public override double CalculateDOM(double val)
        {
            const double epsilon = 0.00001; // Tolerance value

            // Test for the case where the left or right offsets are zero
            // (to prevent divide by zero errors below)
            if ((Math.Abs(m_dRightOffset) < epsilon && Math.Abs(m_dPeakPoint - val) < epsilon)
                || (Math.Abs(m_dLeftOffset) < epsilon && Math.Abs(m_dPeakPoint - val) < epsilon))
            {
                return 1.0;
            }
            // Find DOM if right of center
            else if (val >= m_dPeakPoint && val < (m_dPeakPoint + m_dRightOffset))
            {
                double grad = 1.0 / -m_dRightOffset;
                return grad * (val - m_dPeakPoint) + 1.0;
            }
            // Find DOM if left of center
            else if (val < m_dPeakPoint && val >= m_dPeakPoint - m_dLeftOffset)
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
