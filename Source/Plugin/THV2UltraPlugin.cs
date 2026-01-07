using FanControl.Plugins;
using FanControl.THV2Ultra.Device;

namespace FanControl.THV2Ultra.Plugin
{
    public class THV2UltraPlugin : IPlugin2
    {
        private readonly THV2UltraController _controller = new();
        private readonly THV2UltraSensor _sensor = new();

        public string Name => "THV2 Ultra AIO (Liquid Temperature)";

        public void Initialize()
        {
            _controller.Open();
        }

        public void Close()
        {
            _controller.Dispose();
        }

        public void Load(IPluginSensorsContainer container)
        {
            if (_controller.IsOpen)
            {
                container.TempSensors.Add(_sensor);
            }
        }

        public void Update()
        {
            float? temp = _controller.GetLatestTemperature();
            if (temp.HasValue)
            {
                _sensor.Value = temp.Value;
            }
        }
    }
}
