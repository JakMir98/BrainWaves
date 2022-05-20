using System.Text;

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
        public const int EegAmplification = 3_000;

        //var trasferedString = cos z string zrobic;
        //var sampleValInVolt = ((((trasferedString * 3.3 * 2) / 4095) - 2.048) * 1_000_000) / 2000;
        // razy 3 bo max napiecie,
        // razy 2 bo dzielnik napiecia
        // podzielic na 4095 bo Bit
        // odjąć składowa stałą (zero wirtualne)
        // * milion bo na wolty
        // / 2000 bo empirycznie wyznaczone wzmocnienie
        //y_vals[i - 1] = ((y_vals[i - 1] / 4095 * 3.3 * 2) - 2.048)/2 * 1000
        public float ConvertToVoltage(byte[] receivedBytes)
        {
            string stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);

            return ConvertToVoltage(stringValue);
        }

        public float ConvertToVoltage(string receivedString)
        {
            //samples[i]//4095*3.3*2)-2.048)/3000
            if (float.TryParse(receivedString, out var voltage))
            {
                return ((voltage / BitResolution * McuVoltage * VoltageDividerRescale) - ConstantComponent) / EegAmplification;
            }

            return 0.0f;
        }
    }
}
