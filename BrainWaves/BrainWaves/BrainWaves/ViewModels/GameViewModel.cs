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
        private int answerEntryText;
        private string questionLabelText;
        private bool isBrainActivityViewVisible = false;
        private bool isBrainRelaxViewVisible = true;
        private Color answerColor;
        private string labelText;
        public Stopwatch StopwatchGame { get; set; }
        public int DotCounter { get; set; }
        public int ExerciseCounter { get; set; }
        public GameService gameService { get; }

        public GameViewModel()
        {
            gameService = new GameService();
            randomImageSource = new Uri("https://picsum.photos/200/300?random=1");
            DotCounter = 1;
            ExerciseCounter = 1;
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
        public int AnswerEntryText
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
    }
}
