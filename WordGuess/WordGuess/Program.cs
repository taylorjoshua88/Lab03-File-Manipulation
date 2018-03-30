using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WordGuess
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Project folder is three directories up from the build path
            const string wordbankPath = @"..\..\..\wordbank.txt";
            bool alive = true;

            while (alive)
            {
                Console.Clear();

                switch (PromptHomeMenu())
                {
                    case 1:
                        // Keep calling StartNewGame until the user no longer wants
                        // to play new games (when StartNewGame returns false)
                        while (StartNewGame(wordbankPath)) { }
                        break;
                    case 2:
                        Console.Clear();
                        ViewWordBank(wordbankPath);
                        Console.WriteLine("\nPlease press any key to continue...");
                        Console.ReadKey();
                        break;
                    case 3:
                        Console.Clear();
                        PromptAddToWordBank(wordbankPath);
                        break;
                    case 4:
                        Console.Clear();
                        PromptRemoveWordFromWordBank(wordbankPath);
                        break;
                    case 5:
                        alive = false;
                        break;
                    default:
                        throw new ApplicationException("Program has entered into an invalid" +
                            " state from PromptHomeMenu().");
                }
            }
        }

        /// <summary>
        /// Displays the home menu with a list of actions the user can take and validates
        /// that the input corresponds to an available option 1-5. Returns the user's selection
        /// as an integer.
        /// </summary>
        /// <returns>An integer 1-5 representing the user's menu selection</returns>
        static int PromptHomeMenu()
        {
            int choice = 0;

            for (;;)
            {
                Console.WriteLine("Welcome to WordGuess, the word guessing game!");
                Console.WriteLine("Please choose an option from below and press enter:");
                Console.WriteLine("1) New Game");
                Console.WriteLine("2) View Word Bank");
                Console.WriteLine("3) Add to the Word Bank");
                Console.WriteLine("4) Remove Words From the Word Bank");
                Console.WriteLine("5) Exit the Game");

                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    // Any FormatException or OverflowException should be treated as an
                    // invalid option and will flow to the error message displayed below.
                    choice = 0;
                }

                if (choice >= 1 && choice <= 5)
                {
                    return choice;
                }

                // Could not understand input or a nonexistent option was selected.
                // Prompt the user to try again.
                Console.Clear();
                Console.WriteLine("Invalid choice provided.");
                Console.WriteLine("Please type just the number corresponding to" +
                    " your choice followed by enter.\n");
            }
        }

        /// <summary>
        /// Takes control of the calling thread to begin the main game loop on a
        /// randomly selected word from the file located at wordbankPath 
        /// </summary>
        /// <returns>Boolean stating whether or not to start another new game.
        /// True = start another new game. False = Continue to the home menu</returns>
        static bool StartNewGame(string wordbankPath)
        {
            string mysteryWord = LoadRandomWordFromFile(wordbankPath);
            string guessedLetters = "";
            int correctGuesses = 0;

            while (correctGuesses < mysteryWord.Length)
            {
                Console.Clear();
                Console.WriteLine("Word to guess:");
                Console.WriteLine(CreatePartialWord(
                    mysteryWord.ToUpper(), guessedLetters.ToUpper()));
                Console.WriteLine("\nYou have guessed the following letters:");
                Console.WriteLine(guessedLetters);
                Console.WriteLine("\nPlease enter up to two letters to guess or '/'");
                Console.WriteLine(" to give up and return to the home menu.");

                string userInput = Console.ReadLine();

                if (userInput.Contains("/"))
                {
                    Console.WriteLine($"\nThe mystery word was {mysteryWord}.");
                    Console.WriteLine("Please press any key to continue...");
                    Console.ReadKey(true);
                    return false;
                }

                if (userInput.Length > 0)
                {
                    if (Regex.Matches(guessedLetters, $"[{userInput}]").Count > 0)
                    {
                        Console.WriteLine("You have already guessed one of those letters!");
                        Console.WriteLine("Please press any key to try again...");
                        Console.ReadKey(true);
                    }
                    else
                    {
                        correctGuesses += CheckGuesses(mysteryWord, userInput);
                        guessedLetters += userInput;
                    }
                }
            }

            Console.WriteLine($"Congratulations! You've guessed {mysteryWord}!");
            Console.WriteLine("Would you like to start a new game? (Y/N)");

            // Keep checking for user input until either 'Y' or 'N' is pressed
            ConsoleKey newGameChoice;
            do
            {
                newGameChoice = Console.ReadKey(true).Key;
            } while (newGameChoice != ConsoleKey.Y && newGameChoice != ConsoleKey.N);

            return newGameChoice == ConsoleKey.Y;
        }

        /// <summary>
        /// Displays the contents of the wordbank to the console
        /// </summary>
        /// <param name="wordbankPath">Path to the wordbank file</param>
        static void ViewWordBank(string wordbankPath)
        {
            Console.WriteLine($"Current word bank from {wordbankPath} separated by commas:\n");
            Console.WriteLine(LoadWordbankText(wordbankPath));
        }

        /// <summary>
        /// Prompt the user to add a new word to the word bank. Appends the new word
        /// to the wordbank file if the new file doesn't already exist
        /// </summary>
        /// <param name="wordbankPath">Path to the word bank</param>
        static void PromptAddToWordBank(string wordbankPath)
        {
            ViewWordBank(wordbankPath);

            Console.WriteLine("\nPlease type a new word to add to the list or '/' to cancel...");

            string newWord = Console.ReadLine().Trim();

            if (newWord.Contains("/"))
            {
                return;
            }

            // Make sure redundant entries aren't added to the wordbank
            if (LoadWordbankText(wordbankPath).ToUpper().Contains(newWord.ToUpper())) {
                Console.WriteLine("Inputted word already contained in wordbank!");
                Console.WriteLine("Please press any key to return to home menu...");
                Console.ReadKey();
                return;
            }

            // Commit the word to the wordbank
            try
            {
                using (StreamWriter sw = File.AppendText(wordbankPath))
                {
                    sw.Write($",{newWord}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write the new word to the wordbank!");
                Console.WriteLine(e.Message);
                throw;
            }

            Console.WriteLine($"\nSuccessfully added {newWord} to the wordbank!");
            Console.WriteLine("Please press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Prompt the user to remove a word from the word bank and then overwrite the wordbank
        /// file with the result of removing that word upon success
        /// </summary>
        /// <param name="wordbankPath">Path to the wordbank</param>
        static void PromptRemoveWordFromWordBank(string wordbankPath)
        {
            ViewWordBank(wordbankPath);

            Console.WriteLine("\nPlease type a word to remove from the list or '/' to cancel...");

            string deleteWord = Console.ReadLine().Trim();

            if (deleteWord.Contains("/"))
            {
                return;
            }

            // Load the existing wordbank text and try to find the word to delete using regex
            string wordbankText = LoadWordbankText(wordbankPath);
            Match deleteMatch = Regex.Match(wordbankText, @"\b" + deleteWord + @"\b,?",
                RegexOptions.IgnoreCase);

            if (!deleteMatch.Success)
            {
                Console.WriteLine($"{deleteWord} was not found in the wordbank!");
                Console.WriteLine("Please press any key to return to the home menu...");
                Console.ReadKey();
                return;
            }

            // Remove the deleted word from the wordbank text in memory and make sure
            // there are no trailing commas (if the last word in the wordbank was chosen)
            wordbankText = wordbankText.Remove(
                deleteMatch.Index, deleteMatch.Length).TrimEnd(',');

            // Overwrite the wordbank on disk with our new version
            try
            {
                using (StreamWriter fs = File.CreateText(wordbankPath))
                {
                    fs.Write(wordbankText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to delete the word from the wordbank file!");
                Console.WriteLine(e.Message);
                throw;
            }

            Console.WriteLine("\nSuccessfully removed word from the wordbank!");
            Console.WriteLine("Please press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Takes in a comma-separated text string and outputs an array
        /// of substrings shifted to upper case.
        /// </summary>
        /// <param name="wordbankText">A string with comma separated words
        /// to be used in the game</param>
        /// <returns>A string array of substrings from
        /// <paramref name="wordbankText"/> shifted to upper case</returns>
        public static string[] ParseWordbankText(string wordbankText)
        {
            return Regex.Split(wordbankText.Trim().ToUpper(), @"\s*,\s*");
        }

        /// <summary>
        /// Loads all of the text from the specified wordbank file
        /// </summary>
        /// <param name="path">The path to the wordbank file</param>
        /// <returns>A string containing all of the contents of the wordbank
        /// file</returns>
        public static string LoadWordbankText(string path)
        {
            try
            {
                // Try to read all of the text from the file located at path
                using (StreamReader sr = File.OpenText(path))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load wordbank file!");
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Loads a random word from a comma-separated wordbank file
        /// </summary>
        /// <param name="path">Path to the wordbank file</param>
        /// <param name="random">Can optionally use a specific Random object. If
        /// random is null then the default, time-based seed Random constructor is used
        /// </param>
        /// <returns>A random word as a string</returns>
        public static string LoadRandomWordFromFile(string path, Random random = null)
        {
            // If the user didn't provide their own Random object
            if (random is null)
            {
                random = new Random();
            }
            
            // Pick a random word from the wordbank array and return it
            string[] wordbank = ParseWordbankText(LoadWordbankText(path));
            return wordbank[random.Next(0, wordbank.Length)];
        }

        /// <summary>
        /// Checks a mystery word against a string of guess letters,
        /// returning the total number of times each provided letter appears.
        /// This method will not give erroneously doubled results if provided
        /// a duplicated letter in <paramref name="guess"/> such as "ee"
        /// </summary>
        /// <param name="mysteryWord">The word to guess against</param>
        /// <param name="guess">A series of letters to guess</param>
        /// <returns>A total of matches within <paramref name="mysteryWord"/>
        /// when searching for each letter in <paramref name="guess"/></returns>
        /// <exception cref="ArgumentException">More than two letters were
        /// provided for the guess, which is violation of game rules</exception>
        /// <exception cref="FormatException">The guess contained no letters
        /// </exception>
        public static int CheckGuesses(string mysteryWord, string guess)
        {
            // Ensure that the RegEx works even if the caller did not call
            // ToUpper() or trimmed whitespace
            const RegexOptions regexOptions = 
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

            MatchCollection guessLetters = Regex.Matches(guess, @"\w", regexOptions);

            // Enforce game rules (no more than two guesses per turn)
            if (guessLetters.Count > 2)
            {
                throw new ArgumentException("Attempted to guess more than" +
                    "two letters at a time. WordGuess rules allow no more" +
                    "than two letter guesses in one turn.", nameof(guess));
            }
            if (guessLetters.Count > 1)
            {
                // Using a RegEx set will prevent duplicate hits even if the user
                // provided duplicate letters such as "ee"
                return Regex.Matches(mysteryWord, 
                    $"[{guessLetters[0].Value}{guessLetters[1].Value}]",
                    regexOptions).Count;
            }
            if (guessLetters.Count > 0)
            {
                return Regex.Matches(mysteryWord, $"[{guessLetters[0].Value}]",
                    regexOptions).Count;
            }

            // Simply returning 0 if no letters were found in the guess might
            // cause the game to unfairly punish the player if they
            // accidentally provide an empty input. An exception forces this
            // to be accounted for by the game's logic.
            throw new FormatException("Could not find any letters in the" +
                " provided guesses string.");
        }

        /// <summary>
        /// Creates a string representing a partially guessed word
        /// </summary>
        /// <param name="mysteryWord">The word that is being guessed</param>
        /// <param name="guessedLetters">A list of matched or unmatched guess
        /// letters that the player has made</param>
        /// <returns>A string with the letters that have been correctly guessed
        /// and underscores representing missing letters that still need to be
        /// found by the player</returns>
        public static string CreatePartialWord(string mysteryWord,
            string guessedLetters)
        {
            StringBuilder partialWord = new StringBuilder(mysteryWord.Length);

            // Go through each letter in the mystery word. If that letter is in the
            // guesses then add it to the string. Otherwise, add an underscore to
            // indicate a missing letter.
            foreach (char mysteryLetter in mysteryWord)
            {
                partialWord.Append((guessedLetters.Contains(mysteryLetter.ToString()) ?
                    mysteryLetter : '_') + " ");
            }

            // Trim the end to eliminate the extra space at the end that is added by
            // the above loop
            return partialWord.ToString().TrimEnd();
        }
    }
}
