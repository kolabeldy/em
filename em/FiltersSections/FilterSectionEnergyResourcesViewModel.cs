using em.DBAccess;
using em.Helpers;
using em.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace em.FiltersSections
{
    public class FilterSectionEnergyResourcesViewModel: FilterSectionViewModel
    {
        public FilterSectionEnergyResourcesViewModel(string tittle, TreeInitType treeInitType):base(tittle, treeInitType)
        {

        }

        public override ObservableCollection<Family> RetFamilies()
        {
            ObservableCollection<Family> rez = new ObservableCollection<Family>();
            if (treeInitType != TreeInitType.Losses)
            {
                rez.Add(new Family()
                {
                    Name = "Покупные",
                    Members = PList(EResource.ToList(isActual: SelectChoise.All, isMain: SelectChoise.All, isPrime: SelectChoise.True))
                });
                rez.Add(new Family()
                {
                    Name = "Вторичные",
                    Members = PList(EResource.ToList(isActual: SelectChoise.All, isMain: SelectChoise.All, isPrime: SelectChoise.False))
                });
                return rez;
            }
            else
            {
                rez.Add(new Family()
                {
                    Name = "Покупные",
                    Members = PList(DataAccess.RetERListLosses(isPrime: true))
                });
                rez.Add(new Family()
                {
                    Name = "Вторичные",
                    Members = PList(DataAccess.RetERListLosses(isPrime: false))
                });

                return rez;
            }
            List<Person> PList(List<EResource> tList)
            {
                List<Person> rez1 = new List<Person>();
                foreach (var r in tList)
                {
                    Person n = new Person();
                    n.Id = r.Id;
                    n.Name = r.Id + "." + r.Name;
                    rez1.Add(n);
                }
                return rez1;
            }
        }

    }
}

