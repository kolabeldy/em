using em.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em
{
    public enum ChartType { UseER, UseCC, UsePR, LossesER, CompareER, CompareCC }
    public enum ChartDataType { ER, CC, FactLoss, Period }

    public enum ChartWidthType { Slim, Wide }
    public enum SelectChoise { All, True, False}

    public static class Global
    {
        public static List<Person> RetDynamicTruePeriod(int periodFirst, int periodLast)
        {
            int lastYear = periodFirst == periodLast ? periodLast / 100 + 1 : periodLast / 100;
            int startYear = periodFirst == periodLast ? periodFirst / 100 - 1 : periodFirst / 100;
            int lastMonth = periodFirst == periodLast ? 12 : periodLast - lastYear * 100;
            int startMonth = periodFirst == periodLast ? 1 : periodFirst - startYear * 100;


            string[] arrMonth = new string[] { "янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт", "ноя", "дек" };
            int[] arrYear = new int[lastYear - startYear + 1];

            for (int i = 0; i < (lastYear - startYear + 1); i++)
            {
                arrYear[i] = startYear + i;
            }
            List<Person> rez = new();
            for (int y = startYear; y < lastYear + 1; y++)
            {
                if (y == startYear)
                    for (int m = startMonth; m <= 12; m++)
                    {
                        rez.Add(new Person()
                        {
                            Id = y * 100 + m,
                            Name = $"{y} {arrMonth[m - 1]}",
                        });
                    }
                else if (y == lastYear)
                    for (int m = 1; m <= lastMonth; m++)
                    {
                        rez.Add(new Person()
                        {
                            Id = y * 100 + m,
                            Name = $"{y} {arrMonth[m - 1]}",
                        });
                    }
                else
                    for (int m = 1; m <= 12; m++)
                    {
                        rez.Add(new Person()
                        {
                            Id = y * 100 + m,
                            Name = $"{y} {arrMonth[m - 1]}",
                        });
                    }
            }
            return rez;
        }
        public static string ListToSting(List<Person> list)
        {
            string strList = "( ";
            foreach (Person r in list)
            {
                strList += r.Id + ", ";
            }
            return strList.Substring(0, strList.Length - 2) + " )";
        }


    }
}
