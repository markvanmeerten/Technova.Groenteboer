using HidSharp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Groenteboer.Technova.Devices.Scales.Dymo
{
    public abstract class DymoScale : Scale
    {
        private readonly int _vendorId;
        private readonly int _productId;

        private CancellationTokenSource _cts;
        private Task _readTask;
        private readonly object _lifecycleLock = new object();

        protected DymoScale(int productId, int vendorId = 0x0922, string name = "Dymo")
        {
            _vendorId = vendorId;
            _productId = productId;
            Name = name;
        }

        public override bool IsConnected()
        {
            return DeviceList.Local
                .GetHidDevices(_vendorId, _productId)
                .Any();
        }

        public override void Start()
        {
            lock (_lifecycleLock)
            {
                if (_readTask != null && !_readTask.IsCompleted)
                {
                    return;
                }

                if (_cts != null)
                {
                    _cts.Dispose();
                }

                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                _readTask = Task.Run(delegate
                {
                    ReadLoop(token);
                });
            }
        }

        public override void Stop()
        {
            lock (_lifecycleLock)
            {
                CancellationTokenSource cts = _cts;

                if (cts != null)
                {
                    cts.Cancel();
                }

                _cts = null;

                if (cts != null && _readTask != null)
                {
                    _readTask.ContinueWith(delegate
                    {
                        cts.Dispose();
                    });
                }
            }

            SetStatus(ScaleStatus.Disconnected);
        }

        private void ReadLoop(CancellationToken token)
        {
            try
            {
                var device = DeviceList.Local
                    .GetHidDevices(_vendorId, _productId)
                    .FirstOrDefault();

                if (device == null)
                {
                    SetStatus(ScaleStatus.Disconnected);
                    return;
                }

                using (var stream = device.Open())
                {
                    stream.ReadTimeout = 250;

                    byte[] buffer = new byte[device.GetMaxInputReportLength()];

                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            int count = stream.Read(buffer);

                            if (count >= 6)
                            {
                                ReadWeightFromBuffer(buffer);
                            }
                        }
                        catch (TimeoutException)
                        {
                            // No new weight received.
                        }
                        catch (Exception ex)
                        {
                            ReportError(ex.Message);
                        }
                    }
                }
            }
            finally
            {
                SetStatus(ScaleStatus.Disconnected);
            }
        }

        private void ReadWeightFromBuffer(byte[] buffer)
        {
            byte dymoStatus = buffer[1];
            byte unit = buffer[2];
            sbyte exponent = unchecked((sbyte)buffer[3]);
            short rawWeight = BitConverter.ToInt16(buffer, 4);

            if (unit == 0)
            {
                SetStatus(ScaleStatus.Standby);
                return;
            }

            if (dymoStatus == 5)
            {
                rawWeight = (short)-Math.Abs(rawWeight);
            }

            double weight = rawWeight * Math.Pow(10, exponent);

            string unitText = "";

            if (unit == 2)
                unitText = "g";
            else if (unit == 3)
                unitText = "kg";
            else if (unit == 11)
                unitText = "oz";
            else if (unit == 12)
                unitText = "lb";

            SetStatus(ScaleStatus.Ready);
            SetWeight(weight, unitText);
        }
    }
}
