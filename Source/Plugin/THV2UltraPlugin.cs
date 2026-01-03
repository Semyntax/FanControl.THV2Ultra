using System.Threading;
using System.Threading.Tasks;
using FanControl.Plugins;
using FanControl.THV2Ultra.Device;

namespace FanControl.THV2Ultra.Plugin
{
    public class THV2UltraPlugin : LazyPlugin2
    {
        private readonly THV2UltraController _controller = new();
        private readonly THV2UltraSensor _sensor = new();
        private float? _latestValue;

        public override string Name => "THV2 Ultra AIO (Liquid Temperature)";

        protected override void InitializeHardware()
        {
            _controller.Open();
        }

        protected override bool CanPoll() => _controller.IsOpen;

        protected override async Task PerformPoll(CancellationToken token)
        {
            float? val = _controller.GetLatestTemperature();
            if (val.HasValue)
            {
                _latestValue = val.Value;
            }

            // High frequency polling (1Hz) is fine because LazyPlugin2 handles the idle sleep
            await Task.Delay(1000, token);
        }

        protected override void OnUpdate()
        {
            _sensor.Value = _latestValue;
        }

        public override void Load(IPluginSensorsContainer container)
        {
            if (_controller.IsOpen)
            {
                container.TempSensors.Add(_sensor);
            }
        }

        protected override void CloseHardware()
        {
            _controller.Dispose();
        }
    }
}
