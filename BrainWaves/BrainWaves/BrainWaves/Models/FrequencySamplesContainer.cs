namespace BrainWaves.Models
{
    /// <summary>Class <c>FrequencySamplesContainer</c>
    /// Frequency sample contains samples and frequencies for this samples
    /// </summary>
    public class FrequencySamplesContainer
    {
        /// <summary>
        /// Init with data, both samples and freqz should be same len
        /// </summary>
        public FrequencySamplesContainer(double[] samples, double[] freqz)
        {
            Samples = new FrequencySample[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                Samples[i] = new FrequencySample(samples[i], freqz[i]);
            }
        }

        public FrequencySample[] Samples { get; set; }
    }
}
