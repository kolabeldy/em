using em.DBAccess;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em.Models
{
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double K { get; set; }

        public static List<Unit> RetUnitList()
        {
            List<Unit> rez = new List<Unit>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT Id, Name, K FROM Units ORDER BY Id";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    Unit r = new Unit();
                    r.Id = q.GetInt32(0);
                    r.Name = q.GetString(1);
                    r.K = q.GetDouble(2);
                    rez.Add(r);
                }
            }
            return rez;
        }


    }
}

