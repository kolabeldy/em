using em.DBAccess;
using em.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace em.Models
{
    public class CostCenter
    {
        public int Id { get; set; }
        public int IdCode { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public bool IsActual { get; set; }
        public bool IsTechnology { get; set; }

        public static List<CostCenter> ToList(bool isMain, bool isTechnology)
        {
            List<CostCenter> rez = new List<CostCenter>();
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                string idMain = isMain ? "1" : "0";
                string idTech = isTechnology ? "1" : "0";
                db.Open();
                string SQLtxt = "SELECT IdCode, Name FROM CostCenters  WHERE IsActual = 1 AND IsMain = " + idMain + " AND IsTechnology = " + idTech
                                + " ORDER BY IdCode";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    CostCenter r = new CostCenter();
                    r.Id = q.GetInt32(0);
                    r.Name = q.GetString(1);
                    rez.Add(r);
                }
            }
            return rez;
        }

        public static List<CostCenter> ToList(SelectChoise isActual, SelectChoise isMain, SelectChoise isTechnology)
        {
            string isActualSrt = isActual == SelectChoise.All ? "" : isActual == SelectChoise.True ? " AND IsActual = 1" : " AND IsActual = 0";
            string isMainStr = isMain == SelectChoise.All ? "" : isMain == SelectChoise.True ? " AND IsMain = 1" : " AND IsMain = 0"; ;
            string isTechnologyStr = isTechnology == SelectChoise.All ? "" : isTechnology == SelectChoise.True ? " AND isTechnology = 1" : " AND isTechnology = 0";
            string whereStr = "WHERE True" + isActualSrt + isMainStr + isTechnologyStr;

            List<CostCenter> rez = new List<CostCenter>();
            string SQLtxt = "SELECT IdCode, Name, IsMain, IsActual, IsTechnology FROM CostCenters " + whereStr + " ORDER BY IdCode";

            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    CostCenter r = new CostCenter();
                    r.Id = q.GetInt32(0);
                    r.Name = q.GetString(1);
                    r.IsMain = q.GetBoolean(2);
                    r.IsActual = q.GetBoolean(3);
                    r.IsTechnology = q.GetBoolean(4);
                    rez.Add(r);
                }
            }
            return rez;
        }

        public static List<Person> AllToList()
        {
            List<Person> rez = new List<Person>();
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT IdCode, Name FROM CostCenters WHERE IsActual ORDER BY IdCode";
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

        public static void Add(int id, string name, int ismain, int istechnology, int isactual)
        {
            string SQLtxt = default;
            SqliteCommand insertCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    SQLtxt = "INSERT INTO CostCenters (IdCode, Name, IsMain, IsTechnology, IsActual) VALUES ("
                            + id.ToString() + ", '" + name + "'" + ", " + ismain.ToString() + ", " + istechnology.ToString() + ", " + isactual.ToString() + ")";
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
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    SQLtxt = "Delete FROM CostCenters  WHERE IdCode = " + id.ToString();
                    insertCommand = db.CreateCommand();
                    insertCommand.CommandText = SQLtxt;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                db.Close();
            }

        }
        public static void Update(int id, string name, int ismain, int istechnology, int isactual)
        {
            string SQLtxt = default;
            SqliteCommand insertCommand;
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    SQLtxt = "UPDATE CostCenters SET (Name, IsMain, IsTechnology, IsActual) = ("
                            + "'" + name + "'" + ", " + ismain.ToString() + ", " + istechnology.ToString() + ", " + isactual.ToString() + ")"
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
