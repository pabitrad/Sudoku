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
using System.Collections.Generic;
using System.Diagnostics;

using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel;

namespace SudokuWPF.Model
{
    internal class GameModel
    {
        #region . Variables .

        private CellClass[,] _cells;                            // Array of cells for the puzzle that is playing.
        private List<CellClass>[] _regionList;                  // Array of cells arranged by region.
        private Stack<CellClass[,]> _undoStack; 

        #endregion

        #region . Constructors .

        /// <summary>
        /// Initialized a new instance of the GameModel class.
        /// </summary>
        /// <param name="cells">Array of cells to initialize this class with.</param>
        internal GameModel(CellClass[,] cells)
        {
            InitClass(cells);                                   // Call the initialization routine.
        }

        #endregion

        #region . Properties .

        #region . Properties: Public Read-only .

        /// <summary>
        /// Indexer for the GameModel class.
        /// </summary>
        /// <param name="col">Column of cell to return.</param>
        /// <param name="row">Row of cell to return.</param>
        /// <returns>Returns the CellClass member at the specified column and row.</returns>
        internal CellClass this[Int32 col, Int32 row]
        {
            get
            {
                if ((_cells != null) && (Common.IsValidIndex(col, row)))    // If we have cells and the inputs are valid.
                    return _cells[col, row];                                // Then return the specified cell.
                return null;                                                // Otherwise, return null.
            }
        }
        /// <summary>
        /// Gets a flag indicating whether or not the puzzle is complete.
        /// </summary>
        internal bool GameComplete
        {
            get
            {
                return (EmptyCount == 0);                                   // If there are no more empty cells, then the puzzle is done.
            }
        }
        /// <summary>
        /// Gets a list of cells of the puzzle that is currently playing.
        /// </summary>
        internal List<CellClass> CellList { get; private set; }

        #endregion

        #region . Properties: Public Read/Write .

        /// <summary>
        /// Gets or sets the number of empty cells left in the puzzle.
        /// </summary>
        internal Int32 EmptyCount
        {
            get
            {
                return CellList != null
                    ? CellList.FindAll(cell => cell.CellState == CellStateEnum.Blank).Count
                    : Common.BorderSide * Common.BorderSide;
            }
        }

        #endregion

        #endregion

        #region . Methods .

        #region . Methods: Public .

        internal void PrepareToNextMove()
        {
            if (_cells == null)
            {
                return;
            }
            int cellsLengthX = _cells.GetLength(0);
            int cellsLengthY = _cells.GetLength(1);
            var backupCellsState = new CellClass[cellsLengthX, cellsLengthY];
            for (int i = 0; i < cellsLengthX; i++)
            {
                for (int j = 0; j < cellsLengthY; j++)
                {
                    backupCellsState[i, j] = _cells[i, j].Clone();
                }
            }
            _undoStack.Push(backupCellsState);
        }

        internal void UndoToPrevMove()
        {
            if (_cells == null || _undoStack.Count == 0)
            {
                return;
            }
            var prevMoveCells = _undoStack.Pop();
            int cellsLengthX = _cells.GetLength(0);
            int cellsLengthY = _cells.GetLength(1);
            for (int i = 0; i < cellsLengthX; i++)
            {
                for (int j = 0; j < cellsLengthY; j++)
                {
                    _cells[i, j] = prevMoveCells[i, j];
                }
            }
            InitRegionList();
            ConvertToList();
        }

        /// <summary>
        /// Compute the notes at the specified column and row.
        /// </summary>
        /// <param name="col">Column of the cell to compute the notes for.</param>
        /// <param name="row">Row of the cell to compute the notes for.</param>
        internal void ComputeNote(Int32 col, Int32 row)
        {
            if (Common.IsValidIndex(col, row))              // Are the inputs valid?
                GenerateNote(_cells[col, row]);             // Yes, then generate the note.
        }

        /// <summary>
        /// Reset the puzzle state.
        /// </summary>
        internal void ResetBoard()
        {
            _undoStack.Clear();
            if (_cells != null)                                         // Do we have a game stored?
            {                                                           // Yes.
                for (Int32 i = 0; i < CellList.Count; i++)            // Loop through the cells in the puzzle.
                {
                    CellList[i].CellState = CellStateEnum.Blank;    // Then reset the state to Blank.
                    CellList[i].UserAnswer = 0;                     // Clear out the user's answer.
                }
            }
        }

        /// <summary>
        /// Reset the puzzle state.
        /// </summary>
        internal void ResetPuzzle()
        {
            _undoStack.Clear();
            if (_cells != null)                                         // Do we have a game stored?
            {                                                           // Yes.
                for (Int32 i = 0; i < CellList.Count; i++)              // Loop through the cells in the puzzle.
                    if (CellList[i].CellState != CellStateEnum.Answer)  // Is the state the answer?
                    {                                                   // No.
                        CellList[i].CellState = CellStateEnum.Blank;    // Then reset the state to Blank.
                        CellList[i].UserAnswer = 0;                     // Clear out the user's answer.
                    }
            }
        }

        /// <summary>
        /// Display the notes for all the cells that are blank.
        /// </summary>
        internal void ShowNotes()
        {
            if (_cells != null)                                         // Do we have a game saved?
                for (Int32 i = 0; i < CellList.Count; i++)              // Yes, then loop through the cells.
                    if (CellList[i].CellState == CellStateEnum.Blank)   // Is the state blank?
                        CellList[i].CellState = CellStateEnum.Notes;    // Yes, the change the state to notes.
        }

        /// <summary>
        /// Hide the notes for all the cells.
        /// </summary>
        internal void HideNotes()
        {
            if (_cells != null)                                         // Do we have a game saved?
                for (Int32 i = 0; i < CellList.Count; i++)              // Yes, then loop through the cells.
                    if (CellList[i].CellState == CellStateEnum.Notes)   // Are the notes being displayed?
                        CellList[i].CellState = CellStateEnum.Blank;    // Yes, then change the state to blank.
        }

        /// <summary>
        /// Show the solution.
        /// </summary>
        internal void ShowSolution()
        {
            if (_cells != null)                                         // Do we have a game saved?
                for (Int32 i = 0; i < CellList.Count; i++)              // Yes, then loop through the cells.
                    if ((CellList[i].CellState != CellStateEnum.Answer) && (CellList[i].CellState != CellStateEnum.UserInputCorrect))
                        CellList[i].CellState = CellStateEnum.Hint;     // If the state is not Answer or Correct, then show the hint.
        }

        /// <summary>
        /// Hide the solution.
        /// </summary>
        /// <param name="showAllNotes"></param>
        internal void HideSolution(bool showAllNotes)
        {
            if (_cells != null)                                             // Do we have a game saved?
                for (Int32 i = 0; i < CellList.Count; i++)                  // Yes, then loop through the cells
                    if (CellList[i].CellState == CellStateEnum.Hint)        // If the state is hint,
                        if (showAllNotes)                                   // Then check if the Show Notes flag is set
                            CellList[i].CellState = CellStateEnum.Notes;    // If it is, then change the state to Notes
                        else
                            CellList[i].CellState = CellStateEnum.Blank;    // Otherwise change it to Blank
        }

        /// <summary>
        /// Returns a list of cells in the given region.
        /// </summary>
        /// <param name="index">Zero based index of the region.</param>
        /// <returns>A list off CellClass objects that belong to the specified region.</returns>
        internal List<CellClass> RegionCells(Int32 index)
        {
            if (Common.IsValidIndex(index) && (_regionList != null))    // If the input is valid and the region list is not null.
                return _regionList[index];                              // Return the region requested.
            return null;                                                // Otherwise, return null.
        }

        #endregion

        #region . Methods: Private .

        private void InitClass(CellClass[,] cells)
        {
            if (cells != null)                                      // If the input parameter is not null
            {
                _cells = cells;                                     // Save it.
                InitRegionList();                                   // Initialize the region list.
                ConvertToList();                                    // Convert the 2D array to a list.
                GenerateAllNotes();                                 // Generate all notes for blank cells.
                _undoStack = new Stack<CellClass[,]>();
            }
        }

        private void InitRegionList()
        {
            _regionList = new List<CellClass>[9];                   // Initialize the array with 9 elements.
            for (Int32 i = 0; i < 9; i++)                           // Loop through the array.
                _regionList[i] = new List<CellClass>();             // Initialize each element with a list.
        }

        private void ConvertToList()
        {
            CellList = new List<CellClass>(_cells.Length);          // Initialize the list.
            for (Int32 col = 0; col < 9; col++)                     // Loop through the columns
                for (Int32 row = 0; row < 9; row++)                 // Loop through the rows
                    AddCell(_cells[col, row]);                      // Add each cell to the lists
        }

        private void AddCell(CellClass cell)
        {
            if (cell != null)                                       // Is input a valid object?
            {                                                       // Yes.
                CellList.Add(cell);                                 // Add the cell to the main list.
                _regionList[cell.Region].Add(cell);                 // Add the cell the corresponding region list.
            }
            else
                throw new Exception("Cell cannot be null.");        // TODO: Maybe add this to the event log instead?
        }

        private void GenerateAllNotes()
        {
            for (Int32 i = 0; i < CellList.Count; i++)              // Loop through the list of cells
                GenerateNote(CellList[i]);                          // Generate a note for each cell
        }

        private void GenerateNote(CellClass cell)
        {
            if (cell.CellState != CellStateEnum.Answer)                 // Is the cell state Answer?
            {                                                           // No, the figure out the notes
                for (Int32 i = 0; i < 9; i++)                           // Loop through the notes array
                    cell.Notes[i].State = true;                         // Default the state to True
                for (Int32 i = 0; i < 9; i++)                           // Loop through rows and columns that intersect the given cell
                {
                    ProcessNote(cell, _cells[cell.Col, i]);             // Check the column the cell is in.
                    ProcessNote(cell, _cells[i, cell.Row]);             // Check the row the cell is in.
                }
                foreach (CellClass item in _regionList[cell.Region])    // Now loop through the region the cell belongs to.
                    ProcessNote(cell, item);
            }
        }

        private void ProcessNote(CellClass targetCell, CellClass sourceCell)
        {
            if (sourceCell.CellState == CellStateEnum.Answer)           // Is the cell state Answer?
                targetCell.Notes[sourceCell.Answer - 1].State = false;  // Yes, then turn off the note.
        }

        #endregion

        #endregion
    }
}
