using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace em.ServicesPages
{
    /// <summary>
    /// Логика взаимодействия для ProcessIndicator.xaml
    /// </summary>
    public partial class ProcessIndicator : Window, INotifyPropertyChanged
    {
        private static ProcessIndicator instance;
        public static ProcessIndicator GetInstance()
        {
            if (instance == null)
                instance = new ProcessIndicator();
            return instance;
        }

        private double progressMax;
        public double ProgressMax
        {
            get
            {
                return progressMax;
            }
            set
            {
                progressMax = value;
                OnPropertyChanged("ProgressMax");
            }
        }
        private double progressValue;
        public double ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }

        private ProcessIndicator()
        {
            InitializeComponent();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
