using System;
using System.Collections.Generic;
using BrainWaves.Models;
using FftSharp;

namespace BrainWaves.Helpers
{
    public static class HelperFunctions
    {
        public static double[] GenerateSinWave(int samplingFrequency, int length, float amplitude, int signalFrequency)
        {
            float T = (float)1 / samplingFrequency;            // % Sampling period
            float[] t = new float[length];
            for (int i = 0; i < length - 1; i++)
            {
                t[i] = i * T;
            }

            double[] sinWave = new double[length];
            for (int i = 0; i < length; i++)
            {
                sinWave[i] = amplitude * Math.Sin(2 * Math.PI * signalFrequency * t[i]);
            }
            return sinWave;
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

        public static FrequencySamplesContainer GenerateFreqSamples(double[] input, int samplingFreq)
        {
            return new FrequencySamplesContainer(Transform.Absolute(Transform.FFT(input)),
                            Transform.FFTfreq(samplingFreq, input.Length));
        }
    }
}
