using em.DBAccess;
using em.Helpers;
using em.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace em.FiltersSections
{
    public class FilterSectionCostCentersViewModel: FilterSectionViewModel
    {
        public FilterSectionCostCentersViewModel(string tittle, TreeInitType treeInitType):base(tittle, treeInitType)
        {

        }
        public override ObservableCollection<Family> RetFamilies()
        {
            ObservableCollection<Family> rez = new ObservableCollection<Family>();
            rez.Add(new Family()
            {
                Name = "Основные",
                Members = PList(CostCenter.ToList(isActual: SelectChoise.All, isMain: SelectChoise.True, isTechnology: SelectChoise.True))
            });
            rez.Add(new Family()
            {
                Name = "Прочие технологические",
                Members = PList(CostCenter.ToList(isActual: SelectChoise.All, isMain: SelectChoise.False, isTechnology: SelectChoise.True))
            });
            rez.Add(new Family()
            {
                Name = "Вспомогательные",
                Members = PList(CostCenter.ToList(isActual: SelectChoise.All, isMain: SelectChoise.False, isTechnology: SelectChoise.False))
            });
            return rez;
        }
        protected List<Person> PList(List<CostCenter> tList)
        {
            List<Person> rez1 = new List<Person>();
            foreach (var r in tList)
            {
                Person n = new Person();
                n.Id = r.Id;
                n.Name = r.Name;
                rez1.Add(n);
            }
            return rez1;
        }

    }
}

