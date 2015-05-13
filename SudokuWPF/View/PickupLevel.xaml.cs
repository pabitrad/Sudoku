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
    /// Interaction logic for PickupLevel.xaml
    /// </summary>
    public partial class PickupLevel : Window
    {
        ViewModelClass _vm;

        public GameSetDifficulty Difficultylevel { get; private set; }

        public PickupLevel(ViewModelClass vm)
        {
            _vm = vm;
            InitializeComponent();
        }

        private void btnClickOk(object sender, RoutedEventArgs e)
        {
            var checkedButton = GridPickupLevel.Children.OfType<RadioButton>()
                                      .FirstOrDefault(r => (bool)r.IsChecked);

            string strLevel = checkedButton.Tag.ToString();
            GameSetDifficulty difficultLevel = (GameSetDifficulty)Convert.ToInt32(strLevel);
            this.Close();
            
            showPickUpSetDialogBox(difficultLevel);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void showPickUpSetDialogBox(GameSetDifficulty difficultyLevel)
        {
            PickupSet pickUpSet = null;
            try
            {
                pickUpSet = new PickupSet(_vm, difficultyLevel);
                pickUpSet.Owner = Application.Current.MainWindow;
                pickUpSet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                bool? dialogResult = pickUpSet.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    _vm.GameSetClicked(pickUpSet.SelectedSetNumber);

                    Difficultylevel = difficultyLevel;
                }
            }
            finally
            {
                pickUpSet = null;
            }
        }
    }
}
