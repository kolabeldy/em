using em.DBAccess;
using em.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em.Models
{
    public class NormLosse
    {
        public int Id { get; set; }
        public int IdER { get; set; }
        public int Kvart { get; set; }
        public double LossesNorm { get; set; }

        public static List<NormLosse> ToList()
        {
            List<NormLosse> rez = new List<NormLosse>();
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT Id, IdER, Kvart, LossesNorm FROM NormLosses ORDER BY Id";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    NormLosse r = new NormLosse();
                    r.Id = q.GetInt32(0);
                    r.IdER = q.GetInt32(1);
                    r.Kvart = q.GetInt32(2);
                    r.LossesNorm = q.GetDouble(3);
                    rez.Add(r);
                }
            }
            return rez;
        }
        public static void Save(List<NormLosse> normList)
        {
            string SQLtxt = default;
            SqliteCommand updateCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    foreach (var r in normList)
                    {
                        SQLtxt = "UPDATE NormLosses SET LossesNorm = '" + r.LossesNorm.ToString().Replace(",", ".") + "' WHERE IdER = " + r.IdER.ToString() + " AND Kvart = " + r.Kvart.ToString();
                        updateCommand = db.CreateCommand();
                        updateCommand.CommandText = SQLtxt;
                        updateCommand.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                db.Close();
            }

        }
        public static void Add(NormLossTable rec)
        {
            string SQLtxt = default;
            SqliteCommand insertCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        double val = i == 1 ? rec.LossKv1 : i == 2 ? rec.LossKv2 : i == 3 ? rec.LossKv3 : rec.LossKv4;
                        SQLtxt = "INSERT INTO NormLosses (IdER, Kvart, LossesNorm) VALUES ( " + rec.IdER.ToString() + ", " + i.ToString() + ", '" + val.ToString() + "')";
                        insertCommand = db.CreateCommand();
                        insertCommand.CommandText = SQLtxt;
                        insertCommand.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                db.Close();
            }

        }


    }
}
