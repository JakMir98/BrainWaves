namespace BrainWaves.Models
{
    public class BrainWaveSample
    {
        public BrainWaveSample() { }

        public BrainWaveSample(double alfa, double beta, double delta, double theta)
        {
            AlfaWave = alfa;
            BetaWave = beta;
            DeltaWave = delta;
            ThetaWave = theta;
        }

        public double AlfaWave { get; set; }
        public double BetaWave { get; set; }
        public double DeltaWave { get; set; }
        public double ThetaWave { get; set; }
    }
}
