using System;
using System.Windows;
using SudokuWPF.Model.Enums;
using SudokuWPF.ViewModel;

namespace SudokuWPF.View
{
    public partial class PickupSet : Window
    {
        private readonly ViewModelClass _viewModel;
        private readonly GameSetDifficulty _difficultyLevel;

        public PickupSet(ViewModelClass viewModel, GameSetDifficulty difficultyLevel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            
            _viewModel = viewModel;
            _difficultyLevel = difficultyLevel;

            InitializeComponent();

            txtSet.Focus();
        }

        public string SelectedSetNumber { get; private set; }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string setNumber = txtSet.Text.Trim();
                if (!string.IsNullOrWhiteSpace(setNumber))
                {
                    //TODO:Check the errors.
                    setNumber = setNumber.PadLeft(2, '0');

                    int gameSetPlayedCount;
                    int gameSetWonCount;
                    _viewModel.GetGameSetCounters(_difficultyLevel, setNumber, out gameSetPlayedCount, out gameSetWonCount);
                    if (gameSetPlayedCount > 0 && IsUserAcceptedPlayAnotherSet(gameSetPlayedCount, gameSetWonCount))
                    {
                        return;
                    }

                    SelectedSetNumber = setNumber;
                    DialogResult = true;
                    this.Close();
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

        private bool IsUserAcceptedPlayAnotherSet(int gameSetPlayedCount, int gameSetWonCount)
        {
            string message = gameSetWonCount > 0 
                ? string.Format(
                "You have already won this set {0} number of times and attempted {1} numbers of times. " +
                "Do you want to play another set?",
                gameSetWonCount,
                gameSetPlayedCount)
                : string.Format(
                "You have already attempted this set {0} numbers of times. " +
                "Do you want to play another set?",
                gameSetPlayedCount);
            var answer = MessageBox.Show(message, "Confirm please:", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return answer == MessageBoxResult.Yes;
        } 
    }
}
