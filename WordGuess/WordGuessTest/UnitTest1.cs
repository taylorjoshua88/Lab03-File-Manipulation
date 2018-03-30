using System;
using Xunit;
using System.IO;
using static WordGuess.Program;

namespace WordGuessTest
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("cat,dog,hat,groovy,gravy", new string[] { "CAT", "DOG", "HAT", "GROOVY", "GRAVY" })]
        [InlineData("blah, bar,tag", new string[] { "BLAH", "BAR", "TAG" })]
        [InlineData(" far , tan, Davenport,tango", new string[] { "FAR", "TAN", "DAVENPORT", "TANGO" })]
        public void ParseWordbankTextTest(string inputText, string[] expectedArray)
        {
            Assert.Equal(expectedArray, ParseWordbankText(inputText));
        }

        [Theory]
        [InlineData("DEER", "e", 2)]
        [InlineData("DEER", "E", 2)]
        [InlineData("AARDVARK", "aa", 3)]
        public void CheckGuessesTest(string mysteryWord, string guess, int expectedMatches)
        {
            Assert.Equal(expectedMatches, CheckGuesses(mysteryWord, guess));
        }

        [Theory]
        [InlineData("MONGOOSE", "MO", "M O _ _ O O _ _")]
        [InlineData("FOOTBALL", "FOBAL", "F O O _ B A L L")]
        [InlineData("DONE", "OEDN", "D O N E")]
        [InlineData("GAME", "", "_ _ _ _")]
        [InlineData("KEYBOARD", "Z", "_ _ _ _ _ _ _ _")]
        public void CreatePartialWordTest(string mysteryWord, string guessedLetters,
            string expectedOutput)
        {
            Assert.Equal(expectedOutput, CreatePartialWord(mysteryWord, guessedLetters));
        }

        [Fact]
        public void CanLoadWordbankText()
        {
            const string wordbankPath = @"..\..\..\..\WordGuess\wordbank.txt";

            string expectedText = File.ReadAllText(wordbankPath);
            string actualText = LoadWordbankText(wordbankPath);

            Assert.Equal(expectedText, actualText);
        }

        [Fact]
        public void CanLoadRandomWordFromFile()
        {
            const string wordbankPath = @"..\..\..\..\WordGuess\wordbank.txt";

            // Create two Random objects with matching seeds to ensure
            // result consistency
            Random expectedRandom = new Random(500);
            Random actualRandom = new Random(500);

            // Manually pick a "random" word from a parsed set of words
            string[] expectedWordbank = ParseWordbankText(LoadWordbankText(wordbankPath));
            string expectedWord = 
                expectedWordbank[expectedRandom.Next(0, expectedWordbank.Length)];

            // Use the LoadRandomWordFromFile method to pick a random word using another
            // random number generated with the same seed as above
            string actualWord = LoadRandomWordFromFile(wordbankPath, actualRandom);

            // Assert
            Assert.Equal(expectedWord, actualWord);
        }
    }
}
