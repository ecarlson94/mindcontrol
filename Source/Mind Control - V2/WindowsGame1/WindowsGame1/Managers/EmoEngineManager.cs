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

        private uint _userId;
        public uint UserID
        {
            get { return _userId; }
            private set { _userId = value; }
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

        public void StartCognitivTraining(EdkDll.EE_CognitivAction_t action)
        {
            if (Profile != String.Empty)
            {
                emoEngine.CognitivSetTrainingAction(UserID, action);
                emoEngine.CognitivSetTrainingControl(UserID, EdkDll.EE_CognitivTrainingControl_t.COG_START);
                IsTraining = true;
            }
        }

        public void AcceptTraining()
        {
            if (Profile != String.Empty && IsTraining)
            {
                emoEngine.CognitivSetTrainingControl(UserID, EdkDll.EE_CognitivTrainingControl_t.COG_ACCEPT);
            }
        }

        public void RejectTraining()
        {
            if (Profile != String.Empty && IsTraining)
            {
                emoEngine.CognitivSetTrainingControl(UserID, EdkDll.EE_CognitivTrainingControl_t.COG_REJECT);
            }
        }

        public bool IsCognitivActionTrained(EdkDll.EE_CognitivAction_t action)
        {
            bool cognitivActionTrained = false;

            if (emoEngine != null && Profile != String.Empty && activeActions.Contains(action))
                cognitivActionTrained = (emoEngine.CognitivGetActiveActions(UserID) & (uint) action) == (uint) action;

            return cognitivActionTrained;
        }

        public bool AllCognitivActionsTrained()
        {
            bool allActionsTrained = false;

            if (Profile != String.Empty)
            {
                uint actions = emoEngine.CognitivGetActiveActions(UserID);
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
            bool headsetOnHead = false;
            if (currentEmoState != null && Profile != String.Empty)
            {
                var contactQuality = currentEmoState.ContactQualityFromAllChannels;
                int noSignalCount = 0;
                foreach (KeyValuePair<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t> entry in contactQuality)
                {
                    if (entry.Value == EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_NO_SIGNAL)
                        noSignalCount++;
                }

                headsetOnHead = noSignalCount < 5;
            }

            return headsetOnHead;
        }

        public bool HeadsetOn()
        {
            return currentEmoState != null && currentEmoState.HeadsetOn == 1;
        }

        public void SaveProfile(string profileName)
        {
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
            LoadActiveActions();
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
            throw new NotImplementedException("EmoEngineManager: Delete Profiles");
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Private Methods

        private void LoadActiveActions()
        {
            activeActions.Clear();
            uint actions = emoEngine.CognitivGetActiveActions(UserID);
            if((actions & (uint)EdkDll.EE_CognitivAction_t.COG_NEUTRAL) == (uint)EdkDll.EE_CognitivAction_t.COG_NEUTRAL)
                activeActions.Add(EdkDll.EE_CognitivAction_t.COG_NEUTRAL);
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
                cognitivActions = cognitivActions | cognitivActions;
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
            catch (Exception e)
            {
            }

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
            emoEngine.UserAdded += engine_UserAdded;
            emoEngine.UserRemoved += engine_UserRemoved;

            emoEngine.Connect();
        }

        private void InitializeProcessEventWorker()
        {
            _processEventsWorker = new BackgroundWorker();
            _processEventsWorker.WorkerReportsProgress = true;
            _processEventsWorker.WorkerSupportsCancellation = true;
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
            emoEngine.CognitivSetTrainingControl(UserID, EdkDll.EE_CognitivTrainingControl_t.COG_NONE);
            IsTraining = false;
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoEngine Delegates

        private void engine_Connected(object sender, EmoEngineEventArgs e)
        {
            lock (this)
            {
                UserID = e.userId;
            }
        }

        private void EmoEngineOnEmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            lock (this)
            {
                EmoState emoState = new EmoState(e.emoState);

                _processEventsWorker.ReportProgress(0, emoState);
            }
        }

        private void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            lock (this)
            {
                EmoState emoState = new EmoState(e.emoState);

                _processEventsWorker.ReportProgress(0, emoState);
            }
        }

        private void engine_UserAdded(object sender, EmoEngineEventArgs e)
        {
            lock (this)
            {
                UserID = e.userId;
                DonglePluggedIn = true;
            }
        }

        private void engine_UserRemoved(object sender, EmoEngineEventArgs e)
        {
            lock (this)
            {
                UserID = 0;
                Profile = String.Empty;
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
                var trainingAction = emoEngine.CognitivGetTrainingAction(UserID);
                if (!activeActions.Contains(trainingAction))
                    activeActions.Add(trainingAction);
                emoEngine.CognitivSetActiveActions(UserID, GetActiveActions());

                EndTraining();

                SaveProfile(Profile);
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
            lock (this)
            {
                currentEmoState = new EmoStateWrapper(e.UserState as EmoState);
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