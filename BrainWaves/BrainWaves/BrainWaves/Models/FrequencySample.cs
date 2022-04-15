using System.Globalization;

namespace BrainWaves.Models
{
    /// <summary>Class <c>FrequencySample</c> Frequency sample contains sample and frequency for this sample
    /// </summary>
    public class FrequencySample
    {
        private const string Specifier = "G";
        private CultureInfo Culture = CultureInfo.CreateSpecificCulture("eu-ES");

        public FrequencySample() {}

        public FrequencySample(double sample, double freq)
        {
            Freq = freq;
            Sample = sample;
        }

        public double Freq { get; set; }
        public double Sample { get; set; }

        /// <summary>
        /// This method return freq in format with comma as delimeter so that data can be properly seen in excell files 
        /// </summary>
        public string FreqToString()
        {
            return Freq.ToString(Specifier, Culture);
        }

        /// <summary>
        /// This method return sample in format with comma as delimeter so that data can be properly seen in excell files 
        /// </summary>
        public string SampleToString()
        {
            return Sample.ToString(Specifier, Culture);
        }

    }
}
