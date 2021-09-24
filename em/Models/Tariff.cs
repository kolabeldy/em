using em.DBAccess;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em.Models
{
    public class Tariff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Tarif { get; set; }

        public static List<Tariff> ToList()
        {
            List<Tariff> rez = new();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT Id, Tarif, Name, Unit FROM Tariffs "
                                + "ORDER BY Id";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    Tariff r = new();
                    r.Id = q.GetInt32(0);
                    r.Tarif = q.GetDouble(1);
                    r.Name = q.GetString(2);
                    r.Unit = q.GetString(3);
                    rez.Add(r);
                }
            }
            return rez;
        }
        public static void Save(List<Tariff> tmpTariff)
        {
            string SQLtxt = default;
            SqliteCommand updateCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    foreach (var r in tmpTariff)
                    {
                        SQLtxt = "UPDATE Tariffs SET Tarif = '" + r.Tarif.ToString().Replace(",", ".") + "' WHERE Id = " + r.Id.ToString() + " ";
                        updateCommand = db.CreateCommand();
                        updateCommand.CommandText = SQLtxt;
                        updateCommand.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                db.Close();
            }

        }


    }
}

