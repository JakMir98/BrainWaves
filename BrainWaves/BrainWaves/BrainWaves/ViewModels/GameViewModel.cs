using BrainWaves.Services;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public  class GameViewModel : BaseViewModel
    {
        private bool isBrainActivityVisible = false;
        private ImageSource randomImageSource;
        private string answerEntryText;
        private string questionLabelText;
        private bool isBrainActivityViewVisible = false;
        private bool isBrainRelaxViewVisible = true;
        private Color answerColor;
        private string labelText;
        private string correctAnswersText;
        public Stopwatch StopwatchGame { get; set; }
        public int ExerciseCounter { get; set; }
        public GameService gameService { get; }
        public int TotalExerciseCounter { get; set; }
        public int TotalCorrectAnswersCounter { get; set; }

        public GameViewModel()
        {
            gameService = new GameService();
            randomImageSource = new Uri("https://picsum.photos/200/300?random=1"); // in future maybe add calming photos 
            ExerciseCounter = 1;
            TotalExerciseCounter = 1;
            TotalCorrectAnswersCounter = 0;
            StopwatchGame = new Stopwatch();
        }

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
    }
}
