using System.Linq;
using FTD2XX_NET;
using System;
using System.ComponentModel;
using System.Threading;

namespace WindowsGame1.RCCar
{
    public enum RelayState
    {
        On,
        Off,
    }

    public enum RelayNumber
    {
        One,
        Two,
        Three,
        Four,
    }

    public class USBRelay
    {
        private byte[] _startup = {0x00};
        private uint _bytesToSend = 1;

        private UInt32 _ftdiDeviceCount = 0;
        private  FTDI.FT_STATUS _status = FTDI.FT_STATUS.FT_OK;

        private FTDI FTDIDevice = new FTDI();

        private BackgroundWorker _determineConnectionWorker;
        public BackgroundWorker DetermineConnectionWorker
        {
            get
            {
                if (_determineConnectionWorker == null)
                {
                    InitializeProcessEventWorker();
                }

                return _determineConnectionWorker;
            }
        }

        public bool USBConnected { get; private set; }

        public USBRelay()
        {
            DetermineConnectionWorker.RunWorkerAsync();
        }

        public void TurnOffAllRelays()
        {
            foreach (var relayNumber in Enum.GetValues(typeof(RelayNumber)).Cast<RelayNumber>())
            {
                RelaySwitch(relayNumber, RelayState.Off);
            }
        }

        public void RelaySwitch(RelayNumber relayNum, RelayState state)
        {
            uint numBytes = 1;
            int relay = 0x00;
            byte[] outBytes = {0x00};
            byte pins = 0x00;
            byte output = 0x00;

            //Find which relays are ON/OFF
            FTDIDevice.GetPinStates(ref pins);

            switch (relayNum)
            {
                case RelayNumber.One:
                    relay = 0x01;
                    break;
                case RelayNumber.Two:
                    relay = 0x02;
                    break;
                case RelayNumber.Three:
                    relay = 0x04;
                    break;
                case RelayNumber.Four:
                    relay = 0x08;
                    break;
            }

            switch (state)
            {
                case RelayState.On:
                    output = (byte) (pins | relay);
                    break;
                case RelayState.Off:
                    output = (byte) (pins & ~(relay));
                    break;
            }

            outBytes[0] = output;
            FTDIDevice.Write(outBytes, 1, ref numBytes);
        }

        private void InitializeProcessEventWorker()
        {
            _determineConnectionWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _determineConnectionWorker.DoWork += processEventsWorker_ProcessEvents;
        }

        private void processEventsWorker_ProcessEvents(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                _status = FTDIDevice.GetNumberOfDevices(ref _ftdiDeviceCount);

                if (!USBConnected && _ftdiDeviceCount != 0)
                {
                    // Allocate storage for device info list
                    FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[_ftdiDeviceCount];

                    // Populate our device list
                    _status = FTDIDevice.GetDeviceList(ftdiDeviceList);

                    // Open first device in our list by serial number
                    _status = FTDIDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);

                    if (_status == FTDI.FT_STATUS.FT_OK)
                    {
                        // Set Baud rate to 9600
                        _status = FTDIDevice.SetBaudRate(9600);

                        // Set FT245RL to synchronous bit-bang mode, used on sainsmart relay board
                        FTDIDevice.SetBitMode(0xFF, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);
                        // Switch off all the relays
                        FTDIDevice.Write(_startup, 1, ref _bytesToSend);
                    }
                }

                USBConnected = _ftdiDeviceCount != 0;

                Thread.Sleep(250);
            }
        }
    }
}