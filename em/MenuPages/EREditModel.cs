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
    class EREditModel : INotifyPropertyChanged
    {
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


        private ObservableCollection<EResource> _erList;
        public ObservableCollection<EResource> ERList
        {
            get
            {
                return _erList;
            }
            set
            {
                _erList = value;
                OnPropertyChanged("ERList");
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

        private int newIdUnit;
        public int NewIdUnit
        {
            get
            {
                return newIdUnit;
            }
            set
            {
                newIdUnit = value;
                OnPropertyChanged("NewIdUnit");
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
        private bool newIsPrime;
        public bool NewIsPrime
        {
            get
            {
                return newIsPrime;
            }
            set
            {
                newIsPrime = value;
                OnPropertyChanged("NewIsPrime");
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

        public List<Unit> UnitList { get; set; }

        public EResource SelectedER { get; set; }
        public Unit SelectedUnit { get; set; }

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
        public EREditModel()
        {
            IsAdmin = true;
            UnitList = new List<Unit>();
            UnitListFill();
            ERListFill();
            ChartCaption = "Реестр энергоресурсов";
            IsEdit = false;
            IsTableReadOnly = true;
        }

        private void UnitListFill()
        {
            UnitList = Unit.RetUnitList();
        }

        private void ERListFill()
        {
            ERList = new ObservableCollection<EResource>(EResource.ToListAll());
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
                        EResource.Add(NewIdCode, NewName, SelectedUnit.Id, Convert.ToInt32(NewIsMain), Convert.ToInt32(NewIsPrime), Convert.ToInt32(NewIsActual));
                        AddERIsOpen = false;
                        ERListFill();
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
                        EResource.Delete(SelectedER.IdCode);
                        ERListFill();
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
                        EResource.Update(SelectedER.IdCode, SelectedER.Name, SelectedER.Unit, Convert.ToInt32(SelectedER.IsMain), Convert.ToInt32(SelectedER.IsPrime), Convert.ToInt32(SelectedER.IsActual));
                        ERListFill();
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
                        ERListFill();
                        IsEdit = false;
                    }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
