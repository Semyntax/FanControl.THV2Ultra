using System;
using HidSharp;

namespace FanControl.THV2Ultra.Device
{
    /// <summary>
    /// Pure hardware controller for the Thermaltake THV2 Ultra.
    /// This class has no dependency on FanControl and can be reused in CLI tools, etc.
    /// </summary>
    public class THV2UltraController : IDisposable
    {
        private HidStream? _stream;
        private int _bufSize;
        private byte[]? _readBuffer;
        private byte[]? _writeBuffer;

        public bool IsOpen => _stream != null;

        public bool Open()
        {
            try
            {
                var devices = DeviceList.Local.GetHidDevices(THV2UltraProtocol.VendorId, THV2UltraProtocol.ProductId);
                foreach (var dev in devices)
                {
                    int maxOut = dev.GetMaxOutputReportLength();
                    if (THV2UltraProtocol.IsValidInterface(maxOut))
                    {
                        if (dev.TryOpen(out _stream))
                        {
                            _bufSize = maxOut;
                            _stream.ReadTimeout = 100;

                            // Pre-allocate buffers once
                            _readBuffer = new byte[_bufSize];
                            _writeBuffer = new byte[_bufSize];
                            THV2UltraProtocol.WriteTemperatureRequest(_writeBuffer);

                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public float? GetLatestTemperature()
        {
            if (_stream == null || _writeBuffer == null || _readBuffer == null)
                return null;

            try
            {
                _stream.Write(_writeBuffer);

                float? lastFound = null;
                int bytesRead;
                do
                {
                    bytesRead = _stream.Read(_readBuffer);
                    
                    float? current = THV2UltraProtocol.ParseTemperature(_readBuffer.AsSpan(0, bytesRead));
                    if (current.HasValue)
                        lastFound = current;

                    if (lastFound.HasValue && _stream.ReadTimeout > 0)
                        break;
                } while (bytesRead > 0);
                
                return lastFound;
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            _stream?.Close();
            _stream = null;
        }
    }
}
