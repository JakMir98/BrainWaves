/*
    Video: https://www.youtube.com/watch?v=oCMOYS71NIU
    Based on Neil Kolban example for IDF: https://github.com/nkolban/esp32-snippets/blob/master/cpp_utils/tests/BLE%20Tests/SampleNotify.cpp
    Ported to Arduino ESP32 by Evandro Copercini
    updated by chegewara

   Create a BLE server that, once we receive a connection, will send periodic notifications.
   The service advertises itself as: 4fafc201-1fb5-459e-8fcc-c5c9c331914b
   And has a characteristic of: beb5483e-36e1-4688-b7f5-ea07361b26a8

   The design of creating the BLE server is:
   1. Create a BLE Server
   2. Create a BLE Service
   3. Create a BLE Characteristic on the Service
   4. Create a BLE Descriptor on the characteristic
   5. Start the service.
   6. Start advertising.

   A connect hander associated with the server starts a background task that performs notification
   every couple of seconds.
*/
#include "Arduino.h"
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>
#include <algorithm>
#include <string>
#include <vector>
#include <cmath>

enum MessageType
{
  TIME_FREQ__OR__WAVES_MEASURE,
  TEST_MEASURE,
  CANCEL_MEASURE,
  NONE
};

// See the following for generating UUIDs:
// https://www.uuidgenerator.net/

#define SERVICE_UUID        "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define SEND_CHARACTERISTIC_UUID "beb5483e-36e1-4688-b7f5-ea07361b26a8"
#define READ_CHARACTERISTIC_UUID "cba1d466-344c-4be3-ab3f-189f80dd7518"

#define LOOKUP_TABLE_SIZE 512
#define BUFFER_SIZE 10
const std::string EndMessage = "end";
const std::string StartMessage = "start";
const std::string CancelMessage = "cancel";
const std::string StartWavesMeasureStartMessage = "waves";
const std::string TestSingalMessage = "test";
const char Delimeter = ';';
const int eegClickPin = 15;
const int wavesSamplingFreq = 200;
const float wavesTimeToMeasureInMinutes = 10.0;

static int16_t SinLookUpTable[] = { // todo make different look up table
  0x0000,0x0647,0x0c8b,0x12c7,0x18f8,0x1f19,0x2527,0x2b1e,
  0x30fb,0x36b9,0x3c56,0x41cd,0x471c,0x4c3f,0x5133,0x55f4,
  0x5a81,0x5ed6,0x62f1,0x66ce,0x6a6c,0x6dc9,0x70e1,0x73b5,
  0x7640,0x7883,0x7a7c,0x7c29,0x7d89,0x7e9c,0x7f61,0x7fd7,
  0x7fff
};

BLEServer* pServer = NULL;

BLECharacteristic readCharacteristic(READ_CHARACTERISTIC_UUID, 
                  BLECharacteristic::PROPERTY_WRITE); // reading start measuring etc
BLEDescriptor readDescriptor(BLEUUID((uint16_t)0x2902));

BLECharacteristic writeCharacteristic(SEND_CHARACTERISTIC_UUID, 
                  BLECharacteristic::PROPERTY_NOTIFY); // sending data
BLEDescriptor writeDescriptor(BLEUUID((uint16_t)0x2903));

bool deviceConnected = false;
bool oldDeviceConnected = false;
bool sendEndMessage = false;
bool shouldMeasure = false;
int samplingFrequency = 200; // 5 mili sec delay
float timeToMeasureInMinutes = 0;
int expectedNumOfSamples = 0;
int currentNumberOfSamples = 0;
MessageType currentMessageReceived = NONE;

std::vector<double> GenerateTimeVector(int samplingFrequency, int length)
{
  double T = (double)1 / samplingFrequency;            // % Sampling period
  std::vector<double> t(length);
  for (int i = 0; i < length - 1; i++)
  {
      t[i] = i * T;
  }

  return t;
}

std::vector<double> GenerateSinWave(int samplingFrequency, int length, float amplitude, int signalFrequency)
{
  std::vector<double> t = GenerateTimeVector(samplingFrequency, length);
  std::vector<double>sinWave(length);
  for (int i = 0; i < length; i++)
  {
      sinWave[i] = amplitude * sin(2 * PI * signalFrequency * t[i]);
  }
  return sinWave;
}

MessageType decrypt_message(std::string inputStr)// returns 1 when start message, returns 2 when cancel message, returns -1 when error
{  
  std::size_t firstDelimeter = inputStr.find(Delimeter);
  std::string message;
  if (firstDelimeter != std::string::npos)
  {
    message = inputStr.substr(0, firstDelimeter);
    if(message.compare(StartMessage) == 0 || message.compare(StartWavesMeasureStartMessage) == 0)
    {
      std::size_t secondDelimeter = inputStr.find(Delimeter, firstDelimeter+1);
      if (secondDelimeter != std::string::npos)
      {
          std::string firstNum = inputStr.substr(firstDelimeter + 1, secondDelimeter - firstDelimeter - 1);
          samplingFrequency = atoi(firstNum.c_str());

          std::string secondNum = inputStr.substr(secondDelimeter + 1, inputStr.length() - secondDelimeter);
          std::replace(secondNum.begin(), secondNum.end(), ',', '.');
          timeToMeasureInMinutes = atof(secondNum.c_str());

          expectedNumOfSamples = samplingFrequency * timeToMeasureInMinutes * 60;
          shouldMeasure = true;
          return TIME_FREQ__OR__WAVES_MEASURE;
      }
      else
      {
        return NONE;
      }
    }
    else if(message.compare(TestSingalMessage) == 0)
    {
      expectedNumOfSamples = LOOKUP_TABLE_SIZE;
      currentNumberOfSamples = 0;
      shouldMeasure = true;
      return TEST_MEASURE;
    }
    else if (message.compare(CancelMessage) == 0)
    {
      shouldMeasure = false;
      return CANCEL_MEASURE;
    }
    return NONE;
  }
  return NONE;
}

int sample_freq_to_microseconds_delay_converter(int sampleFreq) 
{
  double delayInMicroSeconds = (1.0/sampleFreq) * 1000000; 
  return (int) delayInMicroSeconds;
}

class ServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      deviceConnected = true;
      Serial.println("Connected");
    };

    void onDisconnect(BLEServer* pServer) {
      deviceConnected = false;
      Serial.println("Disconnected");
    }
};

class ReadCharacteristicCallback: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
      std::string value = pCharacteristic->getValue();  

      currentMessageReceived = decrypt_message(value);

      if(currentMessageReceived == TIME_FREQ__OR__WAVES_MEASURE)
      {
        Serial.println("Received START message: " + String(samplingFrequency) +"hz "+ String(timeToMeasureInMinutes) + " min");
      }
      else if(currentMessageReceived == TEST_MEASURE)
      {
        Serial.println("Received TEST message: ");
      }
      else if(currentMessageReceived == CANCEL_MEASURE)
      {
        Serial.println("Received CANCEL message: ");
      }
      else
      {
        if (value.length() > 0) 
        {
          Serial.println("*********");
          Serial.print("Received value: ");
          for (int i = 0; i < value.length(); i++)
            Serial.print(value[i]);
          Serial.println();
          Serial.println("*********");
        }
      }
    }
};

void setup() {
  Serial.begin(115200);

  // Create the BLE Device
  BLEDevice::init("JMBW_ESP32");

  // Create the BLE Server
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new ServerCallbacks());

  // Create the BLE Service
  BLEService *pService = pServer->createService(SERVICE_UUID);

  // Create a BLE Characteristic
  pService->addCharacteristic(&readCharacteristic);
  readDescriptor.setValue("Read characteristic");
  //readCharacteristic.addDescriptor(&readDescriptor);
  readCharacteristic.setCallbacks(new ReadCharacteristicCallback());

  pService->addCharacteristic(&writeCharacteristic);
  //writeDescriptor.setValue("Write characteristic");
  writeCharacteristic.addDescriptor(new BLE2902());

  // Start the service
  pService->start();

  // Start advertising
  BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
  pAdvertising->addServiceUUID(SERVICE_UUID);
  pServer->getAdvertising()->start();
  //pAdvertising->setScanResponse(false);
  //pAdvertising->setMinPreferred(0x0);  // set value to 0x00 to not advertise this parameter
  Serial.println("Waiting a client connection to notify...");
}

void loop() {
    // notify changed value
    if (deviceConnected) 
    {
      if(shouldMeasure)
      {
        if(!sendEndMessage)
        {
          int valueToSend = 6666;
          if (currentMessageReceived == TIME_FREQ__OR__WAVES_MEASURE)
          {
            valueToSend = analogRead(eegClickPin);
          }
          else if(currentMessageReceived == TEST_MEASURE)
          {
            valueToSend = SinLookUpTable[currentNumberOfSamples];
          }
          currentNumberOfSamples++;
          char outCharArr[BUFFER_SIZE];
          itoa(valueToSend, outCharArr, BUFFER_SIZE);
          writeCharacteristic.setValue(std::string(outCharArr));
          writeCharacteristic.notify();
          if(currentNumberOfSamples > expectedNumOfSamples)
          {
            currentNumberOfSamples = 0;
            sendEndMessage = true;
          }
        }
        else
        {
          writeCharacteristic.setValue(EndMessage);
          writeCharacteristic.notify();
          sendEndMessage = false;
          shouldMeasure = false;
        }
      }
      // bluetooth stack will go into congestion, if too many packets are sent, in 6 hours test i was able to go as low as 3ms
      // 330 possible max cus of ble
      delayMicroseconds(sample_freq_to_microseconds_delay_converter(samplingFrequency));
    }
    // disconnecting
    if (!deviceConnected && oldDeviceConnected) {
        delay(500); // give the bluetooth stack the chance to get things ready
        pServer->startAdvertising(); // restart advertising
        Serial.println("start advertising");
        oldDeviceConnected = deviceConnected;
    }
    // connecting
    if (deviceConnected && !oldDeviceConnected) {
        // do stuff here on connecting
        oldDeviceConnected = deviceConnected;
    }
}
