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

using SudokuWPF.View;
using SudokuWPF.ViewModel;

namespace SudokuWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void ApplicationStartup(object sender, StartupEventArgs args)
        {
            MainWindow mainWindow = new MainWindow();                           // Instantiate the main window
            mainWindow.ViewModel = ViewModelClass.GetInstance(mainWindow);      // Get an instance of the ViewModel and set the View's ViewModel pointer
            mainWindow.Show();                                                  // Now display the view
        }
    }
}
