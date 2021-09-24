using em.DBAccess;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em.Models
{
    public class Norm
    {
        public int Id { get; set; }
        public int IdCostCenter { get; set; }
        public int IdProduct { get; set; }
        public  int IdER { get; set; }
        public double K { get; set; }
        public double NormWinter { get; set; }
        public double NormSummer { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int IdProducer { get; set; }

        public static void DeleteAll()
        {
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand deleteCommand = new();
                deleteCommand.Connection = db;
                deleteCommand.CommandText = "Delete FROM Norms";
                deleteCommand.ExecuteNonQuery();
                db.Close();
            }
        }
        public static void InsertRec(List<TempUseERTable> exportList)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string sqlText = default;
                string NormWinter = default;
                string NormSummer = default;
                //string DateEnd = default;
                string K = default;
                using (var transaction = db.BeginTransaction())
                {
                    foreach (var r in exportList)
                    {
                        NormWinter = r.NormWinter != "" ? Convert.ToDouble(r.NormWinter).ToString().Replace(",", ".")
                            : r.NormAverage != "" ? Convert.ToDouble(r.NormAverage).ToString().Replace(",", ".")
                            : Convert.ToDouble(r.NormSummer).ToString().Replace(",", ".");
                        NormSummer = r.NormSummer != "" ? Convert.ToDouble(r.NormSummer).ToString().Replace(",", ".")
                            : r.NormAverage != "" ? Convert.ToDouble(r.NormAverage).ToString().Replace(",", ".")
                            : Convert.ToDouble(r.NormWinter).ToString().Replace(",", ".");
                        //DateEnd = r.DateEnd ?? "0";
                        if (r.UnitName == "кВт.ч" || r.UnitName == "м3") K = "0.001";
                        else K = "1";
                        sqlText = string.Format("INSERT INTO Norms (IdCostCenter, IdProduct, IdER, NormWinter, NormSummer, DateStart, K, IdProducer) "
                            + "VALUES ({0},{1},{2},{3},{4},'{5}',{6},{7}) ", r.IdCC, r.IdProduct, r.IdER, NormWinter, NormSummer, r.DateStart, K, r.IdERProducer);

                        var insertCmd = db.CreateCommand();
                        insertCmd.CommandText = sqlText;
                        insertCmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                db.Close();
            }
        }


    }
}
