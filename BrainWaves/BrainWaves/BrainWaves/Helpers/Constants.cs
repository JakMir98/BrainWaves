using SkiaSharp;
using System;

namespace BrainWaves.Helpers
{
    public static class Constants
    {
        public static Guid UartGattServiceId = Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
        public static Guid UartGattCharacteristicSendReceiveId = Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        public static Guid GattCharacteristicSendId = Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        public static Guid GattCharacteristicReceiveId = Guid.Parse("cba1d466-344c-4be3-ab3f-189f80dd7518");

        public const int MinSamplingFrequency = 80; //Hz
        public const int MaxSamplingFrequency = 500; //Hz
        public const int MinTimeToReadInMinutes = 1;
        public const int MaxTimeToReadInMinutes = 60; // max = 1khz * 3600 = 3.6MHZ/h
        public const int DefaultLoadedSamples = 20;
        public const int MaxLoadedSamples = 500;
        public const int NumOfDecimalPlaces = 2;
        public const int MaxEntriesForSheet = 1_000_000;
        public const int DefaultSinwaveSamplingFreq = 1000;
        public const int DefaultSinwaveAmplitude = 1;
        public const int DefaultSinwaveLength = 1024;
        public const int DefaultSinwaveFreq = 50;
        public const int DefaultLowPassFilterMaxFreq = 50; //Hz
        public const int DefaultMaxRandomValue = 1_000_000;
        public const int DefaultNumOfExercisesToChangeLevel = 10;

        public const string EnglishLanguageCode = "en-us";
        public const string PolishLanguageCode = "pl";
        public const string StartMeasureStartMessage = "start";
        public const char Delimeter = ';';
        public const string EndMeasureEndMessage = "end";
        public const string TestSingalMessage = "test";

        public const string PrefsCurrentLanguage = "currentLanguage";
        public const string PrefsTheme = "theme";
        public const string PrefsDarkTheme = "dark";
        public const string PrefsLightTheme = "light";
        public const string PrefsAutomaticServiceChossing = "autoServiceChoosing";
        public const string PrefsSavedServiceUUID = "savedServiceUUID";
        public const string PrefsSavedSendCharacteristicUUID = "savedSendCharUUID";
        public const string PrefsSavedReceiveCharacteristicUUID = "savedReceiveCharUUID";
        public const string PrefsSavedSamplingFrequency = "savedSamplingFreq";
        public const string PrefsSavedTimeToReadMindInMinutes = "savedTimeToReadMind";
        public const string PrefsSamplesToShowFromMiddle = "samplesToShow";
        public const string PrefsShouldCalculateFFT = "shouldCalculateFFT";
        public const string PrefsSinwaveSamplingFreq = "sinwaveSamplingFreq";
        public const string PrefsSinwaveAmplitude = "sinwaveAmplitude";
        public const string PrefsSinwaveLength = "sinwaveLength";
        public const string PrefsSinwaveFreq = "sinwaveFreq";
        public const string PrefsCutoffFreqOfLowPassFilter = "cutoffFreqLowPass";

        public const string ExcellSheetName = "Samples";
        public static string[] ExcelSheetNames =
        {
            "Samples 0-1M",
            "Samples 1M-2M",
            "Samples 2M-3M",
            "Samples 3M-4M",
            "Samples 4M-5M",
            "Samples 5M-6M",
            "Samples 6M-7M",
            "Samples 7M-8M",
            "Samples 8M-9M",
            "Samples 9M-10M"
        };

        public static SKColor[] Colors =
        {
            SkiaSharp.SKColor.Parse("#ff0000"),
            SkiaSharp.SKColor.Parse("00BFFF"),
            SkiaSharp.SKColor.Parse("#00D100"),
            SkiaSharp.SKColor.Parse("#8B4513"),
            SkiaSharp.SKColor.Parse("#800080"),
            SkiaSharp.SKColor.Parse("#FF8C00"),
        };

        public static string[] WavesChartLabels =
        {
            "0 - 10 min",
            "10 - 20 min",
            "20 - 30 min",
            "30 - 40 min",
            "40 - 50 min",
            "50 - 60 min",
        };
    }
}
