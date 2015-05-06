using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SudokuWPF.ViewModel;
using SudokuWPF.Model.Enums;

namespace SudokuWPF.View
{
    /// <summary>
    /// Interaction logic for PickupSet.xaml
    /// </summary>
    public partial class PickupSet : Window
    {
        ViewModelClass _vm = null;
        GameSetDifficulty _levelDifficulty;
        public PickupSet(ViewModelClass vm, GameSetDifficulty levelDifficulty)
        {
            InitializeComponent();
            _vm = vm;
            _levelDifficulty = levelDifficulty;
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
