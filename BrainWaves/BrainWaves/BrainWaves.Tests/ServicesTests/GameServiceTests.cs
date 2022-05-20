using BrainWaves.Services;
using FluentAssertions;
using NUnit.Framework;

namespace BrainWaves.Tests.ServicesTests
{
    public class GameServiceTests
    {
        GameService gameService;
        [SetUp]
        public void Setup()
        {
            gameService = new GameService();
        }

        [Test]
        public void GenerateExercise_ReturnsNotNullAnswer()
        {
            (int answer, _) = gameService.GenerateExercise();

            answer.Should().NotBe(null);
        }

        [Test]
        public void GenerateExercise_ReturnsNaturalNumberAnswer()
        {
            (int answer, _) = gameService.GenerateExercise();

            answer.Should().BeInRange(0,5000);
        }

        [Test]
        public void GenerateExercise_ReturnsNotNullEquation()
        {
            (_, string equation) = gameService.GenerateExercise();

            equation.Should().NotBe(null);
        }

        [Test]
        public void GenerateExercise_ReturnsNotEmptyAnswer()
        {
            (_, string equation) = gameService.GenerateExercise();

            equation.Should().NotBe(string.Empty);
        }
    }
}
