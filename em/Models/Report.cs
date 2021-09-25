using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using em.DBAccess;
using System.Drawing;
using System.Linq;
using em.ServicesPages;
using System.Threading.Tasks;

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

            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
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

        public static async void UniversalForm(int period, int numericStartPozition) // факторный анализ
        {
            ProcessIndicator ind = ProcessIndicator.GetInstance();
            ind.Show();


            List<FullFields> gridData = new List<FullFields>();
            List<FullFields> totalData = new List<FullFields>();


            int len2 = 5;
            string[] arrHeaders = new string[10];
            arrHeaders[0] = "Продукт";
            arrHeaders[1] = "Факт";
            arrHeaders[2] = "План";
            arrHeaders[3] = "Откл.";
            arrHeaders[4] = "Откл.%";
            arrHeaders[5] = "Пр.";
            arrHeaders[6] = "Классификатор";
            arrHeaders[7] = "Признак";
            arrHeaders[8] = "Отклонение";
            arrHeaders[9] = "Пояснение причины отклонения от РНЭ/лимитов";


            List<EResource> qry = EResource.ToList(isActual: SelectChoise.True, isMain: SelectChoise.True, isPrime: SelectChoise.All);
            int[] arrMainER = new int[qry.Count];
            int e = 0;
            foreach (var r in qry)
            {
                arrMainER[e] = r.Id;
                e++;
            }

            List<CostCenter> mainCCList = CostCenter.ToList(isActual: SelectChoise.True, isMain: SelectChoise.True, isTechnology: SelectChoise.All);

            Excel.Application ex = new Excel.Application();
            ex.Visible = false;
            ex.DisplayAlerts = true;

            ex.Workbooks.Open(AppDomain.CurrentDomain.BaseDirectory + @"УФ_Пофакторный анализ_ЦЗ-000.xltx",
                              Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                              Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                              Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                              Type.Missing, Type.Missing);

            Excel.Worksheet[] arrSheets = new Excel.Worksheet[mainCCList.Count];
            for (int s = 0; s < mainCCList.Count; s++)
            {
                var sh = ex.Sheets;
                arrSheets[s] = (Excel.Worksheet)sh.Add(Type.Missing, sh[s + 2], Type.Missing, Type.Missing);
                arrSheets[s].Name = mainCCList[s].Name;
                arrSheets[s].Activate();
                Excel.Worksheet sheet = arrSheets[s]; // (Excel.Worksheet)ex.Worksheets.get_Item(s + 2); // arrSheets[s];
                int stroka0 = 4;

                Excel.Range h1 = (Excel.Range)sheet.Cells[1, 1];
                Excel.Range h2 = (Excel.Range)sheet.Cells[1, 1];
                Excel.Range rangeCapt = sheet.get_Range(h1, h2);
                rangeCapt.Value = "Пофакторный анализ отклонений от расходных норм/лимитов по " + mainCCList[s].Name + "  за период: " + period;
                rangeCapt.Cells.Font.Bold = true;
                rangeCapt.Cells.Font.Size = 14;

                for (int i = 0; i < arrMainER.Length; i++)
                {
                    gridData.Clear();
                    await Task.Run(() => TableDataFill(mainCCList[s].Id, arrMainER[i]));
                    if (gridData.Count > 2)
                    {
                        Excel.Range c1 = (Excel.Range)sheet.Cells[stroka0, 1];
                        Excel.Range c2 = (Excel.Range)sheet.Cells[stroka0, len2 + 5];
                        Excel.Range rangeCaption = sheet.get_Range(c1, c2);
                        rangeCaption.Value = arrHeaders;

                        c1 = (Excel.Range)sheet.Cells[stroka0 - 1, 1];
                        c2 = (Excel.Range)sheet.Cells[stroka0 - 1, 1];
                        Excel.Range range1 = sheet.get_Range(c1, c2);
                        range1.Value = gridData[1].ERName + ", " + gridData[1].UnitName;
                        range1.Cells.Font.Bold = true;
                        range1.Cells.Font.Size = 12;
                        range1.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(0xFF, 0xFF, 0xCC));

                        range1.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range1.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range1.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range1.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range1.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;


                        rangeCaption.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;
                        rangeCaption.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                        rangeCaption.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                        rangeCaption.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                        rangeCaption.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;
                        rangeCaption.Cells.Font.Bold = true;
                        rangeCaption.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(220, 220, 220));

                        int j = 0;
                        object[,] arr = new object[gridData.Count, 6];
                        foreach (FullFields r in gridData)
                        {
                            arr[j, 0] = r.ProductName;
                            arr[j, 1] = r.Fact;
                            arr[j, 2] = r.Plan;
                            arr[j, 3] = r.Diff;
                            arr[j, 4] = r.DiffProc;
                            arr[j, 5] = r.Remark;
                            j++;
                        }
                        int len1 = arr.GetLength(0);
                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, 1];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, len2 + 1];
                        Excel.Range range = sheet.get_Range(c1, c2);
                        range.Value = arr;

                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, len2 + 5];
                        range = sheet.get_Range(c1, c2);
                        range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                        range.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;

                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, 2];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, len2 + 5];
                        range = sheet.get_Range(c1, c2);
                        range.EntireColumn.AutoFit();

                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, 1];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + 1, 1];
                        range = sheet.get_Range(c1, c2);
                        range.EntireColumn.ColumnWidth = 40;

                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, numericStartPozition];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, len2];
                        range = sheet.get_Range(c1, c2);
                        range.NumberFormat = "#,0.00";

                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, 2];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + 1, 5];
                        range = sheet.get_Range(c1, c2);
                        range.Value = "";

                        c1 = (Excel.Range)sheet.Cells[stroka0 + len1, 1];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, len2 + 5];
                        range = sheet.get_Range(c1, c2);
                        range.Cells.Font.Bold = true;
                        range.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));

                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, 7];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, 7];
                        range = sheet.get_Range(c1, c2);

                        range.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=Классификатор!$F$2:$F$26");
                        range.Validation.InCellDropdown = true;
                        range.EntireColumn.ColumnWidth = 36;
                        range.WrapText = true;

                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, 8];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, 8];
                        range = sheet.get_Range(c1, c2);

                        range.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=Классификатор!$F$27:$F$30");
                        range.Validation.InCellDropdown = true;
                        range.EntireColumn.ColumnWidth = 18;
                        range.WrapText = true;

                        c1 = (Excel.Range)sheet.Cells[stroka0 + 1, 10];
                        c2 = (Excel.Range)sheet.Cells[stroka0 + len1, 10];
                        range = sheet.get_Range(c1, c2);
                        range.WrapText = true;

                        string form = "= ";
                        string adder = "";
                        for (int k = 1; k < len1; k++)
                        {
                            c1 = (Excel.Range)sheet.Cells[stroka0 + k, 9];
                            c2 = (Excel.Range)sheet.Cells[stroka0 + k, 9];
                            range = sheet.get_Range(c1, c2);
                            adder = range.get_Address(1, 1, Excel.XlReferenceStyle.xlA1, Type.Missing, Type.Missing);
                            form = form + " + " + adder;
                        }
                        Excel.Range rr = sheet.Cells[stroka0 + len1, 9] as Excel.Range;
                        rr.Formula = form;
                        rr.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(0xFF, 0xFF, 0xCC));
                        stroka0 = stroka0 + len1 + 3;
                    }
                }

            }

            ind.Close();
            ex.Visible = true;
            ex.DisplayAlerts = true;

            void TableDataFill(int cc, int er)
            {
                //gridData.Clear();
                //List<FullFields> data = new();
                FullFields m = new FullFields();
                m.ProductName = "Общие причины отклонения";
                gridData.Add(m);

                foreach (FullFields r in FullFields.ERUseFromCC(period, cc, er))
                {
                    FullFields nn = new();
                    nn.Period = r.Period;
                    nn.IdCC = r.IdCC;
                    nn.CCName = r.IdCC.ToString();
                    nn.IdER = r.IdER;
                    nn.ERName = r.ERName;
                    nn.UnitName = r.UnitName;
                    nn.ProductName = r.ProductName.Trim();
                    nn.Year = r.Year;
                    nn.Month = r.Month;
                    nn.Plan = r.Plan;
                    nn.Fact = r.Fact;
                    nn.Diff = r.Diff;
                    nn.DiffProc = r.Plan != 0 ? (r.Fact - r.Plan) / r.Plan * 100 : r.Fact != 0 ? 999 : 0;
                    nn.Remark = Math.Abs(nn.DiffProc) > 2 ? "*" : "";
                    gridData.Add(nn);
                }

                FullFields l = new FullFields();
                l.ProductName = "ИТОГО:";
                l.Plan = gridData.Sum(n => n.Plan);
                l.Fact = gridData.Sum(n => n.Fact);
                l.Diff = gridData.Sum(n => n.Diff);
                l.DiffProc = gridData.Sum(n => n.Plan) != 0 ? gridData.Sum(n => n.Diff) * 100 / gridData.Sum(n => n.Plan) : gridData.Sum(n => n.Fact) != 0 ? 999 : 0;
                gridData.Add(l);
            }

        }

    }
}
