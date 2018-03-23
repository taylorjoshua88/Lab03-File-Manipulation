# WordGuess

**Author**: Joshua Taylor
**Version**: 1.0.0

## Overview

WordGuess is a game which allows the user to guess the letters to a random
mystery word two at a time until all of the word's letters have been revealed.
These words are loaded from a comma-separated word bank stored as a text file
on disk. Additionally, users are able to view this word bank's contents, add
new words to the word bank, and remove words from the word bank. All changes
are saved to disk and will persist across sessions.

## Getting Started

WordGuess targets the .NET Core 2.0 platform. The .NET Core 2.0 SDK can
be downloaded from the following URL for Windows, Linux, and macOS:

https://www.microsoft.com/net/download/

The dotnet CLI utility would then be used to build and run the application:

    cd WordGuess
    dotnet build
    dotnet run

Additionally, users can build, run, and perform unit testing using Visual
Studio 2017 or greater by opening the solution file at the root of this
repository.

## Example

#### Home Menu ####
![Home Menu Screenshot](/assets/homeScreenshot.JPG)
#### Guessing Letters ####
![Guessing Letters Screenshot](/assets/guessingScreenshot.JPG)
#### Victory ####
![Victory Screenshot](/assets/victoryScreenshot.JPG)
#### Viewing the Word Bank ####
![Viewing Words Screenshot](/assets/viewingScreenshot.JPG)
#### Adding Words to Word Bank ####
![Adding Words Screenshot](/assets/addingScreenshot.JPG)
#### Removing Words to Word Bank ####
![Removing Words Screenshot](/assets/removingScreenshot.JPG)

## Architecture

WordGuess uses C# and the .NET Core 2.0 platform. Methods are provided to load
a word bank file from disk, parse comma-separated words from that word bank,
and to select random words from the word bank. All file I/O is handled through
streams in the System.IO namespace.

The gameplay UI is handled through a command line interface on the console
provided via the System namespace.

### Word Bank File Format

Word bank files are simple text files with words separated by commas. These
words are not case sensitive. Whitespace can be added between entries to ease
the human readability of the file. By default, WordGuess looks for a word
bank file at the path "..\..\..\wordbank.txt" relative to the working
directory of the executable.

### Random Selection of Words

Word bank files are parsed using commas as separators, and each word is stored
into elements of a string array. The parser can handle whitespaces between
entries added to ease human readability of the file. The resultant array is
then indexed using a random number generator.

### Data Model

WordGuess loads word bank files into memory before every session. These files
are loaded into an array of strings with one word chosen at random before
every game. Each game session tracks the letters which have already been
guessed by the player and the chosen mystery word.

### Command Line Interface (CLI)

WordGuess operates on a finite state machine representing a session of play,
viewing the word bank, adding new words to the word bank, and removing words
from the word bank. Simple prompts for user input are read and the state is
adjusted accordingly after validating correct input.

### Game Logic

WordGuess allows the user to input up to two letters at a time to guess against
the mystery word chosen at random at the start of gameplay from the word bank.
Each successful guess will reveal that letter as it appears in the mystery word
with underscores representing letters which the player has not yet guessed.
When the number of correctly guessed letters reaches the number of letters in
the mystery word, the game determines that the player has won and presents
a congratulatory message.

## Change Log

* 3.22.2018 [Joshua Taylor](mailto:taylor.joshua88@gmail.com) - Initial
release. All tests are passing.