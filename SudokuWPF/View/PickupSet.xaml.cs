using System;
using System.Windows;
using SudokuWPF.ViewModel;

namespace SudokuWPF.View
{
    /// <summary>
    /// Interaction logic for PickupSet.xaml
    /// </summary>
    public partial class PickupSet : Window
    {
        readonly ViewModelClass _vm = null;
        public PickupSet(ViewModelClass vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string setNumber = txtSet.Text.Trim();
                if (string.IsNullOrWhiteSpace(setNumber) == false)
                {
                    //TODO:Check the errors.
                    setNumber = setNumber.PadLeft(2, '0');
                    _vm.GameSetClicked(setNumber);
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
