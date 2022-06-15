using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainWaves.Models
{
    public class BrainWaveSample
    {
        public BrainWaveSample() { }

        public BrainWaveSample(double alfa, double beta, double delta, double theta)
        {
            AlfaWave = alfa;
            BetaWave = beta;
            DeltaWave = delta;
            ThetaWave = theta;
        }

        public BrainWaveSample(List<Sample> freqSamples)
        {
            double avgAlfa = (from sample in freqSamples
                              where sample.SampleXValue >= 8 && sample.SampleXValue <= 13
                              select sample.SampleYValue).Average();

            double avgBeta = (from sample in freqSamples
                              where sample.SampleXValue >= 3 && sample.SampleXValue <= 30
                              select sample.SampleYValue).Average();

            double avgTheta = (from sample in freqSamples
                               where sample.SampleXValue >= 4 && sample.SampleXValue <= 8
                               select sample.SampleYValue).Average();

            double avgDelta = (from sample in freqSamples
                               where sample.SampleXValue >= 0.5 && sample.SampleXValue <= 3
                               select sample.SampleYValue).Average();

            AlfaWave = Math.Round(avgAlfa, 2);
            BetaWave = Math.Round(avgBeta, 2);
            ThetaWave = Math.Round(avgTheta, 2);
            DeltaWave = Math.Round(avgDelta, 2);
        }

        public double AlfaWave { get; set; }
        public double BetaWave { get; set; }
        public double DeltaWave { get; set; }
        public double ThetaWave { get; set; }
    }
}
