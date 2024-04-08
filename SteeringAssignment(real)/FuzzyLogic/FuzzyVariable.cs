using SharpDX.MediaFoundation;
using SteeringAssignment_real.FuzzyLogic.setTypes;
using System;
using System.Collections.Generic;
using System.IO;

namespace SteeringAssignment_real.FuzzyLogic
{
    public class FuzzyVariable
    {
        private class MemberSets : Dictionary<string, setTypes.FuzzySet> {};

        private FuzzyVariable(FuzzyVariable fv)
        {
            throw new NotSupportedException("Unsupported operation");
        }

        private MemberSets m_MemberSets = new MemberSets();
        private double m_dMinRange;
        private double m_dMaxRange;

        // method is called with the upper and lower bound of a set each time a
        // new set is added to adjust the upper and lower range values accordingly
        private void AdjustRangeToFit(double minBound, double maxBound)
        {
            if (minBound < m_dMinRange)
            {
                m_dMinRange = minBound;
            }
            if (maxBound > m_dMaxRange)
            {
                m_dMaxRange = maxBound;
            }
        }

        public FuzzyVariable()
        {
            m_dMinRange = 0.0;
            m_dMaxRange = 0.0;
        }
        
        // adds a left shoulder type set
        public FzSet AddLeftShoulderSet(string name, double minBound, double peak, double maxBound)
        {
            m_MemberSets[name] = new FuzzySet_LeftShoulder(peak, peak - minBound, maxBound - peak);

            AdjustRangeToFit(minBound, maxBound);

            return new FzSet(m_MemberSets[name]);
        }

        public FzSet AddRightShoulderSet(String name, double minBound, double peak, double maxBound)
        {
            m_MemberSets[name] = new FuzzySet_RightShoulder(peak, peak - minBound, maxBound - peak);

            AdjustRangeToFit(minBound, maxBound);

            return new FzSet(m_MemberSets[name]);
        }

        public FzSet AddTriangularSet(String name, double minBound, double peak, double maxBound)
        {
            m_MemberSets[name] = new FuzzySet_Triangle(peak, peak - minBound, maxBound - peak);

            AdjustRangeToFit(minBound, maxBound);

            return new FzSet(m_MemberSets[name]);
        }

        public FzSet AddSingletonSet(String name, double minBound, double peak, double maxBound)
        {
            m_MemberSets[name] = new FuzzySet_Singleton(peak, peak - minBound, maxBound - peak);

            AdjustRangeToFit(minBound, maxBound);

            return new FzSet(m_MemberSets[name]);
        }

        public void Fuzzify(double val)
        {
            if (!(val >= m_dMinRange && val <= m_dMaxRange))
            {
                throw new ArgumentOutOfRangeException("<FuzzyVariable::Fuzzify>: value out of range");
            }

            // For each set in the FLV, calculate the DOM for the given value
            foreach (var pair in m_MemberSets)
            {
                pair.Value.SetDOM(pair.Value.CalculateDOM(val));
            }
        }

        /**
         * defuzzifies the value by averaging the maxima of the sets that have fired
         *
         * OUTPUT = sum (maxima * DOM) / sum (DOMs) 
         */
        public double DeFuzzifyMaxAv()
        {
            double bottom = 0.0;
            double top = 0.0;

            foreach (var curSet in m_MemberSets)
            {
                bottom += curSet.Value.GetDOM();
                top += curSet.Value.GetRepresentativeVal() * curSet.Value.GetDOM();
            }

            //make sure bottom is not equal to zero
            if (Math.Abs(bottom) < double.Epsilon)
            {
                return 0.0;
            }

            return top / bottom;
        }

    
        // defuzzify the variable using the centroid method
        public double DeFuzzifyCentroid(int NumSamples)
        {
            // calculate the step size
            double StepSize = (m_dMaxRange - m_dMinRange) / (double)NumSamples;

            double TotalArea = 0.0;
            double SumOfMoments = 0.0;

            //step through the range of this variable in increments equal to StepSize
            //adding up the contribution (lower of CalculateDOM or the actual DOM of this
            //variable's fuzzified value) for each subset. This gives an approximation of
            //the total area of the fuzzy manifold.(This is similar to how the area under
            //a curve is calculated using calculus... the heights of lots of 'slices' are
            //summed to give the total area.)
            
            //in addition the moment of each slice is calculated and summed. Dividing
            //the total area by the sum of the moments gives the centroid. (Just like
            //calculating the center of mass of an object)
            for (int samp = 1; samp <= NumSamples; ++samp)
            {
                //for each set get the contribution to the area. This is the lower of the 
                //value returned from CalculateDOM or the actual DOM of the fuzzified 
                //value itself   
                foreach (var curSet in m_MemberSets)
                {
                    double contribution =
                        Math.Min(curSet.Value.CalculateDOM(m_dMinRange + samp * StepSize),
                                 curSet.Value.GetDOM());

                    TotalArea += contribution;

                    SumOfMoments += (m_dMinRange + samp * StepSize) * contribution;
                }
            }

            // Make sure total area is not equal to zero
            if (Math.Abs(TotalArea) < double.Epsilon)
            {
                return 0.0;
            }

            return (SumOfMoments / TotalArea);
        }

        public StreamWriter WriteDOMs(StreamWriter os)
        {
            foreach (var curSet in m_MemberSets)
            {
                os.WriteLine($"\n{curSet.Key} is {curSet.Value.GetDOM()}");
            }

            os.WriteLine($"\nMin Range: {m_dMinRange} \nMax Range: {m_dMaxRange}");

            return os;
        }

    }
}
