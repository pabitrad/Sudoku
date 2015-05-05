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

namespace SudokuWPF.View
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        #region . Constructors .

        /// <summary>
        /// Initializes a new instance of the Aboutbox form.
        /// </summary>
        public AboutBox()
        {
            InitializeComponent();
        }

        #endregion

        #region . Form Event Handlers .

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();                           // OK button clicked.  Close this window.
        }

        #endregion
    }
}
