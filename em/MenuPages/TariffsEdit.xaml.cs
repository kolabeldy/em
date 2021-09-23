using System.Windows;
using System.Windows.Controls;

namespace em.MenuPages
{
    /// <summary>
    /// Логика взаимодействия для PageTariffsEdit.xaml
    /// </summary>
    public partial class TariffsEdit : Window
    {
        TariffsEditModel model;
        public TariffsEdit()
        {
            model = new TariffsEditModel();
            DataContext = model;
            InitializeComponent();
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
