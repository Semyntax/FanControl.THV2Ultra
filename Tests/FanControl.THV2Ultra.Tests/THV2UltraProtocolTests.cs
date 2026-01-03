using FanControl.THV2Ultra.Device;

namespace FanControl.THV2Ultra.Tests
{
    [TestClass]
    public class THV2UltraProtocolTests
    {
        [TestMethod]
        public void ParseTemperature_WithRealisticDeviceResponse_ParsesCorrectly()
        {
            byte[] deviceResponse = new byte[440];
            deviceResponse[0] = 0x00;
            deviceResponse[1] = 0x82;
            deviceResponse[2] = 0x01;
            deviceResponse[3] = 0x00;
            deviceResponse[4] = 0x80;
            deviceResponse[5] = 0x84;
            deviceResponse[6] = 0x09;

            float? temperature = THV2UltraProtocol.ParseTemperature(deviceResponse);

            Assert.IsNotNull(temperature, "Temperature should be parsed from valid device response");
            Assert.AreEqual(2436 / 100.0f, temperature.Value, 0.001f, "Parser should correctly divide raw value by 100 (0x0984 = 2436)");
        }

        [TestMethod]
        public void ParseTemperature_WithSignatureAtDifferentPosition_StillParses()
        {
            byte[] buffer = new byte[100];
            buffer[10] = 0x82;
            buffer[11] = 0x01;
            buffer[12] = 0x00;
            buffer[13] = 0x80;
            buffer[14] = 0xE8;
            buffer[15] = 0x03;

            float? temperature = THV2UltraProtocol.ParseTemperature(buffer);

            Assert.IsNotNull(temperature);
            Assert.AreEqual(10.0f, temperature.Value, 0.01f, "Should find signature at offset 10 and parse 10°C");
        }

        [TestMethod]
        public void ParseTemperature_WithCorruptedData_ReturnsNull()
        {
            byte[] corruptedResponse = new byte[440];
            Array.Fill(corruptedResponse, (byte)0xFF);

            float? temperature = THV2UltraProtocol.ParseTemperature(corruptedResponse);

            Assert.IsNull(temperature, "Corrupted data without signature should return null");
        }

        [TestMethod]
        public void WriteTemperatureRequest_CreatesProtocolCompliantRequest()
        {
            byte[] buffer = new byte[440];
            
            THV2UltraProtocol.WriteTemperatureRequest(buffer);

            Assert.AreEqual(0x00, buffer[0], "Report ID must be 0x00");
            Assert.AreEqual(0x82, buffer[1], "First signature byte incorrect");
            Assert.AreEqual(0x01, buffer[2], "Second signature byte incorrect");
            Assert.AreEqual(0x00, buffer[3], "Third signature byte incorrect");
            Assert.AreEqual(0x80, buffer[4], "Fourth signature byte incorrect");
            
            for (int i = 5; i < buffer.Length; i++)
            {
                Assert.AreEqual(0x00, buffer[i], $"Padding at position {i} should be zero");
            }
        }
    }
}
