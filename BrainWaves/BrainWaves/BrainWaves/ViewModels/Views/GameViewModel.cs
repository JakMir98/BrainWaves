using BrainWaves.Helpers;
using BrainWaves.Services;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrainWaves.ViewModels.Views
{
    public class GameViewModel : BaseViewModel
    {
        #region Variables
        private bool isBrainActivityVisible = false;
        private ImageSource randomImageSource;
        private string answerEntryText;
        private string questionLabelText;
        private bool isBrainActivityViewVisible = false;
        private bool isBrainRelaxViewVisible = true;
        private Color answerColor;
        private string labelText;
        private string correctAnswersText;
        private bool isGameVisible;

        public Stopwatch StopwatchGame { get; set; }
        public int ExerciseCounter { get; set; }
        public GameService gameService { get; }
        public int TotalExerciseCounter { get; set; }
        public int TotalCorrectAnswersCounter { get; set; }
        #endregion

        #region ICommands
        public ICommand CheckCommand { private set; get; }
        #endregion

        #region Constructors
        public GameViewModel()
        {
            gameService = new GameService();
            randomImageSource = new Uri("https://picsum.photos/200/300?random=1"); // in future maybe add calming photos 
            ExerciseCounter = 1;
            TotalExerciseCounter = 1;
            TotalCorrectAnswersCounter = 0;
            StopwatchGame = new Stopwatch();
            CheckCommand = new Command(CheckExercise);
        }
        #endregion

        #region INotify Getters and Setters
        public bool IsBrainActivityVisible
        {
            get => isBrainActivityVisible;
            set
            {
                SetProperty(ref isBrainActivityVisible, value);
                if (value)
                {
                    (gameService.CurrentAnswer, QuestionLabelText) = gameService.GenerateExercise();
                }
            }
        }

        public ImageSource RandomImageSource
        {
            get => randomImageSource;
            set => SetProperty(ref randomImageSource, value);
        }
        public string AnswerEntryText
        {
            get => answerEntryText;
            set => SetProperty(ref answerEntryText, value);
        }
        public string QuestionLabelText
        {
            get => questionLabelText;
            set => SetProperty(ref questionLabelText, value);
        }
        public bool IsBrainActivityViewVisible
        {
            get => isBrainActivityViewVisible;
            set => SetProperty(ref isBrainActivityViewVisible, value);
        }
        public bool IsBrainRelaxViewVisible
        {
            get => isBrainRelaxViewVisible;
            set => SetProperty(ref isBrainRelaxViewVisible, value);
        }

        public Color AnswerColor
        {
            get => answerColor;
            set => SetProperty(ref answerColor, value);
        }

        public string LabelText
        {
            get => labelText;
            set => SetProperty(ref labelText, value);
        }

        public string CorrectAnswersText
        {
            get => correctAnswersText;
            set => SetProperty(ref correctAnswersText, value);
        }

        public bool IsGameVisible
        {
            get => isGameVisible;
            set => SetProperty(ref isGameVisible, value);
        }
        #endregion

        #region Functions
        private void CheckExercise()
        {
            if (int.TryParse(AnswerEntryText, out var number))
            {
                if (gameService.CurrentAnswer == number)
                {
                    AnswerColor = Color.Green;
                    TotalCorrectAnswersCounter++;
                }
                else
                {
                    AnswerColor = Color.Red;
                }
            }
            else
            {
                AnswerColor = Color.Red;
            }

            AnswerEntryText = string.Empty;
            (gameService.CurrentAnswer, QuestionLabelText) = gameService.GenerateExercise();
            CorrectAnswersText = $"{Resources.Strings.Resource.CorrectAnswersText}{TotalCorrectAnswersCounter}/{TotalExerciseCounter}";
            TotalExerciseCounter++;
            ExerciseCounter++;
            if (ExerciseCounter > 4 * Constants.DefaultNumOfExercisesToChangeLevel)
            {
                ExerciseCounter = 0;
            }
            else if (ExerciseCounter > 3 * Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameService.Level = DifficultyLevel.ULTRA;
            }
            else if (ExerciseCounter > 2 * Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameService.Level = DifficultyLevel.HARD;
            }
            else if (ExerciseCounter > Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameService.Level = DifficultyLevel.MEDIUM;
            }
            else if (ExerciseCounter < Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameService.Level = DifficultyLevel.EASY;
            }
        }

        public void UpdateUiGame(float timeToMeasureInMins)
        {
            if (IsBrainActivityVisible && StopwatchGame.Elapsed.TotalMinutes > timeToMeasureInMins / 2)
            {
                IsBrainActivityViewVisible = true;
                LabelText = Resources.Strings.Resource.TimeToFocusText;
            }
        }

        public void SetupGame()
        {
            StopwatchGame.Restart();
            IsBrainRelaxViewVisible = true;
            IsBrainActivityViewVisible = false;
            LabelText = Resources.Strings.Resource.TimeForRelaxText;
        }
        #endregion
    }
}
