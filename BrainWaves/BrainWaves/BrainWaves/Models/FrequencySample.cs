using System.Globalization;

namespace BrainWaves.Models
{
    public class FrequencySample
    {
        private const string Specifier = "G";
        private CultureInfo Culture = CultureInfo.CreateSpecificCulture("eu-ES");

        public FrequencySample()
        {
        }

        public FrequencySample(double sample, double freq)
        {
            Freq = freq;
            Sample = sample;
        }

        public double Freq { get; set; }
        public double Sample { get; set; }

        public string FreqToString() // designed to export to excell, comma is delimeter
        {
            return Freq.ToString(Specifier, Culture);
        }

        public string SampleToString()
        {
            return Sample.ToString(Specifier, Culture);
        }

    }
}
