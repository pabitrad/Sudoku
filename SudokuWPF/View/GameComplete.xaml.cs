//
// Copyright (c) 2014 Han Hung
// 
// This program is free software; it is distributed under the terms
// of the GNU General Public License v3 as published by the Free
// Software Foundation.
//
// http://www.gnu.org/licenses/gpl-3.0.html
// 

using System.Windows;

using SudokuWPF.ViewModel;

namespace SudokuWPF.View
{
    /// <summary>
    /// Interaction logic for GameComplete.xaml
    /// </summary>
    public partial class GameComplete : Window
    {
        #region . Constructors .

        /// <summary>
        /// Initializes a new instance of the GameComplete window.
        /// </summary>
        /// <param name="message">Message to display in this form.</param>
        public GameComplete(ViewModelClass vm)
        {
            InitializeComponent();

            this.DataContext = vm;          // Set the data context for this window
        }

        #endregion

        #region . Form Event Handlers .

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();               // OK button pressed.  Close the window.
        }

        #endregion
    }
}
