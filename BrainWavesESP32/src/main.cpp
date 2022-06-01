/*
    Video: https://www.youtube.com/watch?v=oCMOYS71NIU
    Based on Neil Kolban example for IDF: https://github.com/nkolban/esp32-snippets/blob/master/cpp_utils/tests/BLE%20Tests/SampleNotify.cpp
    Ported to Arduino ESP32 by Evandro Copercini
    updated by chegewara
    changed by jm bw

   The design of creating the BLE server is:
   1. Create a BLE Server
   2. Create a BLE Service
   3. Create a BLE Characteristic on the Service
   4. Create a BLE Descriptor on the characteristic
   5. Start the service.
   6. Start advertising.

   A connect hander associated with the server starts a background task that performs notification
   every couple of seconds.

   See the following for generating UUIDs: https://www.uuidgenerator.net/
*/

/**************************************************************************************\
* Includes
\**************************************************************************************/
#include "Arduino.h"
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>
#include <algorithm>
#include <string>
#include <vector>
#include <cmath>

/**************************************************************************************\
* Custom types
\**************************************************************************************/
enum MessageType
{
  TIME_FREQ__OR__WAVES_MEASURE,
  TEST_MEASURE,
  CANCEL_MEASURE,
  NONE
};

/**************************************************************************************\
* Constants
\**************************************************************************************/
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

static int16_t SinLookUpTable[] = { // todo make different look up table
  2048,2073,2098,2123,2148,2174,2199,2224,
  2249,2274,2299,2324,2349,2373,2398,2423,
  2448,2472,2497,2521,2546,2570,2594,2618,
  2643,2667,2690,2714,2738,2762,2785,2808,
  2832,2855,2878,2901,2924,2946,2969,2991,
  3013,3036,3057,3079,3101,3122,3144,3165,
  3186,3207,3227,3248,3268,3288,3308,3328,
  3347,3367,3386,3405,3423,3442,3460,3478,
  3496,3514,3531,3548,3565,3582,3599,3615,
  3631,3647,3663,3678,3693,3708,3722,3737,
  3751,3765,3778,3792,3805,3817,3830,3842,
  3854,3866,3877,3888,3899,3910,3920,3930,
  3940,3950,3959,3968,3976,3985,3993,4000,
  4008,4015,4022,4028,4035,4041,4046,4052,
  4057,4061,4066,4070,4074,4077,4081,4084,
  4086,4088,4090,4092,4094,4095,4095,4096,
  4096,4096,4095,4095,4094,4092,4090,4088,
  4086,4084,4081,4077,4074,4070,4066,4061,
  4057,4052,4046,4041,4035,4028,4022,4015,
  4008,4000,3993,3985,3976,3968,3959,3950,
  3940,3930,3920,3910,3899,3888,3877,3866,
  3854,3842,3830,3817,3805,3792,3778,3765,
  3751,3737,3722,3708,3693,3678,3663,3647,
  3631,3615,3599,3582,3565,3548,3531,3514,
  3496,3478,3460,3442,3423,3405,3386,3367,
  3347,3328,3308,3288,3268,3248,3227,3207,
  3186,3165,3144,3122,3101,3079,3057,3036,
  3013,2991,2969,2946,2924,2901,2878,2855,
  2832,2808,2785,2762,2738,2714,2690,2667,
  2643,2618,2594,2570,2546,2521,2497,2472,
  2448,2423,2398,2373,2349,2324,2299,2274,
  2249,2224,2199,2174,2148,2123,2098,2073,
  2048,2023,1998,1973,1948,1922,1897,1872,
  1847,1822,1797,1772,1747,1723,1698,1673,
  1648,1624,1599,1575,1550,1526,1502,1478,
  1453,1429,1406,1382,1358,1334,1311,1288,
  1264,1241,1218,1195,1172,1150,1127,1105,
  1083,1060,1039,1017,995,974,952,931,
  910,889,869,848,828,808,788,768,
  749,729,710,691,673,654,636,618,
  600,582,565,548,531,514,497,481,
  465,449,433,418,403,388,374,359,
  345,331,318,304,291,279,266,254,
  242,230,219,208,197,186,176,166,
  156,146,137,128,120,111,103,96,
  88,81,74,68,61,55,50,44,
  39,35,30,26,22,19,15,12,
  10,8,6,4,2,1,1,0,
  0,0,1,1,2,4,6,8,
  10,12,15,19,22,26,30,35,
  39,44,50,55,61,68,74,81,
  88,96,103,111,120,128,137,146,
  156,166,176,186,197,208,219,230,
  242,254,266,279,291,304,318,331,
  345,359,374,388,403,418,433,449,
  465,481,497,514,531,548,565,582,
  600,618,636,654,673,691,710,729,
  749,768,788,808,828,848,869,889,
  910,931,952,974,995,1017,1039,1060,
  1083,1105,1127,1150,1172,1195,1218,1241,
  1264,1288,1311,1334,1358,1382,1406,1429,
  1453,1478,1502,1526,1550,1575,1599,1624,
  1648,1673,1698,1723,1747,1772,1797,1822,
  1847,1872,1897,1922,1948,1973,1998,2023
};

/**************************************************************************************\
* Variables
\**************************************************************************************/
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

/**************************************************************************************\
* Functions
\**************************************************************************************/
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
      samplingFrequency = 50; 
      currentNumberOfSamples = 0;
      shouldMeasure = true;
      return TEST_MEASURE;
    }
    else if (message.compare(CancelMessage) == 0)
    {
      shouldMeasure = false;
      currentNumberOfSamples = 0;
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

/**************************************************************************************\
* BLE Functions
\**************************************************************************************/
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

/**************************************************************************************\
* Main
\**************************************************************************************/
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
          if(currentNumberOfSamples >= expectedNumOfSamples)
          {
            currentNumberOfSamples = 0;
            sendEndMessage = true;
          }
        }
        else
        {
          writeCharacteristic.setValue(EndMessage);
          writeCharacteristic.notify();
           Serial.println("Sended end message");
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
