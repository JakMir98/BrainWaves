using System;
using System.Text;
using FftSharp;

namespace BrainWaves.Services
{
    public class SampleTranformService
    {
        public SampleTranformService()
        {
        }

        public const float McuVoltage = 3.3f;
        public const int VoltageDividerRescale = 2;
        public const int BitResolution = 4095;
        public const float ConstantComponent = 2.048f;
        public const int VolatgeScalingFactor = 1_000_000; // convert to volt
        public const int EegAmplification = 2_000;

        //var trasferedString = cos z string zrobic;
        //var sampleValInVolt = ((((trasferedString * 3.3 * 2) / 4095) - 2.048) * 1_000_000) / 2000;
        // razy 3 bo max napiecie,
        // razy 2 bo dzielnik napiecia
        // podzielic na 4095 bo Bit
        // odjąć składowa stałą (zero wirtualne)
        // * milion bo na wolty
        // / 2000 bo empirycznie wyznaczone wzmocnienie
        public float ConvertToVoltage(byte[] receivedBytes)
        {
            string stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);

            return ConvertToVoltage(stringValue);
        }

        public float ConvertToVoltage(string receivedString)
        {
            if (float.TryParse(receivedString, out var voltage))
            {
                return ((((voltage * McuVoltage * VoltageDividerRescale) / BitResolution) - ConstantComponent) * VolatgeScalingFactor) / EegAmplification;
            }

            return 0.0f;
        }

        public void Filter(double[] samples, double sampleRate)
        {
            double[] filtered = FftSharp.Filter.LowPass(samples, sampleRate, maxFrequency: 2000);
            FftSharp.Filter.BandPass(samples, sampleRate, minFrequency: 20, maxFrequency: 2000);
            FftSharp.Filter.BandStop(samples, sampleRate, minFrequency: 20, maxFrequency: 2000);
            FftSharp.Filter.HighPass(samples, sampleRate, minFrequency: 20);


        }

        public void Window(double[] signal)
        {
            var window = new FftSharp.Windows.Hanning();
            double[] windowed = window.Apply(signal);
        }

        public void Transform(double[] signal)
        {
            Complex[] fftRaw = FftSharp.Transform.FFT(signal);
        }

        public void TransformBack(Complex[] signal)
        {
            FftSharp.Transform.IFFT(signal);
        }
    }
}
