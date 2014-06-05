using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Emotiv;
using System;
using WindowsGame1.Managers;
using WindowsGame1.StackPanels;

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
            CanDrag = false;
            Initialize();
        }

        private void Initialize()
        {
            emoEngine.CognitivTrainingCompleted += EmoEngineOnCognitivTrainingCompleted;
            emoEngine.CognitivTrainingSucceeded += EmoEngineOnCognitivTrainingSucceeded;
            emoEngine.CognitivTrainingFailed += EmoEngineOnCognitivTrainingFailed;
            emoEngine.CognitivTrainingDataErased += EmoEngineOnCognitivTrainingDataErased;
            

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

            stackPanel.Children.Add(new TrainingPanel(emoEngine, EdkDll.EE_CognitivAction_t.COG_NEUTRAL));
            stackPanel.Children.Add(new TrainingPanel(emoEngine, EdkDll.EE_CognitivAction_t.COG_PUSH));
            stackPanel.Children.Add(new TrainingPanel(emoEngine, EdkDll.EE_CognitivAction_t.COG_PULL));
            stackPanel.Children.Add(new TrainingPanel(emoEngine, EdkDll.EE_CognitivAction_t.COG_LEFT));
            stackPanel.Children.Add(new TrainingPanel(emoEngine, EdkDll.EE_CognitivAction_t.COG_RIGHT));

            var slider = new SliderWithLable("Detection Sensitivity");
            slider.Slider.Value = emoEngine.GetOverallCognitivSensitivity();
            slider.Slider.Maximum = 7;
            slider.Slider.Minimum = 1;
            slider.Slider.LargeChange = 1;
            slider.Slider.PropertyChanged += (s, e) =>
            {
                if (e.Property.Name.Equals("Value") && slider.Slider.Value%1 == 0f)
                    emoEngine.SetOverallCognitivSensitivity((int)slider.Slider.Value);
            };
            stackPanel.Children.Add(slider);
            

            trainingStatus = new ProgressBar
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                RenderScale = new Vector2F(1.5f),
                Margin = new Vector4F(20),
                IsIndeterminate = true,
            };//Initialize progress bar here
            stackPanel.Children.Add(trainingStatus);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            X = 250;
            Y = 0;
            Width = Screen.ActualWidth - 250;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            //progress bar visibility and update set here
            if (emoEngine.IsTraining && !trainingStatus.IsVisible)
            {
                trainingStatus.IsVisible = true;
            }
            else if (!emoEngine.IsTraining && trainingStatus.IsVisible)
            {
                trainingStatus.IsVisible = false;
            }

            base.OnUpdate(deltaTime);
        }

        private void EmoEngineOnCognitivTrainingDataErased(object sender, EmoEngineEventArgs e)
        {
            //display confirmation of data erase to user here
            var messageBox = new MessageBox("Data Erased", "The data has been successfully erased.");
            messageBox.Show(Screen);
        }

        private void EmoEngineOnCognitivTrainingFailed(object sender, EmoEngineEventArgs e)
        {
            //training failed
            var messageBox = new MessageBox("Training Failed", "The training session failed due to low quality contact.");
            messageBox.Show(Screen);
        }

        private void EmoEngineOnCognitivTrainingSucceeded(object sender, EmoEngineEventArgs e)
        {
            //training succeded
            //ask user for acception or rejection
            //stop the progress bar
            var acceptDialog = new OptionDialog("Accept this training session?", "Confirm", "Accept", "Reject");
            acceptDialog.Closing += (s, ev) =>
            {
                if ((bool)(s as Window).DialogResult)
                {
                    emoEngine.AcceptTraining();
                }
                else
                {
                    emoEngine.RejectTraining();
                }
            };
            acceptDialog.Show(Screen);
        }

        private void EmoEngineOnCognitivTrainingCompleted(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            //acception completed
            //display confirmation of acceptance
            var messageBox = new MessageBox("Training Complete", "The selected direction has been successfully saved");
            messageBox.Show(Screen);
        }
    }
}