using System;
using System.Windows;
using SudokuWPF.Model.Enums;
using SudokuWPF.ViewModel;

namespace SudokuWPF.View
{
    public partial class PlayerName : Window
    {
        //private readonly ViewModelClass _viewModel;

        public PlayerName()
        {
            //if (viewModel == null)
            //{
            //    throw new ArgumentNullException("viewModel");
            //}
            
            //_viewModel = viewModel;

            InitializeComponent();
            txtPlayerName.Focus();
        }

        public string PName { get; private set; }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPlayerName.Text) == false)
                {
                    PName = txtPlayerName.Text.Trim();
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Player name can not be empty.");
                }
            }
            catch (Exception ex)
            {
                DialogResult = false;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        //private bool IsUserAcceptedPlayAnotherSet(int gameSetPlayedCount)
        //{
        //    string message = string.Format("You have already played this set {0} number of times’.  Do you want to play new set?", gameSetPlayedCount);
        //    var answer = MessageBox.Show(message, "Confirm please:", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    return answer == MessageBoxResult.Yes;
        //} 
    }
}
