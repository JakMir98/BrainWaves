namespace BrainWaves.Models
{
    public class FrequencySamplesContainer
    {
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
