using em.DBAccess;
using em.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace em.FiltersSections
{
    public class FilterSectionDateViewModel: FilterSectionViewModel
    {
        public FilterSectionDateViewModel(string tittle, TreeInitType treeInitType):base(tittle, treeInitType)
        {

        }
        public override ObservableCollection<Family> RetFamilies()
        {
            int periodFirst = DataAccess.firstPeriod;
            int startYear = DataAccess.firstYear;
            int lastPeriod = DataAccess.lastPeriod;
            int lastYear = DataAccess.lastYear;
            int lastMonth = DataAccess.lastMonth;
            string[] arrMonth = new string[] { "янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек" };
            int[] arrYear = new int[lastYear - startYear + 1];
            for (int i = 0; i < (lastYear - startYear + 1); i++)
            {
                arrYear[i] = startYear + i;
            }
            ObservableCollection<Family> rez = new ObservableCollection<Family>();
            for (int y = startYear; y < lastYear; y++)
            {
                rez.Add(new Family()
                {
                    Name = y.ToString(),
                    Members = PList1(y)
                });
            }
            rez.Add(new Family()
            {
                Name = lastYear.ToString(),
                Members = PList2(lastYear)
            });
            return rez;

            List<Person> PList1(int year)
            {
                List<Person> rez1 = new List<Person>();
                for (int m = 1; m <= 12; m++)
                {
                    rez1.Add(new Person()
                    {
                        Id = year * 100 + m,
                        Name = year.ToString() + " " + arrMonth[m - 1],
                    });
                }
                return rez1;
            }
            List<Person> PList2(int year)
            {
                List<Person> rez2 = new List<Person>();
                for (int m = 1; m <= lastMonth; m++)
                {
                    rez2.Add(new Person()
                    {
                        Id = year * 100 + m,
                        Name = year.ToString() + " " + arrMonth[m - 1],
                    });
                }
                return rez2;
            }
        }


    }
}

