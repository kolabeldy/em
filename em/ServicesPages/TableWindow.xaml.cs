using em.Helpers;
using em.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Логика взаимодействия для TableWindow.xaml
    /// </summary>
    public partial class TableWindow : Window, INotifyPropertyChanged
    {
        public List<FullFields> GridData { get; set; }
        public List<FullFields> TotalData { get; set; }
        protected string tableTitle;
        public string TableTitle
        {
            get
            {
                return tableTitle;
            }
            set
            {
                tableTitle = value;
                OnPropertyChanged("TableTitle");
            }
        }

        private List<TableStruct> ts;
        public SolidColorBrush TotalRowBrush { get; set; }
        public bool IsHighlight { get; set; }
        private string EmptyStr { get; set; }



        private SolidColorBrush hightLightBrushPink = new SolidColorBrush(Color.FromRgb(255, 235, 235));
        private SolidColorBrush hightLightBrushGreen = new SolidColorBrush(Color.FromRgb(235, 255, 235));
        private SolidColorBrush normalBrush = new SolidColorBrush(Colors.White);

        public TableWindow(List<FullFields> tableData, List<TableStruct> tStruct, string chartCaption, bool isHighlight = false)
        {
            ts = tStruct;
            GridData = tableData;
            TotalData = RetTableTotal();

            IsHighlight = isHighlight;
            EmptyStr = "";
            DataContext = this;
            InitializeComponent();
            ChartCaption.Text = chartCaption;
            operationGrid.DataContext = GridData;
        }

        private void OperationGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            FullFields stroka = (FullFields)e.Row.DataContext;

            if (IsHighlight)
            {
                if (stroka.DiffProc > 5 || stroka.FactLossProc > stroka.NormLossProc + 1)
                {
                    e.Row.Background = hightLightBrushPink;
                }
                else if (stroka.DiffProc < -5 || stroka.FactLossProc < stroka.NormLossProc - 1)
                {
                    e.Row.Background = hightLightBrushGreen;
                }
                else
                {
                    e.Row.Background = normalBrush;
                }
            }
            else
            {
                e.Row.Background = normalBrush;
            }
        }

        private void TableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            operationGrid.ItemsSource = GridData;
            if (TotalData == null)
            {
                rowTotal.MaxHeight = 0;
            }
            totalGrid.ItemsSource = TotalData;

            var leftTextSetter = new Style(typeof(DataGridCell))
            {
                Setters =
                {
                    new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left)
                }
            };
            var TextSetterHide = new Style(typeof(DataGridCell))
            {
                Setters =
                {
                    new Setter(TextBlock.ForegroundProperty, TotalRowBrush)
                }
            };
            var rightTextSetter = new Style(typeof(DataGridCell))
            {
                Setters =
                {
                    new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right),
                }
            };
            var centerTextSetter = new Style(typeof(DataGridCell))
            {
                Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center) }
            };

            operationGrid.Columns.Clear();
            totalGrid.Columns.Clear();
            foreach (var r in ts)
            {
                if (r.ColumnVisible)
                {
                    operationGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = r.Headers,
                        Binding = new System.Windows.Data.Binding(r.Binding) { StringFormat = r.NumericFormat },
                        IsReadOnly = true,
                        Width = new DataGridLength(r.ColWidth, DataGridLengthUnitType.Star),
                        CellStyle = r.TextAligment == TextAlignment.Right ? rightTextSetter : r.TextAligment == TextAlignment.Center ? centerTextSetter : leftTextSetter,
                    });
                    totalGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = r.Headers,
                        Binding = r.TotalRowTextVisible
                            ? new System.Windows.Data.Binding(r.Binding) { StringFormat = r.NumericFormat }
                            : new System.Windows.Data.Binding("EmptyStr"),
                        IsReadOnly = true,
                        Width = new DataGridLength(r.ColWidth, DataGridLengthUnitType.Star),
                        CellStyle = r.TextAligment == TextAlignment.Right ? rightTextSetter : r.TextAligment == TextAlignment.Center ? centerTextSetter : leftTextSetter,
                    });
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private MyRelayCommand copyToClipboard_Command;
        public MyRelayCommand CopyToClipboard_Command
        {
            get
            {
                return copyToClipboard_Command ??
                    (copyToClipboard_Command = new MyRelayCommand(obj =>
                    {
                        operationGrid.SelectAllCells();
                        operationGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                        ApplicationCommands.Copy.Execute(null, operationGrid);
                        operationGrid.UnselectAllCells();
                        //MyMessageBox mesbox = new MyMessageBox("Данные таблицы успешно скопированы в буфер обмена", "Поздравляю");
                        //mesbox.ShowDialog();
                    }));
            }
        }
        private void BtnPhoto_Click(object sender, RoutedEventArgs e)
        {
            ScreenSave();
        }

        private void ScreenSave()
        {
            string destinationPath = null;
            SaveFileDialog dialog = new SaveFileDialog();
            string filename = "_Screen";

            if (dialog.ShowDialog() == true)
                destinationPath = dialog.FileName;
            else return;
            using (FileStream stream = new FileStream(string.Format("{0}.png", destinationPath + filename), FileMode.Create))
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)this.DesiredSize.Width - 42, (int)this.DesiredSize.Height - 80, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(this);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Interlace = PngInterlaceOption.On;
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(stream);
            }
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            operationGrid.SelectedIndex = -1;
        }
        public List<FullFields> RetTableTotal()
        {
            List<FullFields> rez = new List<FullFields>();
            FullFields n = new FullFields();
            n.ERName = "ИТОГО:";
            n.Plan = GridData.Sum(m => m.Plan);
            n.Fact = GridData.Sum(m => m.Fact);
            n.Diff = GridData.Sum(m => m.Diff);
            n.PlanCost = GridData.Sum(m => m.PlanCost);
            n.FactCost = GridData.Sum(m => m.FactCost);
            n.DiffCost = GridData.Sum(m => m.DiffCost);
            n.DiffProc = GridData.Sum(m => m.DiffCost) * 100 / GridData.Sum(m => m.PlanCost);
            n.dFactLimit = GridData.Sum(m => m.dFactLimit);
            n.dFactNormative = GridData.Sum(m => m.dFactNormative);
            n.dFact = GridData.Sum(m => m.dFact);
            n.dFactLimitCost = GridData.Sum(m => m.dFactLimitCost);
            n.dFactNormativeCost = GridData.Sum(m => m.dFactNormativeCost);
            n.dFactCost = GridData.Sum(m => m.dFactCost);

            rez.Add(n);
            return rez;
        }

    }
}
