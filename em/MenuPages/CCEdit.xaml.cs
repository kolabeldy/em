using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace em.MenuPages
{
    /// <summary>
    /// Логика взаимодействия для CostCentersEdit.xaml
    /// </summary>
    public partial class CCEdit : Window
    {

        private CCEditModel model;

        public CCEdit()
        {
            model = new CCEditModel();
            DataContext = model;
            InitializeComponent();
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
