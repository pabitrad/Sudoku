//
// Copyright (c) 2014 Han Hung
// 
// This program is free software; it is distributed under the terms
// of the GNU General Public License v3 as published by the Free
// Software Foundation.
//
// http://www.gnu.org/licenses/gpl-3.0.html
// 

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SudokuWPF.Model;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel.CustomEventArgs;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class GamesManager
    {
        #region . Variables, Constants, And other Declarations .

        #region . Variables .
        
        private GameCollection[] _games = new GameCollection[Common.MaxLevels];
        GameGenerator _cGameGenerator = null; 
        #endregion

        #region . Other Declarations .

        internal event EventHandler<GameManagerEventArgs> GamesManagerEvent;

        #endregion

        #endregion

        #region . Constructors .

        /// <summary>
        /// Initializes a new instance of the GamesManager class.
        /// </summary>
        internal GamesManager()
        {
            InitializeClass();
        }

        #endregion

        #region . Event Handlers .

        private void GameManagerEventHandler(object sender, GameManagerEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        #region . Methods .

        #region . Methods: Public .

        /// <summary>
        /// Stops the game manager thread.
        /// </summary>
        internal void StopGamesManager()
        {
            StopBackgroundTasks();              // Stop all background threads.
            SaveGames();                        // Save the games to the application config file.
        }

        /// <summary>
        /// Gets a game based on the specified difficulty level.
        /// </summary>
        /// <param name="level">Level of difficulty specified.</param>
        /// <returns>A 2D CellClass array of the game.</returns>
        internal CellClass[,] GetGame(DifficultyLevels level)
        {
            return _games[(int)level].GetGame;          // Get a game based on the specified difficulty level
        }

        /// <summary>
        /// Gets a game based on the specified difficulty level.
        /// </summary>
        /// <param name="level">Level of difficulty specified.</param>
        /// <param name="setNumberString">Number of concrete set within specified difficulty level.</param>
        /// <returns>A 2D CellClass array of the game.</returns>
        internal CellClass[,] GetGame(GameSetDifficulty level, string setNumberString)
        {
            string gameSetFileName = string.Format(Properties.Settings.Default.SetFileNameFormat, setNumberString);
            string gameSetFilePath = Properties.Settings.Default.DatabaseDirectory.EnsureSlash() +
                                     DataConst.GameSetFolderName + "\\" + level + "\\" + gameSetFileName;
            string gamSetAnswersFileName = string.Format(Properties.Settings.Default.SetAnswersFileNameFormat, setNumberString);
            string gameSetAnswersFilePath = Properties.Settings.Default.DatabaseDirectory.EnsureSlash() +
                                            DataConst.AnswerSetFolderName + "\\" + level + "\\" + gamSetAnswersFileName;

            string setSourceString = File.ReadAllText(gameSetFilePath);
            setSourceString = setSourceString.Replace("\r\n", " ");
            var numberStrings = setSourceString.Split(' ');

            string setAnswerSourceString = File.ReadAllText(gameSetAnswersFilePath);
            setAnswerSourceString = setAnswerSourceString.Replace("\r\n", " ");
            var numberAnswerStrings = setAnswerSourceString.Split(' ');

            if (numberStrings.Length != numberAnswerStrings.Length)
            {
                throw new InvalidOperationException("Game set and Answer set numbers count not match.");
            }

            CellClass[,] cellsArray = new CellClass[Common.BorderSide, Common.BorderSide];
            for (int index = 0; index < numberStrings.Length; index++)
            {
                var numberStr = numberStrings[index];
                int number;
                bool isNumberParsed = int.TryParse(numberStr, out number);
                if (!isNumberParsed)
                {
                    throw new InvalidOperationException(string.Format("Cannot parse number {0}", numberStr));
                }

                var numberAnswerStr = numberAnswerStrings[index];
                int numberAnswer;
                bool isNumberAnswerParsed = int.TryParse(numberAnswerStr, out numberAnswer);
                if (!isNumberAnswerParsed)
                {
                    throw new InvalidOperationException(string.Format("Cannot parse number answer {0}", numberAnswerStr));
                }

                if (number != 0 && number != numberAnswer)
                {
                    throw new InvalidOperationException(string.Format("Game set number {0} not equals Answer {1}", number, numberAnswer));
                }

                var cell = new CellClass(index, numberAnswer)
                {
                    CellState = number != 0 ? CellStateEnum.Answer : CellStateEnum.Blank
                };
                cellsArray[cell.Col, cell.Row] = cell;
            }
            return cellsArray;
        }

        internal bool IsAlreadyPlayedLevel(string playerName, GameSetDifficulty difficultyLevel)
        {
            bool isLevelPlayed;
            int setPlayedCount;
            CheckAlreadyPlayedLevelAndSet(playerName, difficultyLevel, "1", out isLevelPlayed, out setPlayedCount);
            return isLevelPlayed;
        }

        internal void CheckAlreadyPlayedLevelAndSet(
             string playerName, GameSetDifficulty difficultyLevel, string setNumber, out bool isLevelPlayed, out int gameSetPlayedCount)
        {
            EnsureDataFile();
            string dataFilePath = Properties.Settings.Default.DatabaseDirectory.EnsureSlash() + DataConst.DataFileName;
            var players = XDocument
                .Load(dataFilePath)
                .Element(DataConst.XmlSudokuElement)
                .Elements(DataConst.XmlPlayerElement);
            var specifiedPlayerElement = players.FirstOrDefault(
                player => player.Attribute(DataConst.XmlPlayerNameAttr).Value == playerName);
            if (specifiedPlayerElement == null)
            {
                isLevelPlayed = false;
                gameSetPlayedCount = 0;
                return;
            }
            var playedLevels = specifiedPlayerElement.Elements(DataConst.XmlDifficultyLevelElement);
            var specifiedLevelElement = playedLevels.FirstOrDefault(
                level => level.Attribute(DataConst.XmlDifficultyLevelNameAttr).Value == difficultyLevel.ToString());
            if (specifiedLevelElement == null)
            {
                isLevelPlayed = false;
                gameSetPlayedCount = 0;
                return;
            }
            var gameSets = specifiedLevelElement.Elements(DataConst.XmlGameSetElement);
            var specifiedSetElement = gameSets.FirstOrDefault(
                set => int.Parse(set.Attribute(DataConst.XmlGameSetNumberAttr).Value) == int.Parse(setNumber));
            if (specifiedSetElement == null)
            {
                isLevelPlayed = true;
                gameSetPlayedCount = 0;
                return;
            }
            isLevelPlayed = true;
            gameSetPlayedCount = int.Parse(specifiedSetElement.Attribute(DataConst.XmlGameSetPlayedCountAttr).Value);
        }

        internal void IncrementGameSetPlayedCount(string playerName, GameSetDifficulty difficultyLevel, string setNumber)
        {
            EnsureDataFile();
            string dataFilePath = Properties.Settings.Default.DatabaseDirectory.EnsureSlash() + DataConst.DataFileName;
            var dataBaseDoc = XDocument.Load(dataFilePath);
            var playerElement = dataBaseDoc.Root.Elements(DataConst.XmlPlayerElement)
                .FirstOrDefault(element => element.Attribute(DataConst.XmlPlayerNameAttr).Value == playerName);
            if (playerElement == null)
            {
                playerElement = new XElement(DataConst.XmlPlayerElement, new XAttribute(DataConst.XmlPlayerNameAttr, playerName));
                dataBaseDoc.Root.Add(playerElement);
            }
            string specifiedLevel = difficultyLevel.ToString();
            var level = playerElement.Elements(DataConst.XmlDifficultyLevelElement)
                .FirstOrDefault(element => element.Attribute(DataConst.XmlDifficultyLevelNameAttr).Value == specifiedLevel);
            if (level == null)
            {
                level = new XElement(DataConst.XmlDifficultyLevelElement, new XAttribute(DataConst.XmlDifficultyLevelNameAttr, specifiedLevel));
                playerElement.Add(level);
            }
            int setNumberInt = int.Parse(setNumber);
            var gameSet = level.Elements(DataConst.XmlGameSetElement)
                .FirstOrDefault(element => int.Parse(element.Attribute(DataConst.XmlGameSetNumberAttr).Value) == setNumberInt);
            if (gameSet == null)
            {
                gameSet = new XElement(DataConst.XmlGameSetElement,
                    new XAttribute(DataConst.XmlGameSetNumberAttr, setNumberInt),
                    new XAttribute(DataConst.XmlGameSetPlayedCountAttr, 0));
                level.Add(gameSet);
            }
            int currentPlayedCount = int.Parse(gameSet.Attribute(DataConst.XmlGameSetPlayedCountAttr).Value);
            gameSet.SetAttributeValue(DataConst.XmlGameSetPlayedCountAttr, ++currentPlayedCount);
            dataBaseDoc.Save(dataFilePath);
        }

        private void EnsureDataFile()
        {
            string dataFilePath = Properties.Settings.Default.DatabaseDirectory.EnsureSlash() + "\\" + DataConst.DataFileName;
            if (!File.Exists(dataFilePath))
            {
                XDocument doc = new XDocument(new XElement(DataConst.XmlSudokuElement));
                doc.Save(dataFilePath);
            }
        }

        /// <summary>
        /// Gets the number of games in the game queue of the specified level.
        /// </summary>
        /// <param name="level">Difficulty level of the count needed.</param>
        /// <returns>The number of games in the queue.</returns>
        internal Int32 GameCount(DifficultyLevels level)
        {
            return _games[(int)level].GameCount;
        }

        #endregion

        #region . Methods: Private .

        private void InitializeClass()
        {
            InitGameCollectionArray();                              // Initialize the game collection array
            LoadGames();                                            // Load any saved games from the config file
            StartBackgroundTasks();                                 // Start the background tasks
        }

        private void InitGameCollectionArray()
        {
            foreach (Int32 i in Enum.GetValues(typeof(DifficultyLevels)))   // Loop through the enum
                _games[i] = InitGameCollection((DifficultyLevels)i);        // For each level, initialize a game manager class
        }

        private GameCollection InitGameCollection(DifficultyLevels level)
        {
            GameCollection collection = new GameCollection(level);      // Instantiate a new game collection class
            collection.GameManagerEvent += GameManagerEventHandler;     // Set the event handler
            return collection;                                          // Return the pointer
        }

        private void LoadGames()
        {
            _games[0].LoadGames(Properties.Settings.Default.GamesLevel0);       // Load games from the config file
            _games[1].LoadGames(Properties.Settings.Default.GamesLevel1);
            _games[2].LoadGames(Properties.Settings.Default.GamesLevel2);
            _games[3].LoadGames(Properties.Settings.Default.GamesLevel3);
            _games[4].LoadGames(Properties.Settings.Default.GamesLevel4);
        }

        private void StartBackgroundTasks()
        {
            foreach (GameCollection item in _games)                     // Loop though the array
                item.StartThread();                                     // Start each background thread
        }

        private void StopBackgroundTasks()
        {
            foreach (GameCollection item in _games)                     // Loop through the array
                item.StopThread();                                      // Stop each background thread
        }

        private void SaveGames()
        {
            Properties.Settings.Default.GamesLevel0 = _games[0].SaveGames();    // Save the games to the application config file
            Properties.Settings.Default.GamesLevel1 = _games[1].SaveGames();
            Properties.Settings.Default.GamesLevel2 = _games[2].SaveGames();
            Properties.Settings.Default.GamesLevel3 = _games[3].SaveGames();
            Properties.Settings.Default.GamesLevel4 = _games[4].SaveGames();
            Properties.Settings.Default.Save();                                 // Now save it to disk
        }

        private void RaiseEvent(GameManagerEventArgs e)
        {
            EventHandler<GameManagerEventArgs> handler = GamesManagerEvent;     // Get a pointer to the event handler
            if (handler != null)                                                // Any listeners?
                handler(this, e);                                               // Yes, then raise the event
        }

        #endregion

        #endregion
    }
}
