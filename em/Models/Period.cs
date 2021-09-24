using em.DBAccess;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace em.Models
{
    public static class Period
    {
        public static int FirstPeriod { get; set; }
        public static int FirstYear { get; set; }
        public static int FirstMonth { get; set; }
        public static int LastPeriod { get; set; }
        public static int LastYear { get; set; }
        public static int LastMonth { get; set; }
        public static int LastPeriodLosses { get; set; }
        public static string LastPeriodDay { get; set; }


        public static void InitPeriods()
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT Min(Period) as Period FROM ERUses ORDER BY Period";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    FirstPeriod = q.GetInt32(0);
                }
                SQLtxt = "SELECT Max(Period) as Period FROM ERUses ORDER BY Period";
                selectCommand = new SqliteCommand(SQLtxt, db);

                q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    LastPeriod = q.GetInt32(0);
                }

                SQLtxt = "SELECT Period FROM FactLosses ORDER BY Period DESC Limit 1";
                selectCommand = new SqliteCommand(SQLtxt, db);
                q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    LastPeriodLosses = q.GetInt32(0);
                }

                SQLtxt = "SELECT Period FROM CurrentERUses ORDER BY Period DESC Limit 1";
                selectCommand = new SqliteCommand(SQLtxt, db);
                q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    LastPeriodDay = q.GetString(0);
                }

            }
            FirstYear = FirstPeriod / 100;
            FirstMonth = FirstPeriod - FirstYear * 100;
            LastYear = LastPeriod / 100;
            LastMonth = LastPeriod - LastYear * 100;
        }

    }
}
