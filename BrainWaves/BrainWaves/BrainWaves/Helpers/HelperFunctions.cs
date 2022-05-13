using System;
using System.Collections.Generic;
using BrainWaves.Models;

namespace BrainWaves.Helpers
{
    public static class HelperFunctions
    {
        public static double[] GenerateTimeVector(int samplingFrequency, int length)
        {
            double T = (double)1 / samplingFrequency;            // % Sampling period
            double[] t = new double[length];
            for (int i = 0; i < length - 1; i++)
            {
                t[i] = i * T;
            }

            return t;
        }
        public static double[] GenerateSinWave(int samplingFrequency, int length, float amplitude, int signalFrequency)
        {
            double[] t = GenerateTimeVector(samplingFrequency, length);
            double[] sinWave = new double[length];
            for (int i = 0; i < length; i++)
            {
                sinWave[i] = amplitude * Math.Sin(2 * Math.PI * signalFrequency * t[i]);
            }
            return sinWave;
        }

        public static double[] GenerateCosWave(int samplingFrequency, int length, float amplitude, int signalFrequency)
        {
            double[] t = GenerateTimeVector(samplingFrequency, length);
            double[] cosWave = new double[length];
            for (int i = 0; i < length; i++)
            {
                cosWave[i] = amplitude * Math.Cos(2 * Math.PI * signalFrequency * t[i]);
            }
            return cosWave;
        }

        public static double[] GenerateRandomValues(int numOfValues)
        {
            Random random = new Random();
            double[] samples = new double[numOfValues];
            double range = 3.3;
            for (int i = 0; i < numOfValues; i++)
            {
                samples[i] = random.NextDouble() * range;
            }
            return samples;
        }

        public static List<Sample> GenerateFreqSamples(double[] input, int samplingFreq)
        {
            PerformZeroPaddingIfNeeded(ref input);
            double[] psd = FftSharp.Transform.FFTpower(input);
            double[] freq = FftSharp.Transform.FFTfreq(samplingFreq, psd.Length);
            List<Sample> samples = new List<Sample>();
            for(int i = 0; i < freq.Length; i++)
            {
                samples.Add(new Sample(psd[i], freq[i]));
            }

            return samples;
        }

        public static void PerformZeroPaddingIfNeeded(ref double[] input)
        {
            if (!FftSharp.Pad.IsPowerOfTwo(input.Length))
            {
                input = FftSharp.Pad.ZeroPad(input);
            }
        }
    }
}
