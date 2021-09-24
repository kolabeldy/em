using em.DBAccess;
using em.Helpers;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace em.Models
{
    public class EResource
    {
        public int Id { get; set; }
        public int IdCode { get; set; }
        public int IdCodeGroup { get; set; }
        public string Name { get; set; }
        public string NameGroup { get; set; }
        public int Unit { get; set; }
        public bool IsMain { get; set; }
        public bool? IsPrime { get; set; }
        public bool IsActual { get; set; }
        public bool IsSelected { get; set; }

        public static List<EResource> ToList(SelectChoise isActual, SelectChoise isMain, SelectChoise isPrime)
        {
            string isActualSrt = isActual == SelectChoise.All ? "" : isActual == SelectChoise.True ? " AND IsActual = 1" : " AND IsActual = 0";
            string isMainStr = isMain == SelectChoise.All ? "" : isMain == SelectChoise.True ? " AND IsMain = 1" : " AND IsMain = 0"; ;
            string isPrimeStr = isPrime == SelectChoise.All ? "" : isPrime == SelectChoise.True ? " AND isPrime = 1" : " AND isPrime = 0";
            string whereStr = "WHERE True" + isActualSrt + isMainStr + isPrimeStr;

            List<EResource> rez = new();
            string SQLtxt = "SELECT IdCode, Name, IsMain, IsActual, IsPrime, NameGroup FROM EResources " + whereStr + " ORDER BY IdCode";

            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new EResource()
                    {
                        Id = q.GetInt32(0),
                        Name = q.GetString(1),
                        IsMain = q.GetBoolean(2),
                        IsActual = q.GetBoolean(3),
                        IsPrime = q.GetBoolean(4),
                        NameGroup = q.GetString(5)
                    });
                }
            }
            return rez;
        }
        public static List<Person> ActualToList()
        {
            List<Person> rez = new();
            rez.AddRange(from r in ToList(isActual: SelectChoise.True, isMain: SelectChoise.All, isPrime: SelectChoise.All)
                         select new Person { Id = r.Id, Name = r.NameGroup });
            return rez;
        }

        public static List<EResource> ToListAll()
        {
            List<EResource> rez = new List<EResource>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT IdCode, Name, Unit, IsMain, IsPrime, IsActual FROM EResources "
                                + "ORDER BY IdCode";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    EResource r = new EResource();
                    r.IdCode = q.GetInt32(0);
                    r.Name = q.GetString(1);
                    r.Unit = q.GetInt32(2);
                    r.IsMain = q.GetBoolean(3);
                    r.IsPrime = q.GetBoolean(4);
                    r.IsActual = q.GetBoolean(5);
                    rez.Add(r);
                }
            }
            return rez;
        }
        public static void Add(int id, string name, int unit, int ismain, int isprime, int isactual)
        {
            string sql = "INSERT INTO EResources (IdCode, IdCodeGroup, Name, Unit, IsMain, IsPrime, IsActual) VALUES ("
                            + id.ToString() + ", " + id.ToString() + ", '" + name + "'" + ", " + unit.ToString() + ", " 
                            + ismain.ToString() + ", " + isprime.ToString() + ", " + isactual.ToString() + ")";
            SqliteCommand insertCommand;
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    insertCommand = db.CreateCommand();
                    insertCommand.CommandText = sql;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                db.Close();
            }

        }
        public static void Delete(int id)
        {
            string sql = "Delete FROM EResources  WHERE IdCode = " + id.ToString();
            SqliteCommand insertCommand;
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    insertCommand = db.CreateCommand();
                    insertCommand.CommandText = sql;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                db.Close();
            }

        }
        public static void Update(int id, string name, int unit, int ismain, int isprime, int isactual)
        {
            string sql = "UPDATE EResources SET (Name, Unit, IsMain, IsActual) = ("
                            + "'" + name + "'" + ", " + unit.ToString() + ", "
                            + ismain.ToString() + ", " + isactual.ToString() + ")"
                            + "WHERE IdCode = " + id.ToString();
            SqliteCommand insertCommand;
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                using (SqliteTransaction transaction = db.BeginTransaction())
                {
                    
                    insertCommand = db.CreateCommand();
                    insertCommand.CommandText = sql;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                db.Close();
            }

        }


    }
}
