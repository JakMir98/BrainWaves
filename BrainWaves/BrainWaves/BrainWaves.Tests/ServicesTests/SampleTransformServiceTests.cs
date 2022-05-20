using BrainWaves.Services;
using FluentAssertions;
using NUnit.Framework;

namespace BrainWaves.Tests.ServicesTests
{
    public class SampleTransformServiceTests
    {
        const double RangeValue = 0.0007;
        [Test]
        [TestCase("0", 0)]
        [TestCase("651", -0.0003329f)]
        [TestCase("980", -0.000156f)]
        [TestCase("1500", 0.000123f)]
        [TestCase("2048", 0.000417f)]
        public void ConvertToVoltage_ReturnsCorrectValue(string inputValue, float expectedOutputValue)
        {
            SampleTranformService sampleTransformService = new SampleTranformService();

            var output = sampleTransformService.ConvertToVoltage(inputValue);

            output.Should().BeInRange((float)(expectedOutputValue - RangeValue), (float)(expectedOutputValue + RangeValue));
        }
    }
}
