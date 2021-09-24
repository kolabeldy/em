using em.DBAccess;
using em.Helpers;
using em.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace em.MenuPages
{
    class NormLossEditModel : INotifyPropertyChanged
    {
        MainWindow window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        public string ChartCaption { get; set; }
        public bool IsAdmin { get; set; }

        private ObservableCollection<NormLossTable> normLossList;
        public ObservableCollection<NormLossTable> NormLossList
        {
            get
            {
                return normLossList;
            }
            set
            {
                normLossList = value;
                OnPropertyChanged("NormLossList");
            }
        }
        private bool _isEdit;
        public bool IsEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                _isEdit = value;
                IsTableReadOnly = !value;
                OnPropertyChanged("IsEdit");
            }
        }
        private bool _isTableReadOnly;
        public bool IsTableReadOnly
        {
            get
            {
                return _isTableReadOnly;
            }
            set
            {
                _isTableReadOnly = value;
                OnPropertyChanged("IsTableReadOnly");
            }
        }

        private int newIdCode;
        public int NewIdCode
        {
            get
            {
                return newIdCode;
            }
            set
            {
                newIdCode = value;
                OnPropertyChanged("NewIdCode");
            }
        }
        protected Visibility controlShadowVisible = Visibility.Collapsed;
        public Visibility ControlShadowVisible
        {
            get
            {
                return controlShadowVisible;
            }
            set
            {
                controlShadowVisible = value;
                OnPropertyChanged("ControlShadowVisible");
            }
        }


        public NormLosse SelectedER { get; set; }

        private bool addERIsOpen;
        public bool AddERIsOpen
        {
            get
            {
                return addERIsOpen;
            }
            set
            {
                addERIsOpen = value;
                if (value == true) ControlShadowVisible = Visibility.Visible;
                else ControlShadowVisible = Visibility.Collapsed;
                OnPropertyChanged("AddERIsOpen");
            }
        }
        private bool normEditIsOpen;
        public bool NormEditIsOpen
        {
            get
            {
                return normEditIsOpen;
            }
            set
            {
                normEditIsOpen = value;
                if (value == true) ControlShadowVisible = Visibility.Visible;
                else ControlShadowVisible = Visibility.Collapsed;
                OnPropertyChanged("NormEditIsOpen");
            }
        }

        public NormLossEditModel()
        {
            NormLossList = new ObservableCollection<NormLossTable>();
            NormLossListFill();
            ChartCaption = "Нормативы потерь энергоресурсов, %";
            IsAdmin = true;
            IsEdit = false;
            IsTableReadOnly = true;
        }

        private void NormLossListFill()
        {
            List<NormLosse> NLossList = NormLosse.ToList();
            NormLossList.Clear();
            var qrySource = from e in EResource.ActualToList()
                            join o1 in NLossList on e.Id equals o1.IdER
                            where o1.Kvart == 1
                            join o2 in NLossList on e.Id equals o2.IdER
                            where o2.Kvart == 2
                            join o3 in NLossList on e.Id equals o3.IdER
                            where o3.Kvart == 3
                            join o4 in NLossList on e.Id equals o4.IdER
                            where o4.Kvart == 4
                            orderby e.Id
                            select new
                            {
                                IdER = e.Id,
                                ERName = e.Name,
                                LossKv1 = o1.LossesNorm,
                                LossKv2 = o2.LossesNorm,
                                LossKv3 = o3.LossesNorm,
                                LossKv4 = o4.LossesNorm,
                            };
            foreach (var o in qrySource)
            {
                NormLossTable n = new NormLossTable();
                n.IdER = o.IdER;
                n.ERName = o.ERName;
                n.LossKv1 = o.LossKv1;
                n.LossKv2 = o.LossKv2;
                n.LossKv3 = o.LossKv3;
                n.LossKv4 = o.LossKv4;
                NormLossList.Add(n);
            }
        }

        private MyRelayCommand addCommand;
        public MyRelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                    (addCommand = new MyRelayCommand(obj =>
                    {
                        IdCode = 0;
                        Loss1Kv = 0;
                        Loss2Kv = 0;
                        Loss3Kv = 0;
                        Loss4Kv = 0;
                        AddERIsOpen = true;
                    }));
            }
        }
        private int idCode;
        public int IdCode
        {
            get
            {
                return idCode;
            }
            set
            {
                idCode = value;
                OnPropertyChanged("IdCode");
            }
        }
        private string erName;
        public string ERName
        {
            get
            {
                return erName;
            }
            set
            {
                erName = value;
                OnPropertyChanged("ERName");
            }
        }

        private double loss1Kv;
        public double Loss1Kv
        {
            get
            {
                return loss1Kv;
            }
            set
            {
                loss1Kv = value;
                OnPropertyChanged("Loss1Kv");
            }
        }
        private double loss2Kv;
        public double Loss2Kv
        {
            get
            {
                return loss2Kv;
            }
            set
            {
                loss2Kv = value;
                OnPropertyChanged("Loss2Kv");
            }
        }
        private double loss3Kv;
        public double Loss3Kv
        {
            get
            {
                return loss3Kv;
            }
            set
            {
                loss3Kv = value;
                OnPropertyChanged("Loss3Kv");
            }
        }
        private double loss4Kv;
        public double Loss4Kv
        {
            get
            {
                return loss4Kv;
            }
            set
            {
                loss4Kv = value;
                OnPropertyChanged("Loss4Kv");
            }
        }

        private MyRelayCommand editCommand;
        public MyRelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                    (editCommand = new MyRelayCommand(obj =>
                    {
                        NormEditIsOpen = true;
                        NormLossTable n = new NormLossTable();
                        n = (NormLossTable)obj;
                        IdCode = n.IdER;
                        ERName = n.ERName;
                        Loss1Kv = n.LossKv1;
                        Loss2Kv = n.LossKv2;
                        Loss3Kv = n.LossKv3;
                        Loss4Kv = n.LossKv4;
                    }));
            }
        }

        private MyRelayCommand btnPopupAddClose_Command;
        public MyRelayCommand BtnPopupAddClose_Command
        {
            get
            {
                return btnPopupAddClose_Command ??
                    (btnPopupAddClose_Command = new MyRelayCommand(obj =>
                    {
                        AddERIsOpen = false;
                    }));
            }
        }
        private MyRelayCommand btnPopupEditSave_Command;
        public MyRelayCommand BtnPopupEditSave_Command => btnPopupEditSave_Command ??
                    (btnPopupEditSave_Command = new MyRelayCommand(obj =>
                    {
                        ObservableCollection<NormLossTable> tmp = new();
                        foreach (NormLossTable r in normLossList)
                        {
                            NormLossTable n = new();
                            if (r.IdER == IdCode)
                            {
                                n.IdER = IdCode;
                                n.ERName = r.ERName;
                                n.LossKv1 = Loss1Kv;
                                n.LossKv2 = Loss2Kv;
                                n.LossKv3 = Loss3Kv;
                                n.LossKv4 = Loss4Kv;
                            }
                            else
                            {
                                n.IdER = r.IdER;
                                n.ERName = r.ERName;
                                n.LossKv1 = r.LossKv1;
                                n.LossKv2 = r.LossKv2;
                                n.LossKv3 = r.LossKv3;
                                n.LossKv4 = r.LossKv4;
                            }
                            tmp.Add(n);
                        }
                        normLossList.Clear();
                        foreach (var r in tmp)
                        {
                            NormLossTable n = new();
                            n.IdER = r.IdER;
                            n.ERName = r.ERName;
                            n.LossKv1 = r.LossKv1;
                            n.LossKv2 = r.LossKv2;
                            n.LossKv3 = r.LossKv3;
                            n.LossKv4 = r.LossKv4;
                            normLossList.Add(n);
                        }
                        NormEditIsOpen = false;
                    }));
        private MyRelayCommand btnPopupEditClose_Command;
        public MyRelayCommand BtnPopupEditClose_Command
        {
            get
            {
                return btnPopupEditClose_Command ??
                    (btnPopupEditClose_Command = new MyRelayCommand(obj =>
                    {
                        NormEditIsOpen = false;
                    }));
            }
        }

        private MyRelayCommand btnPopupAddSave_Command;
        public MyRelayCommand BtnPopupAddSave_Command
        {
            get
            {
                return btnPopupAddSave_Command ??
                    (btnPopupAddSave_Command = new MyRelayCommand(obj =>
                    {
                        NormLossTable rec = new();
                        rec.IdER = IdCode;
                        rec.LossKv1 = Loss1Kv;
                        rec.LossKv2 = Loss2Kv;
                        rec.LossKv3 = Loss3Kv;
                        rec.LossKv4 = Loss4Kv;
                        NormLosse.Add(rec);
                        AddERIsOpen = false;
                        NormLossListFill();
                    }));
            }
        }
        private MyRelayCommand deleteCommand;
        public MyRelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                    (deleteCommand = new MyRelayCommand(obj =>
                    {
                        //int idER = (obj as EResource).Id;
                        //EResource newER = new EResource();
                        //using (db = new MyDBContext())
                        //{
                        //    newER = db.EResources.Find(idER);
                        //    db.EResources.Remove(newER);
                        //    db.SaveChanges();
                        //}
                        //ERListFill();
                    }));
            }
        }


        private MyRelayCommand saveCommand;
        public MyRelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??
                    (saveCommand = new MyRelayCommand(obj =>
                    {
                        NormLosse.Save(TableToList(NormLossList));
                        IsEdit = false;
                        NormLossListFill();
                    }));
            }
        }
        private List<NormLosse> TableToList(ObservableCollection<NormLossTable> table)
        {
            List<NormLosse> rez = new();
            foreach (NormLossTable r in table)
            {
                for(int i = 1; i <= 4; i++)
                {
                    NormLosse n = new();
                    n.IdER = r.IdER;
                    n.Kvart = i;
                    n.LossesNorm = i == 1 ? r.LossKv1 : i == 2 ? r.LossKv2 : i == 3 ? r.LossKv3 : r.LossKv4;
                    rez.Add(n);
                }
            }
            return rez;
        }

        private MyRelayCommand cancelCommand;
        public MyRelayCommand CancelCommand
        {
            get
            {
                return cancelCommand ??
                    (cancelCommand = new MyRelayCommand(obj =>
                    {
                        NormLossListFill();
                        IsEdit = false;
                    }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
