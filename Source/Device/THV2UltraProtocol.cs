using System;
using System.Buffers.Binary;

namespace FanControl.THV2Ultra.Device
{
    public static class THV2UltraProtocol
    {
        public const int VendorId = 0x264A;
        public const int ProductId = 0x233C;

        // Command Identifiers
        private const byte ReportId = 0x00;

        // Common signature for temperature commands and responses
        private static ReadOnlySpan<byte> TempSignature => new byte[] { 0x82, 0x01, 0x00, 0x80 };

        /// <summary>
        /// Checks if the provided buffer contains the temperature signature at any position.
        /// </summary>
        public static bool ContainsTemperatureSignature(ReadOnlySpan<byte> buffer, out int index)
        {
            index = buffer.IndexOf(TempSignature);
            return index >= 0;
        }

        /// <summary>
        /// Writes a temperature request into the provided destination span.
        /// </summary>
        public static void WriteTemperatureRequest(Span<byte> destination)
        {
            destination.Clear();
            destination[0] = ReportId;
            TempSignature.CopyTo(destination.Slice(1));
        }


        /// <summary>
        /// Attempts to parse a temperature value from a raw HID report.
        /// </summary>
    public static float? ParseTemperature(ReadOnlySpan<byte> buffer)
    {
        if (!ContainsTemperatureSignature(buffer, out int index) || buffer.Length < index + 6)
        {
            return null;
        }

        short rawValue = BinaryPrimitives.ReadInt16LittleEndian(buffer.Slice(index + 4, 2));
        return (float)(rawValue / 100.0);
    }

    /// <summary>
    /// Validates if a device's report length matches the THV2 Ultra's known interface.
    /// </summary>
    public static bool IsValidInterface(int maxOutputReportLength)
        {
            return maxOutputReportLength >= 440 && maxOutputReportLength < 500;
        }
    }
}
