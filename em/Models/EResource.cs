using em.DBAccess;
using em.Helpers;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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

        public static List<Person> AllToList()
        {
            List<Person> rez = new List<Person>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT IdCode, Name FROM EResources WHERE IsActual ORDER BY IdCode";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    Person r = new Person();
                    r.Id = q.GetInt32(0);
                    r.Name = q.GetString(1);
                    rez.Add(r);
                }
            }
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
        public static List<EResource> ToList()
        {
            List<EResource> rez = new List<EResource>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT IdCode, Name FROM EResources WHERE IsActual = 1 "
                                + "ORDER BY IdCode";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    EResource r = new EResource();
                    r.IdCode = q.GetInt32(0);
                    r.Name = q.GetString(1);
                    rez.Add(r);
                }
            }
            return rez;
        }
        public static List<EResource> ToList(bool isPrime)
        {
            string idPrime = isPrime ? "1" : "0";
            List<EResource> rez = new List<EResource>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT IdCode, Name FROM EResources WHERE IsActual = 1 AND IsPrime = " + idPrime
                                + " ORDER BY IdCode";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    EResource r = new EResource();
                    r.Id = q.GetInt32(0);
                    r.Name = q.GetString(1);
                    rez.Add(r);
                }
            }
            return rez;
        }

        public static void Add(int id, string name, int unit, int ismain, int isprime, int isactual)
        {
            string SQLtxt = default;
            SqliteCommand insertCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    SQLtxt = "INSERT INTO EResources (IdCode, IdCodeGroup, Name, Unit, IsMain, IsPrime, IsActual) VALUES ("
                            + id.ToString() + ", " + id.ToString() + ", '" + name + "'" + ", " + unit.ToString() + ", " + ismain.ToString() + ", " + isprime.ToString() + ", " + isactual.ToString() + ")";
                    insertCommand = db.CreateCommand();
                    insertCommand.CommandText = SQLtxt;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                db.Close();
            }

        }
        public static void Delete(int id)
        {
            string SQLtxt = default;
            SqliteCommand insertCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    SQLtxt = "Delete FROM EResources  WHERE IdCode = " + id.ToString();
                    insertCommand = db.CreateCommand();
                    insertCommand.CommandText = SQLtxt;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                db.Close();
            }

        }
        public static void Update(int id, string name, int unit, int ismain, int isprime, int isactual)
        {
            string SQLtxt = default;
            SqliteCommand insertCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    SQLtxt = "UPDATE EResources SET (Name, Unit, IsMain, IsActual) = ("
                            + "'" + name + "'" + ", " + unit.ToString() + ", "
                            + ismain.ToString() + ", " + isactual.ToString() + ")"
                            + "WHERE IdCode = " + id.ToString();
                    insertCommand = db.CreateCommand();
                    insertCommand.CommandText = SQLtxt;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                db.Close();
            }

        }


    }
}
