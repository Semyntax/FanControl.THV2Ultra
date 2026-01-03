using System;
using System.Threading;
using System.Threading.Tasks;
using FanControl.Plugins;

namespace FanControl.THV2Ultra.Plugin
{
    public abstract class LazyPlugin2 : IPlugin2
    {
        private CancellationTokenSource? _cts;
        private long _lastUpdateTicks = 0;
        private bool _isPolling = false;
        private readonly object _pollingLock = new();

        public abstract string Name { get; }

        public void Initialize()
        {
            InitializeHardware();
        }

        public void Close()
        {
            StopPolling();
            CloseHardware();
        }

        public abstract void Load(IPluginSensorsContainer container);

        public void Update()
        {
            Interlocked.Exchange(ref _lastUpdateTicks, DateTime.UtcNow.Ticks);

            if (!_isPolling && CanPoll())
            {
                StartPolling();
            }

            OnUpdate();
        }

        protected virtual void OnUpdate() { }

        protected abstract void InitializeHardware();
        protected abstract void CloseHardware();
        protected abstract bool CanPoll();
        protected abstract Task PerformPoll(CancellationToken token);

        private void StartPolling()
        {
            lock (_pollingLock)
            {
                if (_isPolling)
                    return;

                _cts = new CancellationTokenSource();
                _isPolling = true;
                Task.Run(() => PollingLoop(_cts.Token));
            }
        }

        private void StopPolling()
        {
            lock (_pollingLock)
            {
                _isPolling = false;
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async Task PollingLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                long lastTicks = Interlocked.Read(ref _lastUpdateTicks);
                if (DateTime.UtcNow.Ticks - lastTicks > TimeSpan.FromSeconds(10).Ticks)
                {
                    lock (_pollingLock)
                    {
                        _isPolling = false;
                        _cts?.Dispose();
                        _cts = null;
                        return;
                    }
                }

                try
                {
                    await PerformPoll(token);
                }
                catch (OperationCanceledException) { break; }
                catch
                {
                    await Task.Delay(5000, token);
                }
            }
        }
    }
}
