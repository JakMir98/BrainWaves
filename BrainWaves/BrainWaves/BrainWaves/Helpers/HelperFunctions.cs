using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainWaves.Models;
using Microcharts;
using SkiaSharp;

namespace BrainWaves.Helpers
{
    public static class HelperFunctions
    {
        public static BarChart GenerateBarChart(List<ChartEntry> entries, Orientation chartsOrientation)
        {
            return new BarChart
                {
                    Entries = entries,
                    ValueLabelOrientation = chartsOrientation,
                    LabelOrientation = chartsOrientation,
                    BackgroundColor = SKColors.Transparent,
                    LabelColor = SKColors.Gray
                };
        }

        public static LineChart GenerateLineChart(List<ChartEntry> entries, Orientation chartsOrientation)
        {
            return new LineChart
                {
                    Entries = entries,
                    ValueLabelOrientation = chartsOrientation,
                    LabelOrientation = chartsOrientation,
                    BackgroundColor = SKColors.Transparent,
                    LabelColor = SKColors.Gray
                };
        }

        public static PointChart GeneratePointChart(List<ChartEntry> entries, Orientation chartsOrientation)
        {
            return new PointChart
                {
                    Entries = entries,
                    ValueLabelOrientation = chartsOrientation,
                    LabelOrientation = chartsOrientation,
                    BackgroundColor = SKColors.Transparent,
                    LabelColor = SKColors.Gray
                };
        }

        public static ChartEntry GenerateChartEntryForFreqSamples(Sample sample, SKColor color)
        {
            return new ChartEntry((float)Math.Round(sample.SampleYValue, Constants.NumOfDecimalPlaces))
                {
                    Label = $"{Math.Round(sample.SampleXValue, Constants.NumOfDecimalPlaces)} Hz",
                    ValueLabel = $"{Math.Round(sample.SampleYValue, Constants.NumOfDecimalPlaces)}_V",
                    Color = color,
                    TextColor = SKColors.Gray,
                    ValueLabelColor = SKColors.Gray,
                };
        }

        public static ChartEntry GenerateChartEntryForTimeSamples(double value, int index, SKColor color)
        {
            return new ChartEntry((float)Math.Round(value, Constants.NumOfDecimalPlaces))
                {
                    Label = $"{index}.",
                    ValueLabel = $"{Math.Round(value, Constants.NumOfDecimalPlaces)}V",
                    Color = color,
                    TextColor = SKColors.Gray,
                    ValueLabelColor = SKColors.Gray
                };
        }

        public static ChartEntry GenerateChartEntryForWaves(double value, int index)
        {
            return new ChartEntry((float)Math.Round(value, Constants.NumOfDecimalPlaces))
            {
                Label = Constants.WavesChartLabels[index],
                ValueLabel = $"{Math.Round(value, Constants.NumOfDecimalPlaces)}V",
                Color = Constants.Colors[index],
                TextColor = SKColors.Gray,
                ValueLabelColor = SKColors.Gray
            };
        }

        public static List<Sample> GenerateFreqSamples(double[] input, int samplingFreq)
        {
            PerformZeroPaddingIfNeeded(ref input);
            double[] fft = FftSharp.Transform.Absolute(FftSharp.Transform.FFT(input));
            double[] freq = FftSharp.Transform.FFTfreq(samplingFreq, fft.Length);
            List<Sample> samples = new List<Sample>();
            for(int i = 0; i < freq.Length/2; i++)
            {
                samples.Add(new Sample(fft[i], freq[i]*2));
            }

            return samples;
        }

        public static BrainWaveSample GenerateBrainWavesSampleFromFFTWavesSamples(List<Sample> freqSamples)
        {
            IEnumerable<double> alfaWaves = (from sample in freqSamples
                                             where sample.SampleXValue >= 8 && sample.SampleXValue <= 13
                                             select sample.SampleYValue).Cast<double>();
            double avgAlfa = AverageYValues(alfaWaves.ToList());

            IEnumerable<double> betaWaves = ((from sample in freqSamples
                                              where sample.SampleXValue >= 3 && sample.SampleXValue <= 30
                                              select sample.SampleYValue)).Cast<double>();
            double avgBeta = AverageYValues(betaWaves.ToList());

            IEnumerable<double> thetaWaves = ((from sample in freqSamples
                                               where sample.SampleXValue >= 4 && sample.SampleXValue <= 8
                                               select sample.SampleYValue)).Cast<double>();
            double avgTheta = AverageYValues(thetaWaves.ToList());

            IEnumerable<double> deltaWaves = ((from sample in freqSamples
                                               where sample.SampleXValue >= 0.5 && sample.SampleXValue <= 3
                                               select sample.SampleYValue)).Cast<double>();
            double avgDelta = AverageYValues(deltaWaves.ToList());

            return new BrainWaveSample()
            {
                AlfaWave = avgAlfa,
                BetaWave = avgBeta,
                ThetaWave = avgTheta,
                DeltaWave = avgDelta
            };
        }

        public static void PerformZeroPaddingIfNeeded(ref double[] input)
        {
            if (!FftSharp.Pad.IsPowerOfTwo(input.Length))
            {
                input = FftSharp.Pad.ZeroPad(input);
            }
        }

        public static double AverageYValues(List<Sample> samples)
        {
            double avg = 0;
            foreach (var item in samples)
            {
                avg = item.SampleYValue;
            }
            avg /= samples.Count;
            return avg;
        }

        public static double AverageYValues(List<double> samples)
        {
            double avg = 0;
            foreach (var item in samples)
            {
                avg = item;
            }
            avg /= samples.Count;
            return avg;
        }
    }
}
