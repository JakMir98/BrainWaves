using System;
namespace BrainWaves.Models
{
	public class TestResult
	{
		public TestResult()
		{
			Timestamp = DateTime.Now;
			NumberOfReceivedPackets = 0;
			NumberOfIncorrectPacketsReceived = 0;
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
		public double CorrectAllRatioInPercentage { get => Math.Round((double)NumberOfCorrectPacketsReceived / NumberOfReceivedPackets * 100, 2); }

		public static string StringFirstLine()
        {
			return $"{nameof(Timestamp)};{nameof(NumberOfReceivedPackets)};{nameof(NumberOfIncorrectPacketsReceived)};{nameof(CorrectAllRatioInPercentage)}";
        }

		public string DataToStringInLine()
        {
			return $"{Timestamp:dd.MM.yyyy-HH:mm:ss};{NumberOfReceivedPackets};{NumberOfIncorrectPacketsReceived};{CorrectAllRatioInPercentage}%";
        }

	}
}

