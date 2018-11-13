# IoT Central Test Beacon

A sample application that sends messages to an Azure IoT hub containing properties and measurements than can be extracted from a Zebra BT Beacon (note this application only sends the messages, it doesn't extract properties from the beacon).

The corresponding IoT Central template requirements are as follows:

**Measurements**

batteryLevel, min 0, max 100
txPower, min -100, max 100

**Properties**

bluetoothAddress, text
beaconType, text
rssi, text
uuid, text






