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

// See the following for generating UUIDs:
// https://www.uuidgenerator.net/

#define SERVICE_UUID        "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define SEND_CHARACTERISTIC_UUID "beb5483e-36e1-4688-b7f5-ea07361b26a8"
#define READ_CHARACTERISTIC_UUID "cba1d466-344c-4be3-ab3f-189f80dd7518"

#define BUFF_LENGTH 25
const std::string EndMessage = "end";
const std::string StartMessage = "start";
const int eegClickPin = 34;

BLEServer* pServer = NULL;

BLECharacteristic readCharacteristic(READ_CHARACTERISTIC_UUID, 
                  BLECharacteristic::PROPERTY_WRITE); // reading start measuring etc
BLEDescriptor readDescriptor(BLEUUID((uint16_t)0x2902));

BLECharacteristic writeCharacteristic(SEND_CHARACTERISTIC_UUID, 
                  BLECharacteristic::PROPERTY_NOTIFY); // sending data
BLEDescriptor writeDescriptor(BLEUUID((uint16_t)0x2903));

bool deviceConnected = false;
bool oldDeviceConnected = false;
int value = 0;
int counter = 0;
bool sendEndMessage = false;
bool shouldStartMeasure = false;
int samplingFrequency = 200; // 5 mili sec delay
int timeToMeasureInMinutes = 0;

bool get_numbers(std::string inputStr, int * hzOut, int * timeOut)
{
    char c = ';';
    bool firstFound = false;
    bool secondFound = false;
    
    std::size_t firstDelimeter = inputStr.find(c);
    if (firstDelimeter != std::string::npos)
    {
        firstFound = true;
    }
        
    std::size_t secondDelimeter = inputStr.find(c,firstDelimeter+1);
    if (secondDelimeter!=std::string::npos)
    {
        secondFound = true;
    }
    std::string message = inputStr.substr(0, firstDelimeter);

    if(firstFound && secondFound && message.compare("start") == 0)
    {
        std::string firstNum = inputStr.substr(firstDelimeter+1,secondDelimeter-firstDelimeter-1);
        *hzOut = atoi(firstNum.c_str());

        std::string secondNum = inputStr.substr(secondDelimeter+1,inputStr.length()-secondDelimeter);
        *timeOut = atoi(secondNum.c_str());
        return true;
    }

    return false;
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

      if (get_numbers(value, &samplingFrequency, &timeToMeasureInMinutes))
      {
        shouldStartMeasure = true;
        Serial.println("Received start: " + String(samplingFrequency) +"hz "+ String(timeToMeasureInMinutes) + " min");
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
      if(shouldStartMeasure)
      {
        if(!sendEndMessage)
        {
          int eegClickValue = analogRead(eegClickPin);
          char outCharArr[10];
          itoa(value++, outCharArr, 10);
          std::string s = std::string(outCharArr);
          writeCharacteristic.setValue(s);
          writeCharacteristic.notify();
          if(value > 4095)
          {
            value = 0;
            sendEndMessage = true;
          }
        }
        else
        {
          writeCharacteristic.setValue(EndMessage);
          writeCharacteristic.notify();
          sendEndMessage = false;
          shouldStartMeasure = false;
        }
      }
      // bluetooth stack will go into congestion, if too many packets are sent, in 6 hours test i was able to go as low as 3ms
      // 330 possible max cus of ble
      delay(sample_freq_to_microseconds_delay_converter(samplingFrequency));
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