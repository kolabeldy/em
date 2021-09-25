using em.DBAccess;
using em.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace em.Models
{
    public class FullFields
    {

        #region Declare Properties
        public int Period { get; set; }
        public string PeriodStr { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Kvart { get; set; }
        public int Season { get; set; }
        public int IdCC { get; set; }
        public string CCName { get; set; }
        public bool IsCCMain { get; set; }
        public bool IsCCTechnology { get; set; }
        public int IdER { get; set; }
        public string ERName { get; set; }
        public string ERShortName { get; set; }
        public bool IsERMain { get; set; }
        public bool IsERPrime { get; set; }
        public int IdUnit { get; set; }
        public string UnitName { get; set; }
        public int IdProduct { get; set; }
        public string ProductName { get; set; }
        public bool IsNorm { get; set; }
        public double Fact { get; set; }
        public double Plan { get; set; }
        public double Diff { get; set; }
        public double FactCost { get; set; }
        public double PlanCost { get; set; }
        public double DiffCost { get; set; }
        public double DiffProc { get; set; }

        public double FactLoss { get; set; }
        public double NormLoss { get; set; }
        public double DiffLoss { get; set; }
        public double FactLossCost { get; set; }
        public double NormLossCost { get; set; }
        public double DiffLossCost { get; set; }
        public double FactLossProc { get; set; }
        public double NormLossProc { get; set; }

        public double Produced { get; set; }
        public double PlanPrev { get; set; }
        public double FactPrev { get; set; }
        public double dFactLimit { get; set; }
        public double dFactNormative { get; set; }
        public double FactUseVirtual { get; set; }
        public double FactVirtualCost { get; set; }
        public double dFactLimitCost { get; set; }
        public double dFactNormativeCost { get; set; }
        public double dFact { get; set; }
        public double dFactCost { get; set; }
        public double FactCorrectedCost { get; set; }
        public string TotalParamX { get; set; }
        public string Remark { get; set; }

        internal static double Sum(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
        #endregion
        public static List<Person> SelectPeriodList(int begin, int end)
        {
            List<Person> rez = new();
            string sql = "SELECT Period FROM ERUses WHERE Period >= " + begin.ToString()
                + " AND Period <= " + end.ToString()
                + " GROUP BY Period ORDER BY Period";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        Id = q.GetInt32(0),
                        Name = ""
                    });
                }
            }
            return rez;
        }

        #region Total
        public static List<FullFields> RetTotalUseDiffFromERType(List<Person> dateSel)
        {
            List<FullFields> rez = new();
            string sql = "SELECT IsERPrime, SUM(DiffCost) as DiffCost "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "GROUP BY IsERPrime "
                            + "ORDER BY IsERPrime DESC";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        TotalParamX = q.GetDouble(0) > 0 ? "первичные" : "вторичные",
                        DiffCost = q.GetDouble(1)
                    });
                }
            }
            return rez;
        }
        public static List<FullFields> RetTotalUseDiffFromCCType(List<Person> dateSel)
        {
            List<FullFields> rez = new();
            string sql = "SELECT IsCCTechnology, SUM(DiffCost) as DiffCost "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "GROUP BY IsCCTechnology "
                            + "ORDER BY IsCCTechnology DESC";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        TotalParamX = q.GetDouble(0) > 0 ? "технологические" : "вспомогательные",
                        DiffCost = q.GetDouble(1)
                    });
                }
            }
            return rez;
        }
        public static List<FullFields> RetTotalUseDiffFromNormType(List<Person> dateSel)
        {
            List<FullFields> rez = new();
            string sql = "SELECT IsNorm, SUM(DiffCost) as DiffCost "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "GROUP BY IsNorm "
                            + "ORDER BY IsNorm DESC";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        TotalParamX = q.GetDouble(0) > 0 ? "нормируемые" : "лимитируемые",
                        DiffCost = q.GetDouble(1)
                    });
                }
            }
            return rez;
        }

        #endregion

        public static List<FullFields> UsePrime(List<Person> dateSel) // for Sankey
        {
            List<FullFields> rez = new();
            // Распределение первичных без учёта потерь
            string sql = "SELECT CCName, SUM(FactCost) as FactCost "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) AND IsERPrime = 1 AND IsCCMain = 1 "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "GROUP BY IdCC";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        CCName = q.GetString(0),
                        FactCost = q.GetDouble(1)
                    });
                }
                sql = "SELECT SUM(FactCost) as FactCost "
                                    + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) AND IsERPrime = 1 AND IsCCMain = 0 "
                                    + "AND Period IN " + Global.ListToSting(dateSel) + " "
                                    + "GROUP BY IsERPrime";
                selectCommand = new SqliteCommand(sql, db);
                q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        CCName = "прочие",
                        FactCost = q.GetDouble(0)
                    });
                }
            }
            return rez;
        }

        public static List<FullFields> UseSecondary(List<Person> dateSel)
        {
            List<FullFields> rez = new();
            // Распределение вторичных без учёта потерь
            string sql = "SELECT CCName, SUM(FactCost) as FactCost "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) AND IsERPrime = 0 AND IsCCMain = 1 AND IdCC <> 23 "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "GROUP BY IdCC";
            string sql1 = "SELECT SUM(FactCost) as FactCost "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) AND IsERPrime = 0 AND IsCCMain = 0 "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "GROUP BY IsERPrime";

            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        CCName = q.GetString(0),
                        FactCost = q.GetDouble(1)
                    });
                }

                selectCommand = new SqliteCommand(sql1, db);
                q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        CCName = "прочие",
                        FactCost = q.GetDouble(0)
                    });
                }

            }
            return rez;
        }

        #region Month
        public static List<FullFields> RetUseFromER(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            List<FullFields> rez = new();
            string sqlFilterFromPR = prSel == "все" ? "" : prSel == "на производство" ? "AND IsTechnology = 1" : "AND IsTechnology = 0";
            string sql = "SELECT Period, Kvart, IdCC, CCName, IsCCMain, IsCCTechnology, "
                + "IdER, ERName, ERShortName, IsERMain, IsERPrime, UnitName, "
                + "IdProduct, ProductName, "
                + "SUM(Fact) as Fact, SUM(Plan) as Plan, SUM(Diff) as Diff, SUM(FactCost) as FactCost, SUM(PlanCost) as PlanCost, SUM(DiffCost) as DiffCost, "
                + "IsNorm, IsTechnology, Produced "
                + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                + "AND Period IN " + Global.ListToSting(dateSel) + " "
                + "AND IdCC IN " + Global.ListToSting(ccSel) + " "
                + "AND IdER IN " + Global.ListToSting(erSel) + " "
                + sqlFilterFromPR + " "
                + "GROUP BY IdER "
                + "ORDER BY IdER";

            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new()
                    {
                        IdER = q.GetInt32(6),
                        ERName = q.GetString(7),
                        ERShortName = q.GetString(8),
                        IsERMain = q.GetBoolean(9),
                        IsERPrime = q.GetBoolean(10),
                        UnitName = q.GetString(11),
                        Fact = q.GetDouble(14),
                        Plan = q.GetDouble(15),
                        Diff = q.GetDouble(16),
                        FactCost = q.GetDouble(17),
                        PlanCost = q.GetDouble(18),
                        DiffCost = q.GetDouble(19),
                        DiffProc = q.GetDouble(15) > 0 ? q.GetDouble(16) * 100 / q.GetDouble(15) : 0
                    });
                }
            }
            return rez;
        }
        public static List<FullFields> RetUseFromCC(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            List<FullFields> rez = new();
            string sqlFilterFromPR = prSel == "все" ? "" : prSel == "на производство" ? "AND IsTechnology = 1" : "AND IsTechnology = 0";
            string sql = "SELECT Period, Kvart, IdCC, CCName, IsCCMain, IsCCTechnology, "
                            + "IdER, ERName, ERShortName, IsERMain, IsERPrime, UnitName, "
                            + "IdProduct, ProductName, "
                            + "SUM(Fact) as Fact, SUM(Plan) as Plan, SUM(Diff) as Diff, SUM(FactCost) as FactCost, SUM(PlanCost) as PlanCost, SUM(DiffCost) as DiffCost, "
                            + "IsNorm, IsTechnology, Produced "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "AND IdCC IN " + Global.ListToSting(ccSel) + " "
                            + "AND IdER IN " + Global.ListToSting(erSel) + " "
                            + sqlFilterFromPR + " "
                            + "GROUP BY IdCC "
                            + "ORDER BY IdER";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new FullFields
                    {
                        IdCC = q.GetInt32(2),
                        CCName = q.GetString(3),
                        IsCCMain = q.GetBoolean(4),
                        IsCCTechnology = q.GetBoolean(5),
                        IdER = q.GetInt32(6),
                        ERName = q.GetString(7),
                        ERShortName = q.GetString(8),
                        IsERMain = q.GetBoolean(9),
                        IsERPrime = q.GetBoolean(10),
                        UnitName = q.GetString(11),
                        Fact = q.GetDouble(14),
                        Plan = q.GetDouble(15),
                        Diff = q.GetDouble(16),
                        FactCost = q.GetDouble(17),
                        PlanCost = q.GetDouble(18),
                        DiffCost = q.GetDouble(19),
                        DiffProc = q.GetDouble(15) > 0 ? q.GetDouble(16) * 100 / q.GetDouble(15) : 0
                    });
                }
            }
            return rez;
        }
        public static List<FullFields> RetUseFromPR(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            List<FullFields> rez = new();
            string sqlFilterFromPR = prSel == "все" ? "" : prSel == "на производство" ? "AND IsTechnology = 1" : "AND IsTechnology = 0";
            string sql = "SELECT Period, Kvart, IdCC, CCName, IsCCMain, IsCCTechnology, "
                            + "IdER, ERName, ERShortName, IsERMain, IsERPrime, UnitName, "
                            + "IdProduct, ProductName, "
                            + "SUM(Fact) as Fact, SUM(Plan) as Plan, SUM(Diff) as Diff, SUM(FactCost) as FactCost, SUM(PlanCost) as PlanCost, SUM(DiffCost) as DiffCost, "
                            + "IsNorm, IsTechnology, Produced "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "AND IdCC IN " + Global.ListToSting(ccSel) + " "
                            + "AND IdER IN " + Global.ListToSting(erSel) + " "
                            + sqlFilterFromPR + " "
                            + "GROUP BY IdProduct "
                            + "ORDER BY IdProduct";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new FullFields
                    {
                        IdER = q.GetInt32(6),
                        ERName = q.GetString(7),
                        IdProduct = q.GetInt32(12),
                        ProductName = q.GetString(13),
                        UnitName = q.GetString(11),
                        Fact = q.GetDouble(14),
                        Plan = q.GetDouble(15),
                        Diff = q.GetDouble(16),
                        FactCost = q.GetDouble(17),
                        PlanCost = q.GetDouble(18),
                        DiffCost = q.GetDouble(19),
                        DiffProc = q.GetDouble(15) > 0 ? q.GetDouble(16) * 100 / q.GetDouble(15) : 0
                    });
                }
            }
            return rez;
        }
        public static List<FullFields> ERUseFromCC(int dateSel, int ccSel, int er)
        {
            List<FullFields> rez = new();
            string sql = "SELECT Period, Kvart, IdCC, CCName, IsCCMain, IsCCTechnology, "
                            + "IdER, ERName, ERShortName, IsERMain, IsERPrime, UnitName, "
                            + "IdProduct, ProductName, "
                            + "SUM(Fact) as Fact, SUM(Plan) as Plan, SUM(Diff) as Diff, SUM(FactCost) as FactCost, SUM(PlanCost) as PlanCost, SUM(DiffCost) as DiffCost, "
                            + "IsNorm, IsTechnology, Produced "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND Period = " + dateSel.ToString() + " "
                            + "AND IdCC = " + ccSel.ToString() + " "
                            + "AND IdER = " + er.ToString() + " "
                            + "GROUP BY IdProduct "
                            + "ORDER BY IdProduct";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new FullFields
                    {
                        IdER = q.GetInt32(6),
                        ERName = q.GetString(7),
                        IdProduct = q.GetInt32(12),
                        ProductName = q.GetString(13),
                        UnitName = q.GetString(11),
                        Fact = q.GetDouble(14),
                        Plan = q.GetDouble(15),
                        Diff = q.GetDouble(16),
                        FactCost = q.GetDouble(17),
                        PlanCost = q.GetDouble(18),
                        DiffCost = q.GetDouble(19),
                        DiffProc = q.GetDouble(15) > 0 ? q.GetDouble(16) * 100 / q.GetDouble(15) : 0
                    });
                }
            }
            return rez;
        }

        public static List<FullFields> RetUsePeriodFromER(List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            List<FullFields> rez = new();
            string sqlFilterFromPR = prSel == "все" ? "" : prSel == "на производство" ? "AND IsTechnology = 1" : "AND IsTechnology = 0";
            string sql = "SELECT Period, Kvart, IdCC, CCName, IsCCMain, IsCCTechnology, "
                            + "IdER, ERName, ERShortName, IsERMain, IsERPrime, UnitName, "
                            + "IdProduct, ProductName, "
                            + "SUM(Fact) as Fact, SUM(Plan) as Plan, SUM(Diff) as Diff, SUM(FactCost) as FactCost, "
                            + "SUM(PlanCost) as PlanCost, SUM(DiffCost) as DiffCost, "
                            + "IsNorm, IsTechnology, Produced "
                            + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND Period IN " + Global.ListToSting(dateSel) + " "
                            + "AND IdCC IN " + Global.ListToSting(ccSel) + " "
                            + "AND IdER IN " + Global.ListToSting(erSel) + " "
                            + sqlFilterFromPR + " "
                            + "GROUP BY Period "
                            + "ORDER BY Period";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    FullFields r = new FullFields();
                    r.Period = q.GetInt32(0);
                    r.Year = r.Period / 100;
                    r.Month = r.Period - r.Year * 100;
                    r.PeriodStr = r.Year + "_" + (r.Month < 10 ? "0" + r.Month : r.Month);
                    r.IdER = q.GetInt32(6);
                    r.ERName = q.GetString(7);
                    r.ERShortName = q.GetString(8);
                    r.IsERMain = q.GetBoolean(9);
                    r.IsERPrime = q.GetBoolean(10);
                    r.UnitName = q.GetString(11);
                    r.Fact = q.GetDouble(14);
                    r.Plan = q.GetDouble(15);
                    r.Diff = q.GetDouble(16);
                    r.FactCost = q.GetDouble(17);
                    r.PlanCost = q.GetDouble(18);
                    r.DiffCost = q.GetDouble(19);
                    r.DiffProc = r.Plan > 0 ? r.Diff * 100 / r.Plan : 0;
                    r.IsNorm = q.GetBoolean(20);
                    rez.Add(r);
                }
            }
            return rez;
        }
        #endregion

        #region Day
        public static List<FullFields> RetUseDayFromER(List<Person> ccSel, List<Person> erSel)
        {
            List<FullFields> rez = new();
            string sql = "SELECT Period, IdCC, CCName, IsCCMain, IsTechnology, "
                + "IdER, ERName, IsERMain, IsERPrime, UnitName, "
                + "SUM(Fact) as Fact, SUM(Plan) as Plan, SUM(Diff) as Diff, SUM(FactCost) as FactCost, SUM(PlanCost) as PlanCost, SUM(DiffCost) as DiffCost "
                + "FROM CurrentUseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                + "AND IdCC IN " + Global.ListToSting(ccSel) + " "
                + "AND IdER IN " + Global.ListToSting(erSel) + " "
                + "GROUP BY IdER "
                + "ORDER BY IdER";

            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new FullFields
                    {
                        IdER = q.GetInt32(5),
                        ERName = q.GetString(6),
                        IsERMain = q.GetBoolean(7),
                        IsERPrime = q.GetBoolean(8),
                        UnitName = q.GetString(9),
                        Fact = q.GetDouble(10),
                        Plan = q.GetDouble(11),
                        Diff = q.GetDouble(12),
                        FactCost = q.GetDouble(13),
                        PlanCost = q.GetDouble(14),
                        DiffCost = q.GetDouble(15),
                        DiffProc = q.GetDouble(11) > 0 ? q.GetDouble(12) * 100 / q.GetDouble(11) : 0
                    });
                }
            }
            return rez;
        }
        public static List<FullFields> RetUseDayFromCC(List<Person> ccSel, List<Person> erSel)
        {
            List<FullFields> rez = new();
            string sql = "SELECT Period, IdCC, CCName, IsCCMain, IsTechnology, "
                            + "IdER, ERName, IsERMain, IsERPrime, UnitName, "
                            + "SUM(Fact) as Fact, SUM(Plan) as Plan, SUM(Diff) as Diff, SUM(FactCost) as FactCost, SUM(PlanCost) as PlanCost, SUM(DiffCost) as DiffCost "
                            + "FROM CurrentUseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                            + "AND IdCC IN " + Global.ListToSting(ccSel) + " "
                            + "AND IdER IN " + Global.ListToSting(erSel) + " "
                            + "GROUP BY IdCC "
                            + "ORDER BY IdER";
            using (SqliteConnection db = new($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    rez.Add(new FullFields
                    {
                        IdCC = q.GetInt32(1),
                        CCName = q.GetString(2),
                        IsCCMain = q.GetBoolean(3),
                        IsCCTechnology = q.GetBoolean(4),
                        IdER = q.GetInt32(5),
                        ERName = q.GetString(6),
                        IsERMain = q.GetBoolean(7),
                        IsERPrime = q.GetBoolean(8),
                        UnitName = q.GetString(9),
                        Fact = q.GetDouble(10),
                        Plan = q.GetDouble(11),
                        Diff = q.GetDouble(12),
                        FactCost = q.GetDouble(13),
                        PlanCost = q.GetDouble(14),
                        DiffCost = q.GetDouble(15),
                        DiffProc = q.GetDouble(11) > 0 ? q.GetDouble(12) * 100 / q.GetDouble(11) : 0
                    });
                }
            }
            return rez;
        }
        #endregion
        public static List<FullFields> RetUseCompare(ChartDataType argType, List<Person> dateSel, List<Person> ccSel, List<Person> erSel, string prSel)
        {
            string sqlFilterFromPR = prSel == "все" ? "" : prSel == "на производство" ? "AND IsTechnology = 1" : "AND IsTechnology = 0";
            string arg = default;
            if (argType == ChartDataType.ER)
            {
                arg = "IdER";
            }
            else if (argType == ChartDataType.CC)
            {
                arg = "IdCC";
            }
            else if (argType == ChartDataType.Period)
            {
                arg = "Period";
            }

            List<FullFields> currList = new();
            List<FullFields> prevList = new();
            List<Person> datePrev = new();
            foreach (Person r in dateSel)
            {
                int curPeriod = r.Id;
                if (curPeriod <= Models.Period.FirstPeriod)
                {
                    return null;
                }
                int curYear = curPeriod / 100;
                int prevYear = curYear - 1;
                int curMonth = curPeriod - curYear * 100;
                int prevPeriod = prevYear * 100 + curMonth;
                Person n = new();
                n.Id = prevPeriod;
                datePrev.Add(n);
            }
            List<FullFields> rez = new();
            string sql =
                "SELECT s.Period, s.IdCC, s.CCName, s.IdER, s.ERName, s.UnitName, SUM(s.dLimit) as dLimit, SUM(s.dNorm) as dNorm, SUM(s.dFact) as dFact, "
                + "SUM(s.dLimitCost) as dLimitCost, SUM(s.dNormCost) as dNormCost, SUM(s.dSumCost) as dSumCost, s.ERShortName, s.IsNorm, s.IsERPrime, s.IsCCTechnology "
                + "FROM "
                + "(SELECT  c.Period as Period, c.IdCC as IdCC, c.CCName as CCName, c.IsCCTechnology as IsCCTechnology, c.IdER as IdER, c.IsERPrime as IsERPrime, c.ERName as ERName, c.ERShortName as ERShortName, "
                + "c.UnitName as UnitName, c.IsNorm as IsNorm, "
                + "ROUND(SUM(CASE WHEN c.IsNorm = 0 THEN c.Fact - p.Fact ELSE 0 END), 0) as dLimit, "
                + "ROUND(SUM(CASE WHEN c.IsNorm = 1 THEN c.Fact - p.Fact / p.Produced * c.Produced ELSE 0 END), 0) as dNorm, "
                + "ROUND((SUM(CASE WHEN c.IsNorm = 0 THEN c.Fact - p.Fact ELSE 0 END) + SUM(CASE WHEN c.IsNorm = 1 THEN c.Fact - p.Fact / p.Produced * c.Produced ELSE 0 END)), 0) as dFact, "
                + "ROUND(SUM(CASE WHEN c.IsNorm = 0 THEN c.FactCost - p.FactCost ELSE 0 END), 0) as dLimitCost, "
                + "ROUND(SUM(CASE WHEN c.IsNorm = 1 THEN c.FactCost - p.FactCost / p.Produced * c.Produced ELSE 0 END), 0) as dNormCost, "
                + "ROUND((SUM(CASE WHEN c.IsNorm = 0 THEN c.FactCost - p.FactCost ELSE 0 END) + SUM(CASE WHEN c.IsNorm = 1 THEN c.FactCost - p.FactCost / p.Produced * c.Produced ELSE 0 END)), 0) as dSumCost "
                + "FROM "
                + "(SELECT "
                + "   Period, (Period / 100) as Year, (Period - ((Period / 100) * 100)) as Month, "
                + "   IdCC, CCName, IdER, ERName, ERShortName, UnitName, IdProduct, "
                + "   SUM(Fact) as Fact, SUM(FactCost) as FactCost, SUM(Produced) as Produced, IsNorm, IsERPrime, IsCCTechnology "
                + "FROM UseAllCosts "
                + "WHERE "
                + "   NOT(IdCC == 56 AND IdER == 966) "
                + "   AND Period IN" + Global.ListToSting(dateSel) + " "
                + "   AND IdCC IN" + Global.ListToSting(ccSel) + " "
                + "   AND IdER IN" + Global.ListToSting(erSel) + " "
                + sqlFilterFromPR + " "
                + "GROUP BY Period, IdCC, IdER, IdProduct) c "
                + "JOIN "
                + "(SELECT "
                + "   Period, (Period / 100) as Year, (Period - ((Period / 100) * 100)) as Month, "
                + "   IdCC, CCName, IdER, ERName, ERShortName, UnitName, IdProduct, "
                + "   SUM(Fact) as Fact, SUM(FactCost) as FactCost, SUM(Produced) as Produced, IsNorm, IsERPrime, IsCCTechnology "
                + "FROM UseAllCosts "
                + "WHERE "
                + "   NOT(IdCC == 56 AND IdER == 966) "
                + "   AND Period IN" + Global.ListToSting(datePrev) + " "
                + "   AND IdCC IN" + Global.ListToSting(ccSel) + " "
                + "   AND IdER IN" + Global.ListToSting(erSel) + " "
                + sqlFilterFromPR + " "
                + "GROUP BY Period, IdCC, IdER, IdProduct) p "
                + "ON c.Year = p.Year + 1 AND c.Month = p.Month AND c.IdCC = p.IdCC AND c.IdER = p.IdER AND c.IdProduct = p.IdProduct "
                + "GROUP BY c.Period, c.IdCC, c.IdER) s "
                + "GROUP BY s." + arg;
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                SqliteCommand selectCommand = new(sql, db);
                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    FullFields r = new FullFields();
                    r.Period = q.GetInt32(0);
                    r.Year = r.Period / 100;
                    r.Month = r.Period - r.Year * 100;
                    r.PeriodStr = r.Year + "_" + (r.Month < 10 ? "0" + r.Month : r.Month);
                    r.IdCC = q.GetInt32(1);
                    r.CCName = q.GetString(2);
                    r.IsCCTechnology = q.GetBoolean(15);
                    r.IdER = q.GetInt32(3);
                    r.ERName = q.GetString(4);
                    r.ERShortName = q.GetString(12);
                    r.IsERPrime = q.GetBoolean(14);
                    r.UnitName = q.GetString(5);
                    r.dFactLimit = q.GetDouble(6);
                    r.dFactNormative = q.GetDouble(7);
                    r.dFact = q.GetDouble(8);
                    r.dFactLimitCost = q.GetDouble(9);
                    r.dFactNormativeCost = q.GetDouble(10);
                    r.dFactCost = q.GetDouble(11);
                    r.IsNorm = q.GetBoolean(13);
                    rez.Add(r);
                }
            }
            return rez;
        }
    }
}
