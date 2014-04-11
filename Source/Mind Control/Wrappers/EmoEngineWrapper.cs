using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Emotiv;

namespace Mind_Control.Wrappers
{
    public class EmoEngineWrapper : INotifyPropertyChanged, IDisposable
    {
        /*--------------------------------------------------------------------*/
        #region Private Fields

        private const int _maxEmotivPollLapse = 10000;

        private EmoEngine _emoEngine;

        #endregion

        /*--------------------------------------------------------------------*/
        #region Construction

        public EmoEngineWrapper()
        {
            _emoEngine = EmoEngine.Instance;
        }

        static EmoEngineWrapper()
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

        private bool _donglePluggedIn = false;
        public bool DonglePluggedIn
        {
            get { return _donglePluggedIn; }
            set
            {
                _donglePluggedIn = value;
                NotifyPropertyChanged("DonglePluggedIn");
            }
        }

        private EmoStateWrapper _emoState;
        public EmoStateWrapper CurrentEmoState
        {
            get { return _emoState; }
            set { _emoState = value; }
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
                    NotifyPropertyChanged("EmotivPollLapse");
                }
            }
        }

        public bool CanStartEmoEngine
        {
            get { return (_emoEngine != null || !IsEmoEngineRunning); }
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
                    NotifyPropertyChanged("IsEmoEngineRunning");
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

        #endregion

        /*--------------------------------------------------------------------*/
        #region Public Methods

        public void StartEmoEngine()
        {
            if (CanStartEmoEngine)
            {
                InitializeEmoEngineWrapper();
                ProcessEventsWorker.RunWorkerAsync();
                _isEmoEngineRunning = true;
                NotifyPropertyChanged("CanStartEmoEngine");
            }
            else
            {
                throw new InvalidOperationException("The EmoEngine could not be started.");
            }
        }

        public void SaveProfile(string profileName)
        {
            _emoEngine.EE_SaveUserProfile(UserID, GetProfilePath(profileName));
        }

        public void LoadProfile(string profileName)
        {
            _emoEngine.LoadUserProfile(UserID, GetProfilePath(profileName));
        }

        public string[] GetProfileNames()
        {
            string[] fileNames = Directory.GetFiles("Profiles");

            for(int i = 0; i < fileNames.Length; i++)
            {
                Match regexMatch = Regex.Match(fileNames[i], @"(\w*).emu");
                if (regexMatch != null)
                {
                    fileNames[i] = regexMatch.Value.Substring(0, regexMatch.Value.Length - 4);
                }
            }

            return fileNames;
        }

        public void DeleteProfile(string profileName)
        {
            throw new NotImplementedException();
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region Private Methods

        private string GetProfilePath(string profileName)
        {
            return "Profiles/" + profileName + ".emu";
        }

        private void InitializeEmoEngineWrapper()
        {
            _emoEngine.EmoEngineConnected += engine_Connected;
            _emoEngine.EmoStateUpdated += engine_EmoStateUpdated;
            _emoEngine.UserAdded += engine_UserAdded;
            _emoEngine.UserRemoved += engine_UserRemoved;

            _emoEngine.Connect();
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
            if (_emoEngine != null)
            {
                _emoEngine.Disconnect();
                _emoEngine = null;
            }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoEngine Event Handlers

        private void engine_Connected(object sender, EmoEngineEventArgs e)
        {
            UserID = e.userId;
        } 

        private void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState emoState = new EmoState(e.emoState);

            _processEventsWorker.ReportProgress(0, emoState);
        }

        private void engine_UserAdded(object sender, EmoEngineEventArgs e)
        {
            UserID = e.userId;
            DonglePluggedIn = true;
        }

        private void engine_UserRemoved(object sender, EmoEngineEventArgs e)
        {
            UserID = 0;
            DonglePluggedIn = false;
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region EmoEngine Event Wrappers

        public event EmoEngine.UserAddedEventHandler UserAdded
        {
            add { _emoEngine.UserAdded += value; }
            remove { _emoEngine.UserAdded -= value; }
        }

        public event EmoEngine.UserRemovedEventHandler UserRemoved
        {
            add { _emoEngine.UserRemoved += value; }
            remove { _emoEngine.UserRemoved -= value; }
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region BackgroundWork Event Handlers

        private void processEventsWorker_ProcessEvents(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                _emoEngine.ProcessEvents(1000);

                Thread.Sleep(EmotivPollLapse);
            }
        }

        private void processEventsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _emoState = new EmoStateWrapper(e.UserState as EmoState);

            NotifyPropertyChanged("CurrentEmoState");
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

        ~EmoEngineWrapper()
        {
            Dispose(false);
        }

        #endregion

        /*--------------------------------------------------------------------*/
        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion
    }
}