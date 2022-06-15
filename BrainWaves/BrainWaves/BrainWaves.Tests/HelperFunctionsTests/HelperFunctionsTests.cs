using BrainWaves.Helpers;
using BrainWaves.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainWaves.Tests.HelperFunctionTests
{
    public  class HelperFunctionsTests
    {
        private const int DefaultSamplingFrequency = 1000;
        private const int DefaultSignalFrequency = 60;
        private const int DefaultSignalLength = 4096;
        private const int DefaultSignalAmplitude = 1;
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCase(10, 16)]
        [TestCase(16, 16)]
        [TestCase(254, 256)]
        [TestCase(1000, 1024)]
        public void PerformingZeroPaddingOnSinwave_IncreaseInputLength(int inputSignalLength, int expectedOutputSignaLlength)
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, inputSignalLength, DefaultSignalAmplitude, DefaultSignalFrequency);
            
            HelperFunctions.PerformZeroPaddingIfNeeded(ref testSignal);
            
            testSignal.Length.Should().Be(expectedOutputSignaLlength);
        }

        [Test]
        [TestCase(5, 8)]
        [TestCase(31, 32)]
        [TestCase(612, 1024)]
        [TestCase(1025, 2048)]
        public void PerformingZeroPaddingOnRandomSignal_IncreaseInputLength(int inputSignalLength, int expectedOutputSignaLlength)
        {
            double[] testSignal = new double[inputSignalLength];
            Random random = new Random();
            for(int i = 0; i < inputSignalLength; i++)
            {
                testSignal[i] = random.NextDouble();
            }

            HelperFunctions.PerformZeroPaddingIfNeeded(ref testSignal);

            testSignal.Length.Should().Be(expectedOutputSignaLlength);
        }

        [Test]
        public void AverageYValues_ActuallyAverages_YSamples()
        {
            List<Sample> samples = new List<Sample>();
            for(int i = 0; i < 10; i++)
            {
                samples.Add(new Sample(10.0, i));
            }
            var expectedAverage = 10.0;

            var actualAverage = HelperFunctions.AverageYValues(samples);

            actualAverage.Should().Be(expectedAverage);
        }

        [Test]
        public void AverageYValues_ActuallyAverages_values()
        {
            List<double> samples = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                samples.Add(1.0);
            }
            var expectedAverage = 1.0;

            var actualAverage = HelperFunctions.AverageYValues(samples);

            actualAverage.Should().Be(expectedAverage);
        }

        [Test]
        [TestCase(12, 8)]
        [TestCase(6666, 4096)]
        public void GenerateFreqSamples_HasCorrectLength(int inputSignalLength, int expectedSignalLength)
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, inputSignalLength, DefaultSignalAmplitude, DefaultSignalFrequency);

            List<Sample> samples = HelperFunctions.GenerateFreqSamples(testSignal, DefaultSamplingFrequency);

            samples.Count.Should().Be(expectedSignalLength);
        }

        [Test]
        [TestCase(12, 16)]
        [TestCase(6666, 8192)]
        public void FFT_HasCorrectLength(int inputSignalLength, int expectedSignalLength)
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, inputSignalLength, DefaultSignalAmplitude, DefaultSignalFrequency);
            HelperFunctions.PerformZeroPaddingIfNeeded(ref testSignal);
            var Y = FftSharp.Transform.FFT(testSignal);

            Y.Length.Should().Be(expectedSignalLength);
        }

        [Test]
        [TestCase(9, 16)]
        [TestCase(2022, 2048)]
        public void FFTFREQ_HasCorrectLength(int inputSignalLength, int expectedSignalLength)
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, inputSignalLength, DefaultSignalAmplitude, DefaultSignalFrequency);
            HelperFunctions.PerformZeroPaddingIfNeeded(ref testSignal);
            var Y = FftSharp.Transform.FFT(testSignal);
            var X = FftSharp.Transform.FFTfreq(100, Y.Length);
            X.Length.Should().Be(expectedSignalLength);
        }

        [Test]
        public void GenerateBrainWaveSampleFromFFTWavesSamples_AlfaCorrectValue()
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, 9);
            List<Sample> freqSamples = HelperFunctions.GenerateFreqSamples(testSignal, DefaultSamplingFrequency);

            List<double> alfaValues = new List<double>();
            foreach (var sample in freqSamples)
            {
                if (sample.SampleXValue >= 8 && sample.SampleXValue <= 13)
                    alfaValues.Add(sample.SampleYValue);
            }
            var expectedAverage = alfaValues.Average();

            BrainWaveSample brainWaveSample = new BrainWaveSample(freqSamples);

            brainWaveSample.AlfaWave.Should().BeApproximately(expectedAverage, 0.01);
        }

        [Test]
        public void GenerateBrainWaveSampleFromFFTWavesSamples_BetaCorrectValue()
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, 15);
            List<Sample> freqSamples = HelperFunctions.GenerateFreqSamples(testSignal, DefaultSamplingFrequency);

            List<double> betaValues = new List<double>();
            foreach (var sample in freqSamples)
            {
                if (sample.SampleXValue >= 3 && sample.SampleXValue <= 30)
                    betaValues.Add(sample.SampleYValue);
            }
            var expectedAverage = betaValues.Average();

            BrainWaveSample brainWaveSample = new BrainWaveSample(freqSamples);

            brainWaveSample.BetaWave.Should().BeApproximately(expectedAverage, 0.01);
        }

        [Test]
        public void GenerateBrainWaveSampleFromFFTWavesSamples_ThetaCorrectValue()
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, 5);
            List<Sample> freqSamples = HelperFunctions.GenerateFreqSamples(testSignal, DefaultSamplingFrequency);

            List<double> thetaValues = new List<double>();
            foreach (var sample in freqSamples)
            {
                if (sample.SampleXValue >= 4 && sample.SampleXValue <= 8)
                    thetaValues.Add(sample.SampleYValue);
            }
            var expectedAverage = thetaValues.Average();

            BrainWaveSample brainWaveSample = new BrainWaveSample(freqSamples);

            brainWaveSample.ThetaWave.Should().BeApproximately(expectedAverage, 0.01);
        }

        [Test]
        public void GenerateBrainWaveSampleFromFFTWavesSamples_DeltaCorrectValue()
        {
            double[] testSignal = TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, 2);
            List<Sample> freqSamples = HelperFunctions.GenerateFreqSamples(testSignal, DefaultSamplingFrequency);

            List<double> deltaValues = new List<double>();
            foreach(var sample in freqSamples)
            {
                if(sample.SampleXValue >= 0.5 && sample.SampleXValue <= 3)
                    deltaValues.Add(sample.SampleYValue);
            }
            var expectedAverage = deltaValues.Average();

            BrainWaveSample brainWaveSample = new BrainWaveSample(freqSamples);

            brainWaveSample.DeltaWave.Should().BeApproximately(expectedAverage, 0.01);

        }

        [Test]
        [TestCase(50, 25)]
        [TestCase(100, 76)]
        public void ApplyLowPassFilter_GetsRidOfValuesGreaterThenCutOffFreq(int signalFreq, int lowPassCutOffFreq)
        {
            List<double> testSignal = new List<double>(TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, signalFreq));

            HelperFunctions.ApplyLowPassFilter(ref testSignal, DefaultSamplingFrequency, lowPassCutOffFreq);

            testSignal[4].Should().BeInRange(-0.25, 0.25);
        }

        [Test]
        [TestCase(50, 60)]
        [TestCase(100, 150)]
        public void ApplyLowPassFilter_LeavesValuesLowerThenCutOffFreq(int signalFreq, int lowPassCutOffFreq)
        {
            List<double> testSignal = new List<double>(TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, signalFreq));

            HelperFunctions.ApplyLowPassFilter(ref testSignal, DefaultSamplingFrequency, lowPassCutOffFreq);

            testSignal[32].Should().Match(v => v > 0.4 || v < -0.4);
        }

        [Test]
        public void ApplyWindow_GetsRidOfValuesInEndAndStart()
        {
            List<double> testSignal = new List<double>(TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, DefaultSignalFrequency));

            HelperFunctions.ApplyWindow(ref testSignal, new FftSharp.Windows.Hanning());

            testSignal[0].Should().Be(0);
            testSignal[^1].Should().Be(0);
        }

        [Test]
        public void ApplyWindow_ValueInStartIsLowerAfterWindowing()
        {
            List<double> testSignal = new List<double>(TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, DefaultSignalFrequency));
            var beforeValue = testSignal[1]; //sin start from 0 thats why we take second sample

            HelperFunctions.ApplyWindow(ref testSignal, new FftSharp.Windows.Hanning());
            var afterValue = testSignal[1];

            beforeValue.Should().BeGreaterThan(afterValue);
        }

        [Test]
        public void ApplyWindow_LeavesValuesInMiddle()
        {
            List<double> testSignal = new List<double>(TestSamplesGenerator.GenerateSinWave(DefaultSamplingFrequency, DefaultSignalLength, DefaultSignalAmplitude, DefaultSignalFrequency));
            var beforeValue = testSignal[testSignal.Count/2];

            HelperFunctions.ApplyWindow(ref testSignal, new FftSharp.Windows.Hanning());
            var afterValue = testSignal[testSignal.Count/2];

            beforeValue.Should().BeApproximately(afterValue, 0.01);
        }
    }
}
