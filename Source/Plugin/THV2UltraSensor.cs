using FanControl.Plugins;

namespace FanControl.THV2Ultra.Plugin
{
    public class THV2UltraSensor : IPluginSensor
    {
        public string Id => "THV2-Liquid-Temp-Only";
        public string Name => "Liquid Temperature";
        public float? Value { get; set; }

        public void Update()
        {
        }
    }
}
