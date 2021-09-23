using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace em.MenuPages
{
    /// <summary>
    /// Логика взаимодействия для CostCentersEdit.xaml
    /// </summary>
    public partial class NormLossEdit : Window
    {

        private NormLossEditModel model;

        public NormLossEdit()
        {
            model = new NormLossEditModel();
            DataContext = model;
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
