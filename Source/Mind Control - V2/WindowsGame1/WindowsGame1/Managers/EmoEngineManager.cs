
using System;
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

        #endregion

        /*--------------------------------------------------------------------*/
        #region Construction

        public EmoEngineManager()
        {
            emoEngine = EmoEngine.Instance;
        }

        static EmoEngineManager()
        {
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Properties

        private uint _userId;
        public uint UserID
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private volatile bool _donglePluggedIn = false;
        public bool DonglePluggedIn
        {
            get { return _donglePluggedIn; }
            set
            {
                _donglePluggedIn = value;
            }
        }

        private EmoStateWrapper currentEmoState;
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

        private string _profile = "None";
        public string Profile
        {
            get { return _profile; }
            set { _profile = value; }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Public Methods

        public bool ProfileLoggedIn()
        {
            return !Profile.Equals("None");
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

        public void SaveProfile(string profileName)
        {
            Profile tmp = emoEngine.GetUserProfile(0);
            ByteArrayToFile(GetProfilePath(profileName), tmp.GetBytes());
        }

        public void CreateProfile(string profileName)
        {
            emoEngine.EE_SaveUserProfile(UserID, GetProfilePath(profileName));
        }

        public void LoadProfile(string profileName)
        {
            Profile = profileName;

            FileStream fileStream = new FileStream(GetProfilePath(profileName), FileMode.Open, FileAccess.Read);
            Byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int) fileStream.Length);
            fileStream.Close();
            EdkDll.EE_SetUserProfile(UserID, buffer, (uint)buffer.Length);
            emoEngine.LoadUserProfile(UserID, GetProfilePath(profileName));
        }

        public string[] GetProfileNames()
        {
            string[] fileNames = Directory.GetFiles("Profiles");

            for(int i = 0; i < fileNames.Length; i++)
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

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoEngine Event Handlers

        private void engine_Connected(object sender, EmoEngineEventArgs e)
        {
            lock (this)
            {
                UserID = e.userId;
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
                DonglePluggedIn = false;
            }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoEngine Event Wrappers

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

        #endregion

        /*--------------------------------------------------------------------*/
        #region BackgroundWork Event Handlers

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
