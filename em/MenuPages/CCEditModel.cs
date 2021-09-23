using em.DBAccess;
using em.Helpers;
using em.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace em.MenuPages
{
    class CCEditModel : INotifyPropertyChanged
    {
        MainWindow window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        public string ChartCaption { get; set; }

        private bool isAdmin = false;
        public bool IsAdmin
        {
            get
            {
                return isAdmin;
            }
            set
            {
                isAdmin = value;
                OnPropertyChanged("IsAdmin");
            }
        }
        private ObservableCollection<CostCenter> ccList;
        public ObservableCollection<CostCenter> CCList
        {
            get
            {
                return ccList;
            }
            set
            {
                ccList = value;
                OnPropertyChanged("CCList");
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
        private string newName;
        public string NewName
        {
            get
            {
                return newName;
            }
            set
            {
                newName = value;
                OnPropertyChanged("NewName");
            }
        }

        private bool newIsMain;
        public bool NewIsMain
        {
            get
            {
                return newIsMain;
            }
            set
            {
                newIsMain = value;
                OnPropertyChanged("NewIsMain");
            }
        }
        private bool newIsTechnology;
        public bool NewIsTechnology
        {
            get
            {
                return newIsTechnology;
            }
            set
            {
                newIsTechnology = value;
                OnPropertyChanged("NewIsTechnology");
            }
        }
        private bool newIsActual;
        public bool NewIsActual
        {
            get
            {
                return newIsActual;
            }
            set
            {
                newIsActual = value;
                OnPropertyChanged("NewIsActual");
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

        public CostCenter SelectedCC { get; set; }

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
                if(value == true) ControlShadowVisible = Visibility.Visible;
                    else ControlShadowVisible = Visibility.Collapsed;
                OnPropertyChanged("AddERIsOpen");
            }
        }
        public CCEditModel()
        {
            IsAdmin = true;
            CCListFill();
            ChartCaption = "Реестр центров затрат";
            IsEdit = false;
            IsTableReadOnly = true;
        }

        private void CCListFill()
        {
            CCList = new ObservableCollection<CostCenter>(CostCenter.ToList());
        }

        private MyRelayCommand addCommand;
        public MyRelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                    (addCommand = new MyRelayCommand(obj =>
                    {
                        AddERIsOpen = true;
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
        private MyRelayCommand btnPopupAddSave_Command;
        public MyRelayCommand BtnPopupAddSave_Command
        {
            get
            {
                return btnPopupAddSave_Command ??
                    (btnPopupAddSave_Command = new MyRelayCommand(obj =>
                    {
                        CostCenter.Add(NewIdCode, NewName, Convert.ToInt32(NewIsMain), Convert.ToInt32(NewIsTechnology), Convert.ToInt32(NewIsActual));
                        AddERIsOpen = false;
                        CCListFill();
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
                        CostCenter.Delete(SelectedCC.Id);
                        CCListFill();
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
                        CostCenter.Update(SelectedCC.Id,
                                            SelectedCC.Name,
                                            Convert.ToInt32(SelectedCC.IsMain),
                                            Convert.ToInt32(SelectedCC.IsTechnology),
                                            Convert.ToInt32(SelectedCC.IsActual));
                        CCListFill();
                    }));
            }
        }

        private MyRelayCommand cancelCommand;
        public MyRelayCommand CancelCommand
        {
            get
            {
                return cancelCommand ??
                    (cancelCommand = new MyRelayCommand(obj =>
                    {
                        CCListFill();
                        IsEdit = false;
                    }));
            }
        }
        //private MyRelayCommand erEditCloseCommand;
        //public MyRelayCommand EREditCloseCommand
        //{
        //    get
        //    {
        //        return erEditCloseCommand ??
        //            (erEditCloseCommand = new MyRelayCommand(obj =>
        //            {
        //                window.MainFrame.Content = null;
        //            }));
        //    }
        //}


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
