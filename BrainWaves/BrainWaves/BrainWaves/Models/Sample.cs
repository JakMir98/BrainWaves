using System.Globalization;

namespace BrainWaves.Models
{
    /// <summary>Class <c>Sample</c> Sample contains x value and y value
    /// </summary>
    public class Sample
    {
        private const string Specifier = "G";
        private CultureInfo Culture = CultureInfo.CreateSpecificCulture("eu-ES");

        public Sample() { }

        public Sample(double y, double x)
        {
            SampleXValue = x;
            SampleYValue = y;
        }

        public double SampleYValue { get; set; }
        public double SampleXValue { get; set; }

        /// <summary>
        /// This method return x in format with comma as delimeter so that data can be properly seen in excell files 
        /// </summary>
        public string SampleXToString()
        {
            return SampleXValue.ToString(Specifier, Culture);
        }

        /// <summary>
        /// This method return y in format with comma as delimeter so that data can be properly seen in excell files 
        /// </summary>
        public string SampleYToString()
        {
            return SampleYValue.ToString(Specifier, Culture);
        }
    }
}
