using em.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace em.FiltersSections
{
    public partial class FilterTreeSection : UserControl, INotifyPropertyChanged
    {

        #region Delegates & Events

        public delegate void IsChangeMetodContainer(List<Person> filterList);
        public event IsChangeMetodContainer onChange;

        #endregion

        #region Properties
        public ObservableCollection<Family> Families { get; set; }
        public string Tittle { get; set; }
        public List<Person> FilterList { get; set; }

        #endregion

        #region Observable Properties

        private bool popupIsOpen = false;
        public bool PopupIsOpen
        {
            get
            {
                return popupIsOpen;
            }
            set
            {
                popupIsOpen = value;
                if (value == false)
                {
                    List<Person> tmpList = new List<Person>();
                    tmpList = PersonListFill();
                    if (!ListCompare(FilterList, tmpList))
                    {
                        FilterList.Clear();
                        FilterList = tmpList;
                        SelectedText = RetSelected();
                        onChange(FilterList);
                    }
                }
                OnPropertyChanged("PopupIsOpen");
            }
        }

        private bool isFilterSectionEnabled = true;
        public bool IsFilterSectionEnabled
        {
            get
            {
                return isFilterSectionEnabled;
            }
            set
            {
                isFilterSectionEnabled = value;
                OnPropertyChanged("IsFilterSectionEnabled");
            }
        }
        private Visibility toggleButtonCheckedAllIsVisible = Visibility.Visible;
        public Visibility ToggleButtonCheckedAllIsVisible
        {
            get
            {
                return toggleButtonCheckedAllIsVisible;
            }
            set
            {
                toggleButtonCheckedAllIsVisible = value;
                OnPropertyChanged("ToggleButtonCheckedAllIsVisible");
            }
        }
        private Visibility isFilterSectionVisible = Visibility.Visible;
        public Visibility IsFilterSectionVisible
        {
            get
            {
                return isFilterSectionVisible;
            }
            set
            {
                isFilterSectionVisible = value;
                OnPropertyChanged("IsFilterSectionVisible");
            }
        }

        private double popupOffset = 0;
        public double PopupHorisontalOffset
        {
            get
            {
                return popupOffset;
            }
            set
            {
                popupOffset = value;
                OnPropertyChanged("PopupOffset");
            }
        }

        private string selectedText;
        public string SelectedText
        {
            get
            {
                return selectedText;
            }
            set
            {
                selectedText = value;
                OnPropertyChanged("SelectedText");
            }
        }

        #endregion

        #region private Fields

        private TreeInitType treeInitType;
        private int filterListAllCount;

        #endregion
        public FilterTreeSection()
        {
            Families = new ObservableCollection<Family>();
            FilterList = new List<Person>();
            InitializeComponent();
            DataContext = this;
        }

        #region Methods
        public void Init(ObservableCollection<Family> families, string tittle, TreeInitType treeInitType = TreeInitType.All)
        {
            this.treeInitType = treeInitType;
            this.Families = families;
            this.Tittle = tittle;
            Families = FamiliesInit(Families);
            FilterList = PersonListFill();
            SelectedText = RetSelected();
        }
        public ObservableCollection<Family> FamiliesInit(ObservableCollection<Family> families)
        {
            filterListAllCount = 0;
            foreach (Family family in families)
                foreach (var person in family.Members)
                {
                    person.SetValue(ItemHelper.ParentProperty, family);
                    filterListAllCount++;
                }
            int i = 1;
            foreach (Family family in families)
                foreach (var person in family.Members)
                {
                    switch (treeInitType)
                    {
                        case TreeInitType.First:
                            if (i > 1)
                            {
                                ItemHelper.SetIsChecked(person, false);
                            }
                            i++;
                            break;
                        case TreeInitType.Last:
                            if (i != filterListAllCount)
                            {
                                ItemHelper.SetIsChecked(person, false);
                            }
                            i++;
                            break;
                    }
                }
            return families;
        }
        protected List<Person> PersonListFill()
        {
            List<Person> tmpList = new List<Person>();
            foreach (Family family in Families)
                foreach (Person person in family.Members)
                    if (ItemHelper.GetIsChecked(person) == true)
                    {
                        tmpList.Add(new Person() { Id = person.Id, Name = person.Name });
                    }
            var qry = (from o in tmpList
                       group o by new { o.Id } into gr
                       select new Person
                       {
                           Id = gr.Key.Id,
                           Name = gr.Max(m => m.Name)
                       }).ToList();
            //if(Tittle == "Продукты по ЦЗ:")
            //{
            //    var a = qry;
            //}

            return qry;
        }

        public string RetSelected()
        {
            string rez = "выборочно";
            int countList = FilterList.Count;
            if (countList < 4)
            {
                if (countList == 1) rez = FilterList[0].Name;
                if (countList == 2) rez = FilterList[0].Name + ", " + FilterList[1].Name;
                if (countList == 3) rez = FilterList[0].Name + ", " + FilterList[1].Name + ", " + FilterList[2].Name;
            }
            else if (countList == filterListAllCount) rez = "все";
            if (countList == 0) rez = "не выбрано";
            return rez;
        }
        public static bool ListCompare(List<Person> List1, List<Person> List2)
        {
            if (List1.Count == List2.Count)
            {
                for (int i = 0; i < List1.Count; i++)
                {
                    if (List1[i].Id != List2[i].Id)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        protected void SelectAll()
        {
            foreach (Family family in Families)
                foreach (var person in family.Members)
                {
                    ItemHelper.SetIsChecked(person, true);
                }
        }
        protected void UnselectAll()
        {
            foreach (Family family in Families)
                foreach (var person in family.Members)
                {
                    ItemHelper.SetIsChecked(person, false);
                }
        }
        private MyRelayCommand toggleButtonAllCheckedUnchecked_Command;
        public MyRelayCommand ToggleButtonAllCheckedUnchecked_Command
        {
            get
            {
                return toggleButtonAllCheckedUnchecked_Command ??
                    (toggleButtonAllCheckedUnchecked_Command = new MyRelayCommand(obj =>
                    {
                        if ((bool)((ToggleButton)obj).IsChecked == true)
                            SelectAll();
                        else
                            UnselectAll();
                    }));
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
