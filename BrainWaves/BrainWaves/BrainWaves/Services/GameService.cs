using BrainWaves.Helpers;
using System;

namespace BrainWaves.Services
{
    public class GameService
    {
        public int CurrentAnswer { get; set; }
        private Random random;
        public DifficultyLevel Level;

        public GameService()
        {
            random = new Random();
        }

        public (int answer, string stringEquation) GenerateExercise()
        {
            int returnAnswer;
            string message;
            int s1;
            int s2;

            int maxRandomValue;
            if(Level == DifficultyLevel.EASY)
            {
                maxRandomValue = 10;
            }
            else if(Level == DifficultyLevel.MEDIUM)
            {
                maxRandomValue = 25;
            }
            else if (Level == DifficultyLevel.HARD)
            {
                maxRandomValue = 50;
            }
            else
            {
                maxRandomValue = 100;
            }

            s1 = random.Next(1, maxRandomValue);
            s2 = random.Next(1, maxRandomValue);

            if (s2 > s1)
            {
                (s2, s1) = (s1, s2);
            }

            int number = random.Next(Constants.DefaultMaxRandomValue);
            int maxValueDivided = Constants.DefaultMaxRandomValue / 5;
            if (number < maxValueDivided) //ADDITION
            {
                returnAnswer = s1 + s2;
                message = $"{s1}+{s2}=";
            }
            else if (number < maxValueDivided * 2)//SUBTRACTION
            {
                
                returnAnswer = s1 - s2;
                message = $"{s1}-{s2}=";
            }
            else if (number < maxValueDivided * 3)//MULTIPLICATION
            {
                returnAnswer = s1 * s2;
                message = $"{s1}*{s2}=";
            }
            else if (number < maxValueDivided * 4)//DIVISION
            {
                returnAnswer = s1 / s2;
                message = $"{s1}/{s2}=";
            }
            else//MODULO
            {
                returnAnswer = s1 % s2;
                message = $"{s1}%{s2}=";
            }

            return (returnAnswer, message);
        }

    }

    public enum DifficultyLevel
    {
        EASY,
        MEDIUM,
        HARD,
        ULTRA
    }

    public enum EquationType
    {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION,
        MODULO
    }
}
