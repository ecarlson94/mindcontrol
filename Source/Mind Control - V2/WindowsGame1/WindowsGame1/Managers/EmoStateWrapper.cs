using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace WindowsGame1.Managers
{
    public class EmoStateWrapper
    {
        /*--------------------------------------------------------------------*/
        #region Private Fields

        private readonly EmoState _emoState;

        #endregion

        /*--------------------------------------------------------------------*/
        #region Construction
        /// <summary>
        /// Initializes a new instance of EmoStateWrapper class.
        /// </summary>
        /// <param name="emoState">An <see cref="EmoState"/> for the <see cref="EmoStateWrapper"/> properties.</param>
        public EmoStateWrapper(EmoState emoState)
        {
            if (emoState != null)
            {
                _emoState = emoState.Clone() as EmoState;
            }
            else
            {
                throw new ArgumentNullException("emoState", "must be assigned");
            }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoState Method Wrappers

        public EdkDll.EE_CognitivAction_t CognitivCurrentAction
        {
            get { return _emoState.CognitivGetCurrentAction(); }
        }

        public float CognitivCurrentActionPower
        {
            get { return _emoState.CognitivGetCurrentActionPower(); }
        }

        public bool CognitivIsActive
        {
            get { return _emoState.CognitivIsActive(); }
        }

        public int HeadsetOn
        {
            get { return _emoState.GetHeadsetOn(); }
        }

        public int NumContactQualityChannels
        {
            get { return _emoState.GetNumContactQualityChannels(); }
        }

        public float TimeFromStart
        {
            get { return _emoState.GetTimeFromStart(); }
        }

        public EdkDll.EE_SignalStrength_t WirelessSignalStatus
        {
            get { return _emoState.GetWirelessSignalStatus(); }
        }

        public int BatteryChargeLevel
        {
            get
            {
                int batteryChargeLevel;
                int maxChargeLevel;
                _emoState.GetBatteryChargeLevel(out batteryChargeLevel, out maxChargeLevel);

                return batteryChargeLevel;
            }
        }

        public int MaxBatteryChargeLevel
        {
            get
            {
                int batteryChargeLevel;
                int maxChargeLevel;
                _emoState.GetBatteryChargeLevel(out batteryChargeLevel, out maxChargeLevel);

                return maxChargeLevel;
            }
        }

        private static Dictionary<int, EdkDll.EE_DataChannel_t> _inputChannelIndexToDataChannelMap;
        public static Dictionary<int, EdkDll.EE_DataChannel_t> InputChannelIndexToDataChannelMap
        {
            get
            {
                // TBD: A bit hacky.
                Dictionary<EdkDll.EE_InputChannels_t, EdkDll.EE_DataChannel_t> d = InputChannelToDataChannelMap;

                return _inputChannelIndexToDataChannelMap;
            }
        }

        private static Dictionary<EdkDll.EE_InputChannels_t, EdkDll.EE_DataChannel_t> _inputChannelToDataChannelMap;
        public static Dictionary<EdkDll.EE_InputChannels_t, EdkDll.EE_DataChannel_t> InputChannelToDataChannelMap
        {
            get
            {
                if (_inputChannelToDataChannelMap == null)
                {
                    _inputChannelToDataChannelMap = new Dictionary<EdkDll.EE_InputChannels_t, EdkDll.EE_DataChannel_t>();
                    _inputChannelIndexToDataChannelMap = new Dictionary<int, EdkDll.EE_DataChannel_t>();

                    Array inputChannels = Enum.GetValues(typeof(EdkDll.EE_InputChannels_t));

                    for (int i = 0; i < inputChannels.Length; i++)
                    {
                        EdkDll.EE_InputChannels_t inputChannel = (EdkDll.EE_InputChannels_t)inputChannels.GetValue(i);

                        switch (inputChannel)
                        {
                            case EdkDll.EE_InputChannels_t.EE_CHAN_AF3:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.AF3;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.AF3;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_AF4:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.AF4;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.AF4;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_F3:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.F3;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.F3;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_F4:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.F4;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.F3;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_F7:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.F7;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.F7;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_F8:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.F8;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.F8;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_FC5:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.FC5;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.FC5;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_FC6:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.FC6;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.FC6;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_O1:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.O1;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.O1;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_O2:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.O2;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.O2;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_P7:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.P7;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.P7;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_P8:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.P8;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.P8;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_T7:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.T7;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.T7;
                                    break;
                                }
                            case EdkDll.EE_InputChannels_t.EE_CHAN_T8:
                                {
                                    _inputChannelToDataChannelMap[inputChannel] = EdkDll.EE_DataChannel_t.T8;
                                    _inputChannelIndexToDataChannelMap[i] = EdkDll.EE_DataChannel_t.T8;
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }
                    }
                }

                return _inputChannelToDataChannelMap;
            }
        }

        public Dictionary<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t> ContactQualityFromAllChannels
        {
            get
            {
                Dictionary<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t> contactQualityDictionary =
                    new Dictionary<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t>();

                EdkDll.EE_EEG_ContactQuality_t[] contactQualityArray = this._emoState.GetContactQualityFromAllChannels();

                for (int i = 0; i < contactQualityArray.Length; i++)
                {
                    if (InputChannelIndexToDataChannelMap.ContainsKey(i))
                    {
                        EdkDll.EE_DataChannel_t channel = InputChannelIndexToDataChannelMap[i];

                        contactQualityDictionary[channel] = contactQualityArray[i];
                    }
                }

                return contactQualityDictionary;
            }
        }

        #endregion
    }
}
