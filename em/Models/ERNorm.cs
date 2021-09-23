using em.DBAccess;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace em.Models
{
    public class ERNorm
    {
        public int Id { get; set; }
        public int IdER { get; set; }
        public int IdPrime { get; set; }
        public int Season { get; set; }
        public double Norm { get; set; }

        private class TempNormTable
        {
            public int IdProduct { get; set; }
            public int IdCostCenter { get; set; }
            public int IdER { get; set; }
            public double K { get; set; }
            public double NormWinter { get; set; }
            public double NormSummer { get; set; }
        }

        public static void DeleteAll()
        {
            using (SqliteConnection db = new($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                SqliteCommand deleteCommand = new();
                deleteCommand.Connection = db;
                deleteCommand.CommandText = "Delete FROM ERNorms";
                deleteCommand.ExecuteNonQuery();
                db.Close();
            }
        }
        public static void InsertRec()
        {
            List<TempNormTable> normList = new List<TempNormTable>();
            int[] arrIdER = new int[] { 950, 951, 954, 955, 966, 990, 991 };
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT IdCostCenter, IdProduct, IdER, K, NormWinter, NormSummer FROM Norms";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    normList.Add(new TempNormTable()
                    {
                        IdCostCenter = q.GetInt32(0),
                        IdProduct = q.GetInt32(1),
                        IdER = q.GetInt32(2),
                        K = q.GetDouble(3),
                        NormWinter = q.GetDouble(4),
                        NormSummer = q.GetDouble(5)
                    });
                }
                for (int i = 0; i < arrIdER.Length; i++)
                {
                    normList.Add(new TempNormTable()
                    {
                        IdCostCenter = 23,
                        IdProduct = arrIdER[i],
                        IdER = arrIdER[i],
                        K = 1,
                        NormWinter = 1,
                        NormSummer = 1
                    });
                }
                var qry = from o in normList
                          where o.IdCostCenter == 23
                          select new
                          {
                              IdProduct = o.IdProduct,
                              IdER = o.IdER,
                              NormWinter = o.NormWinter,
                              NormSummer = o.NormSummer,
                              K = o.K
                          };
                var qry1 = from o1 in qry
                           join o2 in normList on o1.IdER equals o2.IdProduct
                           join o3 in normList on o2.IdER equals o3.IdProduct
                           join o4 in normList on o3.IdER equals o4.IdProduct
                           select new
                           {
                               IdProduct = o1.IdProduct,
                               IdER1 = o1.IdER,
                               NormWinter1 = o1.NormWinter,
                               NormSummer1 = o1.NormSummer,
                               IdER2 = o2.IdER,
                               NormWinter2 = o2.NormWinter,
                               NormSummer2 = o2.NormSummer,
                               IdER3 = o3.IdER,
                               NormWinter3 = o3.NormWinter,
                               NormSummer3 = o3.NormSummer,
                               IdER4 = o4.IdER,
                               NormWinter4 = o4.NormWinter,
                               NormSummer4 = o4.NormSummer,
                               NormWinter = o1.NormWinter * o1.K * o2.NormWinter * o2.K * o3.NormWinter * o3.K * o4.NormWinter * o4.K,
                               NormSummer = o1.NormSummer * o1.K * o2.NormSummer * o2.K * o3.NormSummer * o3.K * o4.NormSummer * o4.K
                           };
                var sqry = from o in qry1
                           group o by new { o.IdProduct, o.IdER4 } into gr
                           select new
                           {
                               IdProduct = gr.Key.IdProduct,
                               IdER = gr.Key.IdER4,
                               NormWinter = gr.Sum(m => m.NormWinter),
                               NormSummer = gr.Sum(m => m.NormSummer)
                           };
                var tList = sqry.ToList();

                string sqlText = default;

                using (var transaction = db.BeginTransaction())
                {
                    SqliteCommand insertCmd = new();
                    foreach (var r in tList)
                    {
                        string NormWinter = Convert.ToDouble(r.NormWinter).ToString().Replace(",", ".");
                        string NormSummer = Convert.ToDouble(r.NormSummer).ToString().Replace(",", ".");

                        sqlText = string.Format("INSERT INTO ERNorms (IdER, IdPrime, Season, Norm) "
                            + "VALUES ({0},{1},{2},{3})", r.IdProduct, r.IdER, "1", NormWinter);
                        insertCmd = db.CreateCommand();
                        insertCmd.CommandText = sqlText;
                        insertCmd.ExecuteNonQuery();

                        sqlText = string.Format("INSERT INTO ERNorms (IdER, IdPrime, Season, Norm) "
                            + "VALUES ({0},{1},{2},{3})", r.IdProduct, r.IdER, "2", NormSummer);

                        insertCmd = db.CreateCommand();
                        insertCmd.CommandText = sqlText;
                        insertCmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    insertCmd = db.CreateCommand();
                    insertCmd.CommandText = "DELETE FROM ERNorms WHERE (IDER = 990 AND IDPrime <> 990) OR IDPrime = 1644";
                    insertCmd.ExecuteNonQuery();
                }
                db.Close();
                MessageBox.Show(string.Format("В базу данных загружено {0} записей.", tList.Count() * 2));
            }
        }


    }
}
