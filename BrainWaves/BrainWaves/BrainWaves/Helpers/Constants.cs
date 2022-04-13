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

        public const int MaxEntriesForSheet = 1_000_000; 

        public const string EnglishLanguageCode = "en-us";
        public const string PolishLanguageCode = "pl";

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

        public const string ExcellSheetName = "Samples";
        public const string ExcellSheetName1 = "Samples 0-1M";
        public const string ExcellSheetName2 = "Samples 1M-2M";
        public const string ExcellSheetName3 = "Samples 2M-3M";
        public const string ExcellSheetName4 = "Samples 3M-4M";
        public const string ExcellSheetName5 = "Samples 4M-5M";
        public const string ExcellSheetName6 = "Samples 5M-6M";
        public const string ExcellSheetName7 = "Samples 6M-7M";
        public const string ExcellSheetName8 = "Samples 7M-8M";
        public const string ExcellSheetName9 = "Samples 8M-9M";
        public const string ExcellSheetName10 = "Samples 9M-10M";

    }
}
