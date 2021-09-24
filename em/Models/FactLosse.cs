using em.DBAccess;
using em.Helpers;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace em.Models
{
    public class FactLosse
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Period { get; set; }
        public int Kvart { get; set; }
        public int IdCC { get; set; }
        public int IdER { get; set; }
        public double Fact { get; set; }

        public static List<FullFields> ToList(List<Person> dateSel)
        {
            List<FullFields> rez = new List<FullFields>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT Period, IdER, ERName, UnitName, IsERPrime, "
                                + "SUM(FactCost) as FactCost "
                                + "FROM LosseFullCosts "
                                + "WHERE Period IN " + Global.ListToSting(dateSel) + " "
                                + "GROUP BY IsERPrime, IdER";

                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    FullFields r = new FullFields();
                    r.Period = q.GetInt32(0);
                    r.IsERPrime = q.GetBoolean(4);
                    r.FactCost = q.GetDouble(5);
                    rez.Add(r);
                }
            }
            return rez;
        }

    }
}
