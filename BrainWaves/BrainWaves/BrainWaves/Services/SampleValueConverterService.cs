using System;
using System.Collections.Generic;
using System.Text;

namespace BrainWaves.Services
{
    public static class SampleValueConverterService
    {
        public const float McuVoltage = 3.3f;
        public const int VoltageDividerRescale = 2;
        public const int BitResolution = 4095;
        public const float ConstantComponent = 2.048f;
        public const int VolatgeScalingFactor = 1_000_000; // convert to volt
        public const int EegAmplification = 2_000;

        public static float ConvertToVoltage(byte[] receivedBytes)
        {
            string stringValue = Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length);

            if(float.TryParse(stringValue, out var voltage))
            {
                return ((((voltage * McuVoltage * VoltageDividerRescale) / BitResolution) - ConstantComponent) * VolatgeScalingFactor) / EegAmplification;
            }
            
            return 0.0f;

            //var trasferedString = cos z string zrobic;
            //var sampleValInVolt = ((((trasferedString * 3.3 * 2) / 4095) - 2.048) * 1_000_000) / 2000;
            // razy 3 bo max napiecie,
            // razy 2 bo dzielnik napiecia
            // podzielic na 4095 bo Bit
            // odjąć składowa stałą (zero wirtualne)
            // * milion bo na wolty
            // / 2000 bo empirycznie wyznaczone wzmocnienie
        }

    }
}
