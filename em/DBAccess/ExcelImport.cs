using em.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.XlsIO;
using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using Syncfusion.Office;

namespace em.DBAccess
{
    public class ExcelImport
    {
        public static bool ImportMothDataFromExcel()
        {

            List<ERUse> lastMonthUse = new List<ERUse>();
            List<ERUse> rezTableUse = new List<ERUse>();

            //Create an instance of ExcelEngine
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            string opFile = "";
            if (openFileDialog.ShowDialog() == true)
                opFile = openFileDialog.FileName;
            else return false;
            FileStream inputStream = new FileStream(opFile, FileMode.Open);
            IWorkbook workbook = excelEngine.Excel.Workbooks.Open(inputStream);
            IWorksheet worksheet = workbook.Worksheets[0];

            string textControl = worksheet.Range[1, 9].Text;
            if (textControl != "FAH0201E")
            {
                MessageBox.Show("Этот файл не является источником записей базы данных.");
                return false;
            }

            int exportYear = Convert.ToInt32(worksheet.Range[4, 1].Text.Substring(3));
            int exportMonth = Convert.ToInt32(worksheet.Range[4, 1].Text.Substring(0, 2));
            int prevPeriod = exportMonth > 1 ? exportYear * 100 + exportMonth - 1 : (exportYear - 1) * 100 + 12;
            if (prevPeriod != Period.LastPeriod)
            {
                int lastYear = Period.LastPeriod / 100;
                int lastMonth = Period.LastPeriod - lastYear * 100;
                int nextYear = lastMonth < 12 ? lastYear : lastYear + 1;
                int nextMonth = lastMonth < 12 ? lastMonth + 1 : 1;
                MessageBox.Show(String.Format("Не найдены записи за требуемый период: {0}", nextYear.ToString() + "_" + nextMonth.ToString("00")));
                return false;

            }

            for (int i = 1; i < 16; i++)
            {
                worksheet.Range[3, i].Text = i.ToString();
            }
            Dictionary<string, string> mappingProperties = new Dictionary<string, string>();
            mappingProperties.Add("1", "Period");
            mappingProperties.Add("2", "IdERProducer");
            mappingProperties.Add("3", "IdER");
            mappingProperties.Add("5", "IdOrg");
            mappingProperties.Add("6", "IdCC");
            mappingProperties.Add("7", "IdProduct");
            mappingProperties.Add("8", "ProductName");
            mappingProperties.Add("10", "ERFact");
            mappingProperties.Add("11", "ERPlan");
            mappingProperties.Add("12", "ERNormFact");
            mappingProperties.Add("13", "ERNormPlan");
            mappingProperties.Add("14", "Produced");

            int usedRowCount = 3;
            while (worksheet.Range[usedRowCount + 1, 1].Text != null)
            {
                usedRowCount++;
            }

            List<TempUseERTable> exportList = worksheet.ExportData<TempUseERTable>(3, 1, usedRowCount, 15, mappingProperties);

            workbook.Close();
            excelEngine.Dispose();

            foreach (var r in exportList)
            {
                ERUse n = new ERUse();
                n.Year = Convert.ToInt32(r.Period.Substring(3));
                n.Month = Convert.ToInt32(r.Period.ToString().Substring(0, 2));
                n.Period = n.Year * 100 + n.Month;
                n.IdERProducer = Convert.ToInt32(r.IdERProducer);
                n.IdER = Convert.ToInt32(r.IdER);
                n.IdOrg = Convert.ToInt32(r.IdOrg);
                n.IdCC = Convert.ToInt32(r.IdCC);
                n.IdProduct = Convert.ToInt32(r.IdProduct);
                n.ProductName = r.ProductName;
                n.ERFact = Convert.ToDouble(r.ERFact);
                n.ERPlan = Convert.ToDouble(r.ERPlan);
                n.ERNormFact = Convert.ToDouble(r.ERNormFact);
                n.ERNormPlan = Convert.ToDouble(r.ERNormPlan);
                n.Produced = Convert.ToDouble(r.Produced);

                n.IdGroup = n.IdER == 990 ? -990 : n.IdER == 28462 ? -990 : n.IdER == 54814 ? -990 : n.IdER;
                n.Kvart = n.Month < 4 ? 1 : n.Month < 7 ? 2 : n.Month < 10 ? 3 : 4;
                n.Season = (n.Month >= 4 & n.Month <= 9) ? 2 : 1;
                n.IsNorm = (r.ERNormPlan == null && r.ERNormFact == null) ? false : true;
                n.IsTechnology = (n.IsNorm || r.IdProduct == "15633") ? true : false;

                lastMonthUse.Add(n);
            }
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {

                    foreach (ERUse r in lastMonthUse)
                    {
                        ERUse n = new ERUse();
                        n.Year = r.Year;
                        n.Month = r.Month;
                        n.Period = r.Period;
                        n.IdERProducer = r.IdERProducer;
                        n.IdER = r.IdER;
                        n.IdOrg = r.IdOrg;
                        n.IdCC = r.IdCC;
                        n.IdProduct = r.IdProduct;
                        n.ProductName = r.ProductName;
                        n.IsNorm = r.IsNorm;
                        n.IsTechnology = r.IsTechnology;
                        n.Produced = r.Produced;
                        n.ERFact = r.ERFact;
                        n.ERPlan = r.ERPlan;
                        n.ERNormFact = r.ERNormFact;
                        n.ERNormPlan = r.ERNormPlan;
                        n.IdGroup = r.IdGroup;
                        if (n.IdGroup == -990)
                        {
                            n.SumFact = (from o in lastMonthUse
                                         where o.IdGroup == n.IdGroup && o.IdCC == n.IdCC && o.IdProduct == n.IdProduct
                                         group o by new { o.IdCC, o.IdProduct, o.IdGroup } into gr
                                         select new
                                         {
                                             SumFact = gr.Sum(m => m.ERFact)
                                         }).ToList()[0].SumFact;
                            n.SumPlan = (from o in lastMonthUse
                                         where o.IdGroup == n.IdGroup && o.IdCC == n.IdCC && o.IdProduct == n.IdProduct
                                         group o by new { o.IdCC, o.IdProduct, o.IdGroup } into gr
                                         select new
                                         {
                                             SumPlan = gr.Sum(m => m.ERPlan)
                                         }).ToList()[0].SumPlan;
                            n.ERPlanCorrected = n.SumFact != 0 ? n.ERFact / n.SumFact * n.SumPlan : n.ERPlan;
                        }
                        else
                        {
                            n.SumFact = n.ERFact;
                            n.SumPlan = n.ERPlan;
                            n.ERPlanCorrected = n.ERPlan;
                        }
                        n.Season = r.Season;
                        n.Kvart = r.Kvart;
                        rezTableUse.Add(n);
                        string sqlText = String.Format("INSERT INTO ERUses (Year, Month, Period, Kvart, Season, IdOrg, IdCC, "
                            + "IdProduct, ProductName, Produced, IdER, IdGroup, IdERProducer, ERFact, ERPlan, ERPlanCorrected, "
                            + "ERNormFact, ERNormPlan, SumFact, SumPlan, IsNorm, IsTechnology) "
                            + "VALUES ({0},{1},{2},{3},{4},{5},{6},{7},'{8}',{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})",
                            n.Year, n.Month, n.Period, n.Kvart, n.Season, n.IdOrg, n.IdCC,
                            n.IdProduct, n.ProductName, n.Produced.ToString().Replace(",", "."),
                            n.IdER, n.IdGroup, n.IdERProducer, n.ERFact.ToString().Replace(",", "."),
                            n.ERPlan.ToString().Replace(",", "."), n.ERPlanCorrected.ToString().Replace(",", "."),
                            n.ERNormFact.ToString().Replace(",", "."), n.ERNormPlan.ToString().Replace(",", "."),
                            n.SumFact.ToString().Replace(",", "."), n.SumPlan.ToString().Replace(",", "."), n.IsNorm ? 1 : 0, n.IsTechnology ? 1 : 0);

                        var insertCmd = db.CreateCommand();
                        insertCmd.CommandText = sqlText;
                        insertCmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                db.Close();
            }
            MessageBox.Show(string.Format("В базу данных загружено {0} записей.", lastMonthUse.Count.ToString()));
            return true;

        }
        public static bool ImportDayDataFromExcel()
        {

            List<ERUse> lastMonthUse = new List<ERUse>();
            List<ERUse> rezTableUse = new List<ERUse>();

            //Create an instance of ExcelEngine
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            //Instantiates the File Picker. 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string opFile = "";
            if (openFileDialog.ShowDialog() == true)
                opFile = openFileDialog.FileName;
            else return false;
            FileStream inputStream = new FileStream(opFile, FileMode.Open);
            IWorkbook workbook = excelEngine.Excel.Workbooks.Open(inputStream);
            IWorksheet worksheet = workbook.Worksheets[0];

            string textControl = worksheet.Range[1, 14].Text;
            if (textControl != "FAH5237E")
            {
                MessageBox.Show("Этот файл не является источником записей базы данных.");
                return false;
            }

            for (int i = 1; i < 10; i++)
            {
                worksheet.Range[4, i].Text = i.ToString();
            }
            Dictionary<string, string> mappingProperties = new Dictionary<string, string>();
            mappingProperties.Add("1", "Period");
            mappingProperties.Add("2", "IdOrg");
            mappingProperties.Add("3", "IdCC");
            mappingProperties.Add("6", "ERPlan");
            mappingProperties.Add("7", "ERFact");
            mappingProperties.Add("9", "IdER");

            int usedRowCount = 4;
            while (worksheet.Range[usedRowCount + 1, 1].Text != null)
            {
                usedRowCount++;
            }

            List<TempUseERTable> exportList = worksheet.ExportData<TempUseERTable>(4, 1, usedRowCount, 9, mappingProperties);

            workbook.Close();
            excelEngine.Dispose();
            List<CurrentERUse> currTableUse = new List<CurrentERUse>();
            int reccount = 0;
            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();
                string sqlText = "Delete FROM CurrentERUses";

                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = sqlText;
                deleteCommand.ExecuteReader();

                bool isFound = false;
                using (var transaction = db.BeginTransaction())
                {
                    foreach (var r in exportList)
                    {
                        if (r.IdCC == "002" || isFound)
                        {
                            CurrentERUse n = new CurrentERUse();
                            n.Year = Convert.ToInt32("20" + r.Period.Substring(6, 2));
                            n.Month = Convert.ToInt32(r.Period.Substring(3, 2));
                            n.Period = r.Period;
                            n.IdER = Convert.ToInt32(r.IdER);
                            n.IdOrg = Convert.ToInt32(r.IdOrg);
                            n.IdCC = Convert.ToInt32(r.IdCC);
                            if (r.ERFact == "") n.ERFact = 0;
                            else if (r.ERFact.Substring(0, 1) != "-")
                                n.ERFact = Convert.ToDouble(r.ERFact);
                            else n.ERFact = -Convert.ToDouble(r.ERFact.Substring(1, r.ERFact.Length - 2));
                            n.ERPlan = r.ERPlan == "" ? 0 : Convert.ToDouble(r.ERPlan);
                            n.Season = (n.Month >= 4 & n.Month <= 9) ? 2 : 1;
                            currTableUse.Add(n);
                            isFound = true;

                            sqlText = String.Format("INSERT INTO CurrentERUses (Year, Month, Period, Season, IdOrg, IdCC, IdER, ERFact, ERPlan) "
                                + "VALUES ({0},{1},'{2}',{3},{4},{5},{6},{7},{8})",
                                n.Year, n.Month, n.Period, n.Season, n.IdOrg, n.IdCC, n.IdER,
                                n.ERFact.ToString().Replace(",", "."), n.ERPlan.ToString().Replace(",", "."));

                            var insertCmd = db.CreateCommand();

                            insertCmd.CommandText = sqlText;
                            insertCmd.ExecuteNonQuery();
                            reccount++;
                        }
                    }
                    transaction.Commit();
                }
                db.Close();
            }
            MessageBox.Show(string.Format("В базу данных загружено {0} записей.", reccount.ToString()));
            return true;
        }
        public static bool ImportLossesDataFromExcel()
        {
            int lastLossPeriod = Period.LastPeriodLosses;
            List<FactLosse> factLosses = new List<FactLosse>();

            //Create an instance of ExcelEngine
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            string opFile = "";
            if (openFileDialog.ShowDialog() == true)
                opFile = openFileDialog.FileName;
            else return false;
            FileStream inputStream = new FileStream(opFile, FileMode.Open);
            IWorkbook workbook = excelEngine.Excel.Workbooks.Open(inputStream);
            IWorksheet worksheet = workbook.Worksheets[0];

            string textControl = worksheet.Range[1, 16].Text;
            if (textControl != "FAH0511E")
            {
                MessageBox.Show("Этот файл не является источником записей базы данных.");
                return false;
            }

            int exportYear = Convert.ToInt32(worksheet.Range[5, 1].Text.Substring(6, 4));
            int exportMonth = Convert.ToInt32(worksheet.Range[5, 1].Text.Substring(3, 2));
            int prevPeriod = exportMonth > 1 ? exportYear * 100 + exportMonth - 1 : (exportYear - 1) * 100 + 12;
            if (prevPeriod != lastLossPeriod)
            {
                int lastYear = lastLossPeriod / 100;
                int lastMonth = lastLossPeriod - lastYear * 100;
                int nextYear = lastMonth < 12 ? lastYear : lastYear + 1;
                int nextMonth = lastMonth < 12 ? lastMonth + 1 : 1;
                //int nextPeriod = nextYear * 100 + nextMonth;

                MessageBox.Show(String.Format("Не найдены записи за требуемый период: {0}", nextYear.ToString() + "_" + nextMonth.ToString("00")));
                return false;
            }

            for (int i = 1; i < 17; i++)
            {
                worksheet.Range[4, i].Text = i.ToString();
            }
            Dictionary<string, string> mappingProperties = new Dictionary<string, string>();
            mappingProperties.Add("1", "Period");
            mappingProperties.Add("3", "IdCC");
            mappingProperties.Add("7", "ERFact");
            mappingProperties.Add("16", "IdER");

            int usedRowCount = 4;
            while (worksheet.Range[usedRowCount + 1, 1].Text != null)
            {
                usedRowCount++;
            }

            List<TempUseERTable> exportList = worksheet.ExportData<TempUseERTable>(4, 1, usedRowCount, 16, mappingProperties);

            workbook.Close();
            excelEngine.Dispose();

            using (SqliteConnection db = new SqliteConnection($"Filename={DataAccess.dbpath}"))
            {
                db.Open();

                foreach (var r in exportList)
                {
                    FactLosse n = new FactLosse();
                    n.Year = exportYear;
                    n.Month = exportMonth;
                    n.Kvart = n.Month < 4 ? 1 : n.Month < 7 ? 2 : n.Month < 10 ? 3 : 4;
                    n.Period = n.Year * 100 + n.Month;
                    n.IdER = Convert.ToInt32(r.IdER);
                    n.IdCC = Convert.ToInt32(r.IdCC);
                    n.Fact = Convert.ToDouble(r.ERFact);
                    factLosses.Add(n);

                    string sqlText = String.Format("INSERT INTO FactLosses (Year, Month, Period, Kvart, IdCC, IdER, Fact) "
                        + "VALUES ({0},{1},{2},{3},{4},{5},{6})", n.Year, n.Month, n.Period, n.Kvart, n.IdCC, n.IdER, n.Fact.ToString().Replace(",", "."));

                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    insertCommand.CommandText = sqlText;
                    insertCommand.ExecuteReader();
                }
                db.Close();
            }
            MessageBox.Show(string.Format("В базу данных загружено {0} записей.", factLosses.Count.ToString()));
            return true;
        }
        public static List<TempUseERTable> ImportNormsFromExcel()
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            string opFile = "";
            if (openFileDialog.ShowDialog() == true)
                opFile = openFileDialog.FileName;
            else return null;

            FileStream inputStream = new FileStream(opFile, FileMode.Open);
            IWorkbook workbook = excelEngine.Excel.Workbooks.Open(inputStream);
            IWorksheet worksheet = workbook.Worksheets[0];

            string textControl = worksheet.Range[1, 11].Text;
            if (textControl != "FAH1001E")
            {
                MessageBox.Show("Этот файл не является источником записей базы данных.");
                return null;
            }

            for (int i = 1; i < 15; i++)
            {
                worksheet.Range[4, i].Text = i.ToString();
            }
            Dictionary<string, string> mappingProperties = new Dictionary<string, string>();
            mappingProperties.Add("1", "IdCC");
            mappingProperties.Add("2", "IdProduct");
            mappingProperties.Add("5", "IdER");
            mappingProperties.Add("7", "NormAverage");
            mappingProperties.Add("8", "NormWinter");
            mappingProperties.Add("9", "NormSummer");
            mappingProperties.Add("11", "UnitName");
            mappingProperties.Add("12", "DateStart");
            mappingProperties.Add("13", "DateEnd");
            mappingProperties.Add("14", "IdERProducer");
            int usedRowCount = 4;
            while (worksheet.Range[usedRowCount + 1, 1].Text != null)
            {
                usedRowCount++;
            }

            List<TempUseERTable> exportList = worksheet.ExportData<TempUseERTable>(4, 1, usedRowCount, 14, mappingProperties);
            workbook.Close();
            excelEngine.Dispose();

            return exportList;
        }

        private class TempUse
        {
            public int IdCC { get; set; }
            public string TypeCC { get; set; }
            public double Fact { get; set; }

            public TempUse(int idcc, string typecc, double fact)
            {
                IdCC = idcc;
                TypeCC = typecc;
                Fact = fact;
            }
        }
        public static bool ReportMonthShow()
        {
            bool rez = true;

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;

                FileStream inputStream = new FileStream("template/emReportMonth.xlsx", FileMode.Open);
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open(inputStream);
                IWorksheet worksheet = workbook.Worksheets[0];

                IList<TempUse> reports = GetSalesReports();
                worksheet.InsertRow(2, reports.Count - 1, ExcelInsertOptions.FormatDefault);

                worksheet.ImportData(reports, 2, 1, false);

                ////Creating Vba project
                //IVbaProject project = workbook.VbaProject;

                ////Accessing vba modules collection
                //IVbaModules vbaModules = project.Modules;

                ////Accessing sheet module
                //IVbaModule vbaModule = vbaModules[worksheet.CodeName];

                ////Adding vba code to the module
                //vbaModule.Code = "Sub MacroRefresh\n RefreshAll() \n End Sub";
                //vbaModules.Clear();

                FileStream outputStream = new FileStream("report/МесячныйОтчёт.xlsx", FileMode.Create);



                workbook.SaveAs(outputStream);
                workbook.Close();
                excelEngine.ThrowNotSavedOnDestroy = true;
                excelEngine.Dispose();

            }

            return rez;

        }
        public static void RemoveSheet(int id)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {

                FileStream inputStream = new FileStream("report/МесячныйОтчёт.xlsx", FileMode.Open);
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open(inputStream);
                workbook.Worksheets[id].Remove();
                workbook.SaveAs(inputStream);
                workbook.Close();
            }

        }
        private static List<TempUse> GetSalesReports()
        {
            List<TempUse> reports = new List<TempUse>();
            reports.Add(new TempUse(2, "технологические", 50));
            reports.Add(new TempUse(16, "технологические", 63));
            reports.Add(new TempUse(23, "технологические", 115));
            reports.Add(new TempUse(56, "технологические", 74));
            reports.Add(new TempUse(61, "технологические", 28));
            reports.Add(new TempUse(62, "технологические", 25));
            reports.Add(new TempUse(71, "технологические", 80));
            reports.Add(new TempUse(110, "технологические", 250));
            reports.Add(new TempUse(501, "технологические", 90));
            reports.Add(new TempUse(21, "вспомогательные", 15));
            reports.Add(new TempUse(29, "вспомогательные", 10));
            reports.Add(new TempUse(45, "вспомогательные", 8));
            return reports;
        }
    }
}
