using BrainWaves.Models;
using System;
using System.Collections.Generic;

namespace BrainWaves.Helpers
{
    public static class TestSamplesGenerator
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

        public static List<BrainWaveSample> GenerateBrainWaveSamples(int numOfValues)
        {
            Random random = new Random();
            List<BrainWaveSample> samples = new List<BrainWaveSample>();
            for (int i = 0; i < numOfValues; i++)
            {
                samples.Add(new BrainWaveSample(
                    random.NextDouble() + random.Next(8, 13),
                    random.NextDouble() + random.Next(3, 30),
                    random.NextDouble() + random.Next(1, 3),
                    random.NextDouble() + random.Next(4, 8)));
            }

            return samples;
        }

        public static List<List<double>> GenerateWavesSamples(int numOfValues)
        {
            List<List<double>> returnSamples = new List<List<double>>();

            for (int i = 0; i < Constants.DefaultNumOfMeasurementsForWaves; i++)
            {
                double[] samples;
                switch (i)
                {
                    case 0:
                        samples = GenerateSinWave(200, numOfValues, 1, 2); // delta waves
                        break;
                    case 1:
                        samples = GenerateSinWave(200, numOfValues, 1, 5); // theta waves
                        break;
                    case 2:
                        samples = GenerateSinWave(200, numOfValues, 1, 10); // alfa waves
                        break;
                    case 3:
                        samples = GenerateSinWave(200, numOfValues, 1, 20); // beta waves
                        break;
                    default:
                        samples = GenerateSinWave(200, numOfValues, 1, 100);
                        break;
                }

                returnSamples.Add(new List<double>(samples));
            }

            return returnSamples;
        }

        public static List<List<double>> GenerateRandomWavesSamples(int numOfValues)
        {
            List<List<double>> returnSamples = new List<List<double>>();
            Random random = new Random();
            for (int i = 0; i < Constants.DefaultNumOfMeasurementsForWaves; i++)
            {
                List<double> samples = new List<double>();
                int divider = numOfValues / 4;
                for (int j = 0; j < numOfValues; j++)
                {
                    if (j < divider)
                    {
                        samples.Add(8 + random.NextDouble());
                    }
                    else if (j < 2 * divider)
                    {
                        samples.Add(3 + random.NextDouble());
                    }
                    else if (j < 3 * divider)
                    {
                        samples.Add(6 + random.NextDouble());
                    }
                    else
                    {
                        samples.Add(2 + random.NextDouble());
                    }
                }

                returnSamples.Add(samples);
            }

            return returnSamples;
        }
    }
}
