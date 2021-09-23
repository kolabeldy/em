using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using em.DBAccess;

namespace em.Models
{
    class Report
    {
        public string PeriodStr { get; set; }
        public int IdCC { get; set; }
        public string CCName { get; set; }
        public string TypeCC { get; set; }
        public string GroupCC { get; set; }
        public int IdER { get; set; }
        public string ERName { get; set; }
        public string ERNameFull { get; set; }
        public string ERCodeName { get; set; }
        public string TypeER { get; set; }
        public string GroupER { get; set; }
        public string Unit { get; set; }
        public int IdProduct { get; set; }
        public string ProductName { get; set; }
        public string ProductCodeName { get; set; }
        public string TypeNorm { get; set; }
        public string TypeUse { get; set; }
        public double Fact { get; set; }
        public double Plan { get; set; }
        public double Diff { get; set; }
        public double FactCost { get; set; }
        public double PlanCost { get; set; }
        public double DiffCost { get; set; }


        public static bool ReportMonthShow()
        {
            bool rez = true;
            string inputpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template/emReportMonth.xltm");
            string outputpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report/ОтчетМес.xlsm");

            Excel.Application application = null;
            Excel.Workbooks workbooks = null;
            Excel.Workbook workbook = null;
            Excel.Sheets worksheets = null;
            Excel.Worksheet worksheet = null;
            Excel.Range c1 = null;
            Excel.Range c2 = null;
            try
            {
                application = new Excel.Application
                {
                    Visible = false
                };

                application.ScreenUpdating = true;

                workbooks = application.Workbooks;
                workbook = application.Workbooks.Open(inputpath,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                  Type.Missing, Type.Missing);

                worksheets = workbook.Worksheets; //получаем доступ к коллекции рабочих листов
                dynamic dynamic = worksheets.Item[1] as dynamic;
                worksheet = dynamic;//получаем доступ к первому листу

                object[,] arrRes = GetArrReports();

                c1 = (Excel.Range)worksheet.Cells[2, 1];
                c2 = (Excel.Range)worksheet.Cells[arrRes.GetLength(0) + 1, arrRes.GetLength(1)];
                Excel.Range rangeCaption = worksheet.get_Range(c1, c2);
                rangeCaption.Value = arrRes;

                application.Run((object)"RefreshAll");
                //worksheet = worksheets.Item[2];
                //worksheet = workbook.ActiveSheet();
                //application.ScreenUpdating = true;
                application.Visible = true;
            }
            finally
            {
                //освобождаем память, занятую объектами
                //Marshal.ReleaseComObject(cell);
                //Marshal.ReleaseComObject(worksheet);
                //Marshal.ReleaseComObject(worksheets);
                //Marshal.ReleaseComObject(workbook);
                //Marshal.ReleaseComObject(workbooks);
                //Marshal.ReleaseComObject(application);
            }
            return rez;

        }
        private static object[,] GetArrReports()
        {
            List<Report> reports = new List<Report>();

            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT Period, Kvart, IdCC, CCName, IsCCMain, IsCCTechnology, "
                                + "IdER, ERName, ERShortName, IsERMain, IsERPrime, UnitName, "
                                + "IdProduct, ProductName, "
                                + "Fact, Plan, Diff, FactCost, PlanCost, DiffCost, "
                                + "IsNorm, IsTechnology "
                                + "FROM UseAllCosts WHERE NOT(IdCC == 56 AND IdER == 966) "
                                + "AND Period = 202101";
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    Report r = new Report();
                    int period = q.GetInt32(0);
                    int year = period / 100;
                    int month = period - year * 100;
                    r.PeriodStr = year + "_" + (month < 10 ? "0" + month : month);
                    r.IdCC = q.GetInt32(2);
                    r.CCName = q.GetString(3);
                    r.GroupCC = q.GetBoolean(4) ? "основные" : "прочие";
                    r.TypeCC = q.GetBoolean(5) ? "технологические" : "вспомогательные";
                    r.IdER = q.GetInt32(6);
                    r.ERName = q.GetString(7);
                    r.ERCodeName = r.IdER + "_" + r.ERName;
                    r.Unit = q.GetString(11);
                    r.ERNameFull = r.ERCodeName + ", " + r.Unit;
                    r.GroupER = q.GetBoolean(9) ? "основные" : "прочие";
                    r.TypeER = q.GetBoolean(10) ? "первичные" : "вторичные";
                    r.IdProduct = q.GetInt32(12);
                    r.ProductName = q.GetString(13);
                    r.ProductCodeName = r.IdProduct + "_" + r.ProductName;
                    r.Fact = q.GetDouble(14);
                    r.Plan = q.GetDouble(15);
                    r.Diff = q.GetDouble(16);
                    r.FactCost = q.GetDouble(17);
                    r.PlanCost = q.GetDouble(18);
                    r.DiffCost = q.GetDouble(19);
                    r.TypeNorm = q.GetBoolean(20) ? "нормируемые" : "лимитируемые";
                    r.TypeUse = q.GetBoolean(21) ? "на технологию" : "общецеховые";
                    reports.Add(r);
                }

                object[,] arrRes = new object[reports.Count, 23];
                for (int i = 0; i < reports.Count; i++)
                {
                    arrRes[i, 0] = reports[i].PeriodStr;
                    arrRes[i, 1] = reports[i].IdCC;
                    arrRes[i, 2] = reports[i].CCName;
                    arrRes[i, 3] = reports[i].GroupCC;
                    arrRes[i, 4] = reports[i].TypeCC;
                    arrRes[i, 5] = reports[i].IdER;
                    arrRes[i, 6] = reports[i].ERName;
                    arrRes[i, 7] = reports[i].ERNameFull;
                    arrRes[i, 8] = reports[i].ERCodeName;
                    arrRes[i, 9] = reports[i].TypeER;
                    arrRes[i, 10] = reports[i].GroupER;
                    arrRes[i, 11] = reports[i].Unit;
                    arrRes[i, 12] = reports[i].IdProduct;
                    arrRes[i, 13] = reports[i].ProductName;
                    arrRes[i, 14] = reports[i].ProductCodeName;
                    arrRes[i, 15] = reports[i].TypeNorm;
                    arrRes[i, 16] = reports[i].TypeUse;
                    arrRes[i, 17] = reports[i].Fact;
                    arrRes[i, 18] = reports[i].Plan;
                    arrRes[i, 19] = reports[i].Diff;
                    arrRes[i, 20] = reports[i].FactCost;
                    arrRes[i, 21] = reports[i].PlanCost;
                    arrRes[i, 22] = reports[i].DiffCost;

                }

                return arrRes;
            }


        }
    }
}
