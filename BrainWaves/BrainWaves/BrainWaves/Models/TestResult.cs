using System;
namespace BrainWaves.Models
{
	public class TestResult
	{
		public TestResult()
		{
		}

		public TestResult(DateTime timestamp, int numOfReceivedPackets, int numOfIncorrectRecivedPackets)
        {
			Timestamp = timestamp;
			NumberOfReceivedPackets = numOfReceivedPackets;
			NumberOfIncorrectPacketsReceived = numOfIncorrectRecivedPackets;
		}

		public DateTime Timestamp { get; set; }
		public int NumberOfReceivedPackets { get; set; }
		public int NumberOfIncorrectPacketsReceived { get; set; }
		public int NumberOfCorrectPacketsReceived { get => NumberOfReceivedPackets - NumberOfIncorrectPacketsReceived; }

		public static string StringFirstLine()
        {
			return $"{nameof(Timestamp)};{nameof(NumberOfReceivedPackets)};{nameof(NumberOfIncorrectPacketsReceived)}";
        }

		public string DataToStringInLine()
        {
			return $"{Timestamp};{NumberOfReceivedPackets};{NumberOfIncorrectPacketsReceived}";
        }

	}
}

