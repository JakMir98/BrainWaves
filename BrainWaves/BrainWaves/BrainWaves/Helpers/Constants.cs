using System;
using System.Collections.Generic;
using System.Text;

namespace BrainWaves.Helpers
{
    public static class Constants
    {
        public static Guid UartGattServiceId = Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
        public static Guid UartGattCharacteristicSendReceiveId = Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        public static Guid GattCharacteristicSendId = Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8");
        public static Guid GattCharacteristicReceiveId = Guid.Parse("cba1d466-344c-4be3-ab3f-189f80dd7518");
        //public static Guid UartGattCharacteristicSendId = Guid.Parse("0000FFF2-0000-1000-8000-00805f9b34fb");
        //public static Guid SpecialNotificationDescriptorId = Guid.Parse("00002902-0000-1000-8000-00805f9b34fb");

        public const string PrefsCurrentLanguage = "currentLanguage";
        public const string EnglishLanguageCode = "en-us";
        public const string PolishLanguageCode = "pl";
        public const string PrefsTheme = "theme";
        public const string PrefsDarkTheme = "dark";
        public const string PrefsLightTheme = "light";
        public const string PrefsAutomaticServiceChossing = "autoServiceChoosing";
        public const string PrefsSavedServiceUUID = "savedServiceUUID";
        public const string PrefsSavedSendCharacteristicUUID = "savedSendCharUUID";
        public const string PrefsSavedReceiveCharacteristicUUID = "savedReceiveCharUUID";
        public const string PrefsSavedSamplingFrequency = "savedSamplingFreq";
        public const string PrefsSavedTimeToReadMindInMinutes = "savedTimeToReadMind";
    }
}
