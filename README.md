## Brain Waves

## Table of contents
* [Design assumptions](#design-assumptions)
* [Technologies](#technologies)
* [Used hardware](#used-hardware)
* [UI] (#user-interface)
* [Authors](#authors)

## Design assumptions:
- Application written in cross platform technology - Xamarin Forms
- Application can be run on MacOS, Android, iOS
- Application can connect to ESP32 using Bluetooth Low Energy
- Possibility to visualise data gathered from ESP32 (time and frequency domain)
- Send/Receive test signal to check BLE data transfer
- In app generate sine wave to test filtering and windowing data in Charts Page
- Export data to excel (time and frequency domain)
- Export test result data to csv

## Technologies
Project is created with:
* Visual Studio 2022
* Xamarin Forms
* Visual Studio Code
* PlatformIO

## Used hardware:
- ESP32 Dev Kit V1
- EEG Click
- MacOS, Android Phone

## UI
Scan Page
![Scan Page - Android](https://github.com/JakMir98/BrainWaves/blob/main/Images/ScanDevices_IMG_Android.jpeg)
![Scan Page - MacOS](https://github.com/JakMir98/BrainWaves/blob/main/Images/ScanDevices_IMG_MacOS.jpeg)

Generate Test Sine wave
![Generate Test Sine wave - Android](https://github.com/JakMir98/BrainWaves/blob/main/Images/GenerateTestSinewave_IMG_Android.jpeg)
![Generate Test Sine wave - MacOS](https://github.com/JakMir98/BrainWaves/blob/main/Images/GenerateTestSinewave_IMG_MacOS.jpeg)

Time and Frequency Charts
![Time and Frequency Page - Android](https://github.com/JakMir98/BrainWaves/blob/main/Images/ChartsTimeFreq_IMG_Android.jpeg)
![Time and Frequency Page - MacOS](https://github.com/JakMir98/BrainWaves/blob/main/Images/ChartsTimeFreq_IMG_MacOS.jpeg)

Settings Page
![Settings Page - Android](https://github.com/JakMir98/BrainWaves/blob/main/Images/SettingsPage_IMG_Android.jpeg)
![Settings Page - MacOS](https://github.com/JakMir98/BrainWaves/blob/main/Images/SettingsPage_IMG_MacOS.jpeg)

Simple measurement 
![Simple measurement - Android](https://github.com/JakMir98/BrainWaves/blob/main/Images/SimpleMeasurement_IMG_Android.jpeg)
![Simple measurement - MacOS](https://github.com/JakMir98/BrainWaves/blob/main/Images/SimpleMeasurement_IMG_MacOS.jpeg)

Brain Waves Page
![Brain Waves Page - Android](https://github.com/JakMir98/BrainWaves/blob/main/Images/BrainWaves_IMG_Android.jpeg)
![Brain Waves Page - MacOS](https://github.com/JakMir98/BrainWaves/blob/main/Images/BrainWaves_IMG_MacOS.jpeg)

Export Test Data look
![Test Data](https://github.com/JakMir98/BrainWaves/blob/main/Images/ExportTestData_IMG_MacOS.jpeg)

Export Time Data
![Settings Page - MacOS](https://github.com/JakMir98/BrainWaves/blob/main/Images/ExportTimeData_IMG_MacOS.jpeg)

## Authors
Jakub Mirota, Bartosz Wójcik