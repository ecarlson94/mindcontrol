using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Emotiv;

namespace WindowsGame1.Managers
{
    public class EmoEngineManager : IDisposable
    {
        /*--------------------------------------------------------------------*/
        #region Private Fields

        private const int _maxEmotivPollLapse = 10000;
        private EmoEngine emoEngine;
        private List<EdkDll.EE_CognitivAction_t> activeActions;

        #endregion

        /*--------------------------------------------------------------------*/
        #region Construction

        public EmoEngineManager()
        {
            emoEngine = EmoEngine.Instance;
            activeActions = new List<EdkDll.EE_CognitivAction_t>();
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Properties

        private EdkDll.EE_CognitivTrainingControl_t _trainingStatus = EdkDll.EE_CognitivTrainingControl_t.COG_NONE;
        public EdkDll.EE_CognitivTrainingControl_t TrainingStatus
        {
            get { return _trainingStatus; }
            set
            {
                if (Profile != String.Empty)
                {
                    _trainingStatus = value;
                    emoEngine.CognitivSetTrainingControl(UserID, value);
                }
            }
        }

        private uint _userId;
        public uint UserID
        {
            get { return _userId; }
            private set { _userId = value; }
        }

        private volatile float _overallContactQuality;
        public float OverallContactQuality
        {
            get { return _overallContactQuality; }
            set { _overallContactQuality = value; }
        }

        private bool _isTraining = false;
        public bool IsTraining
        {
            get { return _isTraining; }
            private set { _isTraining = value; }
        }

        private volatile bool _donglePluggedIn = false;
        public bool DonglePluggedIn
        {
            get { return _donglePluggedIn; }
            private set{ _donglePluggedIn = value; }
        }

        private volatile EmoStateWrapper currentEmoState;
        public EmoStateWrapper CurrentEmoState
        {
            get { return currentEmoState; }
            set
            {
                currentEmoState = value;
            }
        }

        private int _emotivPollLapse;
        public int EmotivPollLapse
        {
            get { return _emotivPollLapse; }
            set
            {
                if (value <= 0 || value > _maxEmotivPollLapse)
                {
                    throw new ArgumentException(("must be > 0 and >= " + _maxEmotivPollLapse), "EmotivPollLapse");
                }

                if (value != _emotivPollLapse)
                {
                    _emotivPollLapse = value;
                }
            }
        }

        public bool CanStartEmoEngine
        {
            get { return (emoEngine != null || !IsEmoEngineRunning); }
        }

        public bool CanStopEmoEngine
        {
            get { return IsEmoEngineRunning; }
        }

        private bool _isEmoEngineRunning = false;
        public bool IsEmoEngineRunning
        {
            get { return _isEmoEngineRunning; }
            private set
            {
                if (value != _isEmoEngineRunning)
                {
                    _isEmoEngineRunning = value;
                }
            }
        }

        private BackgroundWorker _processEventsWorker;
        public BackgroundWorker ProcessEventsWorker
        {
            get
            {
                if (_processEventsWorker == null)
                {
                    InitializeProcessEventWorker();
                }

                return _processEventsWorker;
            }
        }

        private string _profile = String.Empty;
        public string Profile
        {
            get { return _profile; }
            private set { _profile = value; }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Public Methods

        public Dictionary<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t> GetContactQualityMap()
        {
            return Profile != String.Empty && currentEmoState != null ? currentEmoState.ContactQualityFromAllChannels : new Dictionary<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t>();
        }

        public void SetOverallCognitivSensitivity(int sensitivity)
        {
            if (Profile != String.Empty)
            {
                if (sensitivity <= 7 && sensitivity >= 1)
                {
                    emoEngine.CognitivSetActivationLevel(UserID, sensitivity);
                    SaveProfile(Profile);
                }
                else
                {
                    throw new Exception("Sensitivity must be within the range 0-7");
                }
            }
        }

        public int GetOverallCognitivSensitivity()
        {
            int sensitivity = 0;

            if (Profile != String.Empty)
                sensitivity = emoEngine.CognitivGetActivationLevel(UserID);

            return sensitivity;
        }

        public uint GetTrainingTime()
        {
            uint trainingTime = 0;

            if (Profile != String.Empty && IsTraining)
                trainingTime = emoEngine.CognitivGetTrainingTime(UserID);

            return trainingTime;
        }

        public void StartCognitivTraining(EdkDll.EE_CognitivAction_t action)
        {
            if (Profile != String.Empty)
            {
                if (action != EdkDll.EE_CognitivAction_t.COG_NEUTRAL &&
                    !activeActions.Contains(action))
                    activeActions.Add(action);
                emoEngine.CognitivSetActiveActions(UserID, GetActiveActions());
                IsTraining = true;
                emoEngine.CognitivSetTrainingAction(UserID, action);
                TrainingStatus = EdkDll.EE_CognitivTrainingControl_t.COG_START;
            }
        }

        public void EraseCognitivTraining(EdkDll.EE_CognitivAction_t action)
        {
            lock (emoEngine)
            {
                if (Profile != String.Empty)
                {
                    IsTraining = true;
                    emoEngine.CognitivSetTrainingAction(UserID, action);
                    TrainingStatus = EdkDll.EE_CognitivTrainingControl_t.COG_ERASE;
                }
            }
        }

        public void AcceptTraining()
        {
            lock (emoEngine)
            {
                if (IsTraining)
                {
                    TrainingStatus = EdkDll.EE_CognitivTrainingControl_t.COG_ACCEPT;
                }
            }
        }

        public void RejectTraining()
        {
            lock (emoEngine)
            {
                if (IsTraining)
                {
                    TrainingStatus = EdkDll.EE_CognitivTrainingControl_t.COG_REJECT;
                }
            }
        }

        public void SetCognitivActionInactive(EdkDll.EE_CognitivAction_t action)
        {
            if (emoEngine != null && Profile != String.Empty && action != EdkDll.EE_CognitivAction_t.COG_NEUTRAL)
            {
                uint activeActions = emoEngine.CognitivGetActiveActions(UserID);
                if ((activeActions & (uint) action) == (uint) action)
                {
                    activeActions = (activeActions | (uint) action);
                }

                emoEngine.CognitivSetActiveActions(UserID, activeActions);
            }
        }

        public void SetAllTrainedCognitivActionActive()
        {
            if (Profile != String.Empty)
            {
                emoEngine.CognitivSetActiveActions(UserID, GetActiveActions());
            }
        }

        public bool IsCognitivActionTrained(EdkDll.EE_CognitivAction_t action)
        {
            bool cognitivActionTrained = false;

            lock (emoEngine)
            {
                if (emoEngine != null && Profile != String.Empty)
                {
                    uint trainedActions = emoEngine.CognitivGetTrainedSignatureActions(UserID);
                    cognitivActionTrained = (trainedActions & (uint) action) ==
                                            (uint) action;
                }
            }

            return cognitivActionTrained;
        }

        public bool AtLeastTwoCogActionsTrained()
        {
            bool twoActionsTrained = IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PUSH);
            if(!twoActionsTrained)
                twoActionsTrained = IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PULL);
            if (!twoActionsTrained)
                twoActionsTrained = IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_LEFT);
            if (!twoActionsTrained)
                twoActionsTrained = IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_RIGHT);

            return twoActionsTrained;
        }

        public bool AllCognitivActionsTrained()
        {
            bool allActionsTrained = false;

            if (Profile != String.Empty)
            {
                lock (emoEngine)
                {
                    uint actions = emoEngine.CognitivGetTrainedSignatureActions(UserID);
                    allActionsTrained = (actions & (uint) EdkDll.EE_CognitivAction_t.COG_NEUTRAL) ==
                                        (uint) EdkDll.EE_CognitivAction_t.COG_NEUTRAL;
                    if (allActionsTrained)
                        allActionsTrained = (actions & (uint) EdkDll.EE_CognitivAction_t.COG_PUSH) ==
                                            (uint) EdkDll.EE_CognitivAction_t.COG_PUSH;
                    if (allActionsTrained)
                        allActionsTrained = (actions & (uint) EdkDll.EE_CognitivAction_t.COG_PULL) ==
                                            (uint) EdkDll.EE_CognitivAction_t.COG_PULL;
                    if (allActionsTrained)
                        allActionsTrained = (actions & (uint) EdkDll.EE_CognitivAction_t.COG_LEFT) ==
                                            (uint) EdkDll.EE_CognitivAction_t.COG_LEFT;
                    if (allActionsTrained)
                        allActionsTrained = (actions & (uint) EdkDll.EE_CognitivAction_t.COG_RIGHT) ==
                                            (uint) EdkDll.EE_CognitivAction_t.COG_RIGHT;
                }
            }

            return allActionsTrained;
        }

        public bool ProfileLoggedIn()
        {
            return Profile != String.Empty;
        }

        public void StartEmoEngine()
        {
            if (CanStartEmoEngine)
            {
                InitializeEmoEngineWrapper();
                ProcessEventsWorker.RunWorkerAsync();
                _isEmoEngineRunning = true;
            }
            else
            {
                throw new InvalidOperationException("The EmoEngine could not be started.");
            }
        }

        public bool HeadsetOnHead()
        {
            return OverallContactQuality > .75f;
        }

        public bool OverallGoodQuality()
        {
            return OverallContactQuality >= 2.5f;
        }

        public bool HeadsetOn()
        {
            return currentEmoState != null && currentEmoState.WirelessSignalStatus != EdkDll.EE_SignalStrength_t.NO_SIGNAL;
        }

        public void SaveProfile(string profileName)
        {
            SetAllTrainedCognitivActionActive();
            Profile tmp = emoEngine.GetUserProfile(0);
            ByteArrayToFile(GetProfilePath(profileName), tmp.GetBytes());
        }

        public void CreateProfile(string profileName)
        {
            emoEngine.CognitivSetActiveActions(UserID, 0);
            SaveProfile(profileName);
        }

        public void LoadProfile(string profileName)
        {
            Profile = profileName;

            FileStream fileStream = new FileStream(GetProfilePath(profileName), FileMode.Open, FileAccess.Read);
            Byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            EdkDll.EE_SetUserProfile(UserID, buffer, (uint)buffer.Length);
            emoEngine.LoadUserProfile(UserID, GetProfilePath(profileName));
            LoadTrainedActions();
        }

        public string[] GetProfileNames()
        {
            string[] fileNames = Directory.GetFiles("Profiles");

            for (int i = 0; i < fileNames.Length; i++)
            {
                Match regexMatch = Regex.Match(fileNames[i], @"([\w ]*).emu");
                if (regexMatch != null)
                {
                    fileNames[i] = regexMatch.Value.Substring(0, regexMatch.Value.Length - 4);
                }
            }

            return fileNames;
        }

        public void DeleteProfile(string profileName)
        {
            string profilePath = GetProfilePath(profileName);
            if(File.Exists(profilePath))
                File.Delete(profilePath);
        }

        public EdkDll.EE_CognitivAction_t CurrentCognitivAction()
        {
            return CurrentEmoState.CognitivCurrentAction;
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Private Methods

        private void LoadTrainedActions()
        {
            activeActions.Clear();
            uint actions = emoEngine.CognitivGetTrainedSignatureActions(UserID);
            if ((actions & (uint)EdkDll.EE_CognitivAction_t.COG_PUSH) == (uint)EdkDll.EE_CognitivAction_t.COG_PUSH)
                activeActions.Add(EdkDll.EE_CognitivAction_t.COG_PUSH);
            if ((actions & (uint)EdkDll.EE_CognitivAction_t.COG_PULL) == (uint)EdkDll.EE_CognitivAction_t.COG_PULL)
                activeActions.Add(EdkDll.EE_CognitivAction_t.COG_PULL);
            if ((actions & (uint)EdkDll.EE_CognitivAction_t.COG_LEFT) == (uint)EdkDll.EE_CognitivAction_t.COG_LEFT)
                activeActions.Add(EdkDll.EE_CognitivAction_t.COG_LEFT);
            if ((actions & (uint)EdkDll.EE_CognitivAction_t.COG_RIGHT) == (uint)EdkDll.EE_CognitivAction_t.COG_RIGHT)
                activeActions.Add(EdkDll.EE_CognitivAction_t.COG_RIGHT);
        }

        private uint GetActiveActions()
        {
            uint cognitivActions = 0x0000;

            foreach (EdkDll.EE_CognitivAction_t activeAction in activeActions)
            {
                cognitivActions = cognitivActions | (uint)activeAction;
            }

            return cognitivActions;
        }

        private bool ByteArrayToFile(string path, byte[] profileBuffer)
        {
            bool success = false;
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                fileStream.Write(profileBuffer, 0, profileBuffer.Length);
                fileStream.Close();
                success = true;
            }
            catch (Exception e){}

            return success;
        }

        private string GetProfilePath(string profileName)
        {
            return "Profiles/" + profileName + ".emu";
        }

        private void InitializeEmoEngineWrapper()
        {
            emoEngine.EmoEngineConnected += engine_Connected;
            emoEngine.EmoStateUpdated += engine_EmoStateUpdated;
            emoEngine.EmoEngineEmoStateUpdated += EmoEngineOnEmoEngineEmoStateUpdated;
            emoEngine.CognitivTrainingFailed += EmoEngineOnCognitivTrainingFailed;
            emoEngine.CognitivTrainingRejected += EmoEngineOnCognitivTrainingRejected;
            emoEngine.CognitivTrainingCompleted += EmoEngineOnCognitivTrainingCompleted;
            emoEngine.CognitivTrainingDataErased += EmoEngineOnCognitivTrainingDataErased;
            emoEngine.UserAdded += engine_UserAdded;
            emoEngine.UserRemoved += engine_UserRemoved;

            emoEngine.Connect();
            emoEngine.ProcessEvents(10000);
        }

        private void InitializeProcessEventWorker()
        {
            _processEventsWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _processEventsWorker.DoWork += processEventsWorker_ProcessEvents;
            _processEventsWorker.ProgressChanged += processEventsWorker_ProgressChanged;
        }

        private void DisconnectEmoEngine()
        {
            if (emoEngine != null)
            {
                emoEngine.Disconnect();
                emoEngine = null;
            }
        }

        private void EndTraining()
        {
            TrainingStatus = EdkDll.EE_CognitivTrainingControl_t.COG_NONE;
            IsTraining = false;
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoEngine Delegates

        private void engine_Connected(object sender, EmoEngineEventArgs e)
        {
            lock (emoEngine)
            {
                UserID = e.userId;
            }
        }

        private void EmoEngineOnEmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            lock (emoEngine)
            {
                EmoState emoState = new EmoState(e.emoState);

                _processEventsWorker.ReportProgress(0, emoState);
            }
        }

        private void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            lock (emoEngine)
            {
                EmoState emoState = new EmoState(e.emoState);

                _processEventsWorker.ReportProgress(0, emoState);
            }
        }

        private void engine_UserAdded(object sender, EmoEngineEventArgs e)
        {
            lock (emoEngine)
            {
                UserID = e.userId;
                DonglePluggedIn = true;
            }
        }

        private void engine_UserRemoved(object sender, EmoEngineEventArgs e)
        {
            lock (emoEngine)
            {
                Profile = String.Empty;
                UserID = 0;
                activeActions.Clear();
                DonglePluggedIn = false;
            }
        }

        private void EmoEngineOnCognitivTrainingFailed(object sender, EmoEngineEventArgs e)
        {
            EndTraining();
        }

        private void EmoEngineOnCognitivTrainingRejected(object sender, EmoEngineEventArgs e)
        {
            EndTraining();
        }

        private void EmoEngineOnCognitivTrainingCompleted(object sender, EmoEngineEventArgs e)
        {
            if (Profile != String.Empty)
            {
                lock (emoEngine)
                {
                    EndTraining();
                    SaveProfile(Profile);
                }
            }
        }

        private void EmoEngineOnCognitivTrainingDataErased(object sender, EmoEngineEventArgs e)
        {

            if (Profile != String.Empty)
            {
                //perhaps move this somewhere else i.e. save button in settings menu TBD
                lock (emoEngine)
                {
                    var trainingAction = emoEngine.CognitivGetTrainingAction(UserID);
                    activeActions.Remove(trainingAction);
                    emoEngine.CognitivSetActiveActions(UserID, GetActiveActions());

                
                    EndTraining();
                    SaveProfile(Profile);
                }
            }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoEngine Event Handler Wrappers

        public event EmoEngine.UserAddedEventHandler UserAdded
        {
            add { emoEngine.UserAdded += value; }
            remove { emoEngine.UserAdded -= value; }
        }

        public event EmoEngine.UserRemovedEventHandler UserRemoved
        {
            add { emoEngine.UserRemoved += value; }
            remove { emoEngine.UserRemoved -= value; }
        }

        public event EmoEngine.CognitivTrainingStartedEventEventHandler CognitivTrainingStarted
        {
            add { emoEngine.CognitivTrainingStarted += value; }
            remove { emoEngine.CognitivTrainingStarted -= value; }
        }

        public event EmoEngine.CognitivTrainingCompletedEventHandler CognitivTrainingCompleted
        {
            add { emoEngine.CognitivTrainingCompleted += value; }
            remove { emoEngine.CognitivTrainingCompleted -= value; }
        }

        public event EmoEngine.CognitivTrainingDataErasedEventHandler CognitivTrainingDataErased
        {
            add { emoEngine.CognitivTrainingDataErased += value; }
            remove { emoEngine.CognitivTrainingDataErased -= value; }
        }

        public event EmoEngine.CognitivTrainingFailedEventHandler CognitivTrainingFailed
        {
            add { emoEngine.CognitivTrainingFailed += value; }
            remove { emoEngine.CognitivTrainingFailed -= value; }
        }

        public event EmoEngine.CognitivTrainingRejectedEventHandler CognitivTrainingRejected
        {
            add { emoEngine.CognitivTrainingRejected += value; }
            remove { emoEngine.CognitivTrainingRejected -= value; }
        }

        public event EmoEngine.CognitivTrainingResetEventHandler CognitivTrainingReset
        {
            add { emoEngine.CognitivTrainingReset += value; }
            remove { emoEngine.CognitivTrainingReset -= value; }
        }

        public event EmoEngine.CognitivTrainingSucceededEventHandler CognitivTrainingSucceeded
        {
            add { emoEngine.CognitivTrainingSucceeded += value; }
            remove { emoEngine.CognitivTrainingSucceeded -= value; }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region BackgroundWork Delegates

        private void processEventsWorker_ProcessEvents(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                emoEngine.ProcessEvents(1000);

                Thread.Sleep(EmotivPollLapse);
            }
        }

        private void processEventsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (emoEngine)
            {
                currentEmoState = new EmoStateWrapper(e.UserState as EmoState);

                if (currentEmoState != null && Profile != String.Empty)
                {
                    var contactQuality = GetContactQualityMap();
                    float qualityTotal = 0;
                    foreach (KeyValuePair<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t> entry in contactQuality)
                    {
                        qualityTotal += (float) entry.Value;
                    }

                    OverallContactQuality = qualityTotal/contactQuality.Count;
                }
                else
                {
                    OverallContactQuality = 0.0f;
                }
            }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Disposal

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (CanStopEmoEngine)
                {
                    _processEventsWorker.CancelAsync();
                }

                DisconnectEmoEngine();
            }
        }

        ~EmoEngineManager()
        {
            Dispose(false);
        }

        #endregion
    }
}