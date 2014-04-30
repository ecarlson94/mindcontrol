using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using Emotiv;
using Microsoft.Xna.Framework;

namespace WindowsGame1.Windows
{
    public class Settings : Window
    {
        private EmoEngineManager emoEngine;
        private ProgressBar trainingStatus;

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
            emoEngine.CognitivTrainingFailed += EmoEngineOnCognitivTrainingFailed;
            

            FillWindow();
        }

        private void FillWindow()
        {
            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical,
                Name = "SettingsPanel",
            };
            Content = stackPanel;

            

            trainingStatus = new ProgressBar();//Initialize progress bar here
            stackPanel.Children.Add(trainingStatus);
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
            //training failed
            throw new NotImplementedException();
        }

        private void EmoEngineOnCognitivTrainingSucceeded(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            //training succeded
            //ask user for acception or rejection
            //stop the progress bar
            throw new NotImplementedException();
        }

        private void EmoEngineOnCognitivTrainingStarted(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            //training started
            //start the progress bar
            throw new NotImplementedException();
        }

        private void EmoEngineOnCognitivTrainingCompleted(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            //acception completed
            //display confirmation of acceptance
            throw new NotImplementedException();
        }
    }
}