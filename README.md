## Brain Waves

## Table of contents
* [Design assumptions](#design-assumptions)
* [Technologies](#technologies)
* [Used hardware](#used-hardware)
* [UI](#user-interface)
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
<p float="left">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/ScanDevices_IMG_Android.jpg" width="250" height="500">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/ScanDevices_IMG_MacOS.png" width="500" height="500">
</p>

Generate Test Sine wave
<p float="left">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/GenerateTestSinewave_IMG_Android.jpg" width="250" height="500">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/GenerateTestSinewave_IMG_MacOS.png" width="500" height="500">
</p>

Time and Frequency Charts
<p float="left">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/ChartsTimeFreq_IMG_Android.jpg" width="250" height="500">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/ChartsTimeFreq_IMG_MacOS.png" width="500" height="500">
</p>

Settings Page
<p float="left">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/SettingsPage_IMG_Android.jpg" width="250" height="500">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/SettingsPage_IMG_MacOS.png" width="500" height="500">
</p>

Simple measurement 
<p float="left">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/SimpleMeasurement_IMG_Android.jpg" width="250" height="500">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/SimpleMeasurement_IMG_MacOS.png" width="500" height="500">
</p>

Brain Waves Page
<p float="left">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/BrainWaves_IMG_Android.jpg" width="250" height="500">
  <img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/BrainWaves_IMG_MacOS.png" width="500" height="500">
</p>

Export Test Data look

<img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/ExportTestData_IMG_MacOS.png" width="500" height="500">

Export Time Data

<img src="https://github.com/JakMir98/BrainWaves/blob/main/Images/ExportTimeData_IMG_MacOS.png" width="500" height="500">

## Authors
Jakub Mirota, Bartosz Wójcik
