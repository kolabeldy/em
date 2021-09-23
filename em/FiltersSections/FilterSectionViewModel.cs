using em.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace em.FiltersSections
{
    public enum TreeInitType { First, Last, All, None, Losses }

    public abstract class FilterSectionViewModel : INotifyPropertyChanged
    {
        public delegate void IsChangeMetodContainer();
        public event IsChangeMetodContainer onChange;


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
                        onChange();
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

        protected TreeInitType treeInitType;
        private int filterListAllCount;

        #endregion
        public FilterSectionViewModel(string tittle, TreeInitType treeInitType)
        {
            Families = new ObservableCollection<Family>();
            FilterList = new List<Person>();
            this.treeInitType = treeInitType;
            this.Tittle = tittle;
        }

        #region Methods
        public void Init()
        {
            Families = FamiliesInit(RetFamilies());
            FilterList = PersonListFill();
            SelectedText = RetSelected();
            onChange();
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
        public abstract ObservableCollection<Family> RetFamilies();

        protected List<Person> PersonListFill()
        {
            List<Person> tmpList = new List<Person>();
            foreach (Family family in Families)
                foreach (Person person in family.Members)
                    if (ItemHelper.GetIsChecked(person) == true)
                    {
                        tmpList.Add(new Person() { Id = person.Id, Name = person.Name });
                    }
            //var qry = (from o in tmpList
            //           group o by new { o.Id } into gr
            //           orderby gr.Key.Id
            //           select new Person
            //           {
            //               Id = gr.Key.Id,
            //               Name = gr.Max(m => m.Name)
            //           }).ToList();
            return tmpList;
        }

        public string RetSelected()
        {
            string rez = "выборочно";

            int countList = FilterList.Count;
            if (countList == 1) rez = FilterList[0].Name;
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

        //private MyRelayCommand toggleButtonAllCheckedUnchecked_Command;
        //public MyRelayCommand ToggleButtonAllCheckedUnchecked_Command
        //{
        //    get
        //    {
        //        return toggleButtonAllCheckedUnchecked_Command ??
        //            (toggleButtonAllCheckedUnchecked_Command = new MyRelayCommand(obj =>
        //            {
        //                if (obj != null)
        //                {
        //                    if ((ToggleButton)obj).IsChecked == true)
        //                        SelectAll();
        //                    else
        //                        UnselectAll();
        //                }
        //            }));
        //    }
        //}
        private MyRelayCommand selectAll_Command;
        public MyRelayCommand SelectAll_Command
        {
            get
            {
                return selectAll_Command ??
                    (selectAll_Command = new MyRelayCommand(obj =>
                    {
                        SelectAll();
                        //UnselectAll();
                    }));
            }
        }
        private MyRelayCommand unselectAll_Command;
        public MyRelayCommand UnselectAll_Command
        {
            get
            {
                return unselectAll_Command ??
                    (unselectAll_Command = new MyRelayCommand(obj =>
                    {
                        //SelectAll();
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

