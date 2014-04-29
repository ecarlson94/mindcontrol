using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using Emotiv;

namespace WindowsGame1.Windows
{
    public class Settings : Window
    {
        private EmoEngineManager emoEngine;
        private EdkDll.EE_CognitivAction_t trainingAction;

        public Settings(EmoEngineManager emoEngineParam)
        {
            emoEngine = emoEngineParam;
            VerticalAlignment = VerticalAlignment.Stretch;
            Title = "Settings";
            HideOnClose = false;
            Initialize();
        }

        private void Initialize()
        {
            emoEngine.CognitivTrainingCompleted += EmoEngineOnCognitivTrainingCompleted;
            emoEngine.CognitivTrainingStarted += EmoEngineOnCognitivTrainingStarted;
            emoEngine.CognitivTrainingSucceeded += EmoEngineOnCognitivTrainingSucceeded;
            emoEngine.CognitivTrainingRejected += EmoEngineOnCognitivTrainingRejected;
            emoEngine.CognitivTrainingFailed += EmoEngineOnCognitivTrainingFailed;

            MakePanels();
        }

        private void MakePanels()
        {
            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical,
                Name = "SettingsPanel",
            };
            Content = stackPanel;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            X = 250;
            Y = 0;
            Width = Screen.ActualWidth - 250;
        }

        private void EmoEngineOnCognitivTrainingFailed(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            throw new NotImplementedException();
        }

        private void EmoEngineOnCognitivTrainingRejected(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            throw new NotImplementedException();
        }

        private void EmoEngineOnCognitivTrainingSucceeded(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            throw new NotImplementedException();
        }

        private void EmoEngineOnCognitivTrainingStarted(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            throw new NotImplementedException();
        }

        private void EmoEngineOnCognitivTrainingCompleted(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            throw new NotImplementedException();
        }
    }
}