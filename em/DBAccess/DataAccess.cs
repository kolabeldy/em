using em.Helpers;
using em.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace em.DBAccess
{
    public class DataAccess
    {
        public static List<EResource> RetERListLosses(bool isPrime)
        {
            string idPrime = isPrime ? "1" : "0";
            List<EResource> rez = new List<EResource>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT e.IdCode, e.Name FROM EResources as e, (Select * FROM FactLosses Group By Id) as l WHERE e.IsActual = 1 AND e.IsPrime = " + idPrime
                                + " AND e.IdCode = l.IdER AND l.Year >= " + (Period.LastYear - 1).ToString() + " AND e.IdCode <> 16155 AND e.IdCode <> 954 GROUP BY e.IdCode";
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
            public static List<FullFields> RetLosses(ChartDataType argType, List<Person> dateSel, List<Person> erSel)
        {
            string cType = default;
            if (argType == ChartDataType.FactLoss) cType = "IdER";
            else if (argType == ChartDataType.Period) cType = "Period";
            List<FullFields> rez = new List<FullFields>();
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string SQLtxt = "SELECT Period, "
                                + "IdER, ERName, UnitName, "
                                + "SUM(FactLoss) as FactLoss, SUM(NormLoss) as NormLoss, SUM(DiffLoss) as DiffLoss, SUM(FactCost) as FactCost, "
                                + "SUM(NormCost) as NormCost, SUM(DiffCost) as DiffCost, FactProc, NormProc "
                                + "FROM LosseFullCosts "
                                + "WHERE Period IN " + Global.ListToSting(dateSel) + " "
                                + "AND IdER IN " + Global.ListToSting(erSel) + " "
                                + "GROUP BY " + cType;
                SqliteCommand selectCommand = new SqliteCommand(SQLtxt, db);

                SqliteDataReader q = selectCommand.ExecuteReader();
                while (q.Read())
                {
                    FullFields r = new FullFields();
                    r.Period = q.GetInt32(0);
                    r.Year = r.Period / 100;
                    r.Month = r.Period - r.Year * 100;
                    r.PeriodStr = r.Year + "_" + (r.Month < 10 ? "0" + r.Month : r.Month);
                    r.IdER = q.GetInt32(1);
                    r.ERName = q.GetString(2);
                    r.UnitName = q.GetString(3);
                    r.FactLoss = q.GetDouble(4);
                    r.NormLoss = q.GetDouble(5);
                    r.DiffLoss = q.GetDouble(6);
                    r.FactLossCost = q.GetDouble(7);
                    r.NormLossCost = q.GetDouble(8);
                    r.DiffLossCost = q.GetDouble(9);
                    r.FactLossProc = q.GetDouble(10);
                    r.NormLossProc = q.GetDouble(11);
                    rez.Add(r);
                }
                //Period.LastPeriodLosses = rez[0].Period;
            }
            return rez;
        }

        public static bool DeleteLastMonthUseAndLosses()
        {
            MessageBoxResult result = MessageBox.Show("Данные по использованию энергоресурсов и по фактическим потерям за последний введённый месяц будут удалены", "Обновление БД", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result != MessageBoxResult.OK) return false;
            using (SqliteConnection db = new SqliteConnection($"Filename={Global.dbpath}"))
            {
                db.Open();
                string sqlText = String.Format("DELETE FROM ERUses WHERE Period = " + Period.LastPeriod);

                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = sqlText;
                deleteCommand.ExecuteReader();

                sqlText = String.Format("DELETE FROM FactLosses WHERE Period = " + Period.LastPeriod);

                deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = sqlText;
                deleteCommand.ExecuteReader();

                db.Close();
            }
            MessageBox.Show("Данные за последний месяц успешно удалены");
            return true;
        }
        public static bool SaveDB()
        {
            string sourcePath = AppDomain.CurrentDomain.BaseDirectory + @"DB\";
            string destinationPath = null;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.CheckPathExists = true;
            dialog.FileName = "emdb.db";
            string opFile = "";
            if (dialog.ShowDialog() == true)
                opFile = dialog.FileName;
            else return false;


            destinationPath = dialog.FileName;
            string sourceFileName = "emdb.db";
            string sourceFile = System.IO.Path.Combine(sourcePath, sourceFileName);
            string destinationFile = destinationPath;

            try
            {
                System.IO.File.Copy(sourceFile, destinationFile, true);
            }
            catch (IOException copyError)
            {
                MessageBoxResult resErr = System.Windows.MessageBox.Show(copyError.Message, "Резервное копирование БД", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            MessageBoxResult resOk = System.Windows.MessageBox.Show("База данных успешно сохранена в резервную копию", "Резервное копирование БД");
            return true;
        }
        public static bool RestoreDB()
        {
            string destinationPath = AppDomain.CurrentDomain.BaseDirectory + @"DB\";
            string sourceFileName = null;

            OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "emdb.db";
            dialog.DefaultExt = ".db"; // Default file extension
            dialog.Filter = "База данных SQLite (.db)|*.db"; // Filter files by extension
            if (dialog.ShowDialog() == true)
            {
                sourceFileName = dialog.FileName;
                string destinationFileName = "emdb.db";
                string destinationFile = Path.Combine(destinationPath, destinationFileName);

                try
                {
                    System.IO.File.Copy(sourceFileName, destinationFile, true);
                }
                catch (IOException copyError)
                {
                    MessageBoxResult resErr = MessageBox.Show(copyError.Message, "Резервное копирование БД", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                MessageBoxResult resOk = MessageBox.Show("База данных успешно восстановлена из резервной копии", "Резервное копирование БД");
            }
            return true;
        }
        public static void SynchronizeDB()
        {
            string sourcePath = AppDomain.CurrentDomain.BaseDirectory + @"DB\";
            //string destinationPath = @"\\172.16.16.167\совещание по РНЭ\em\DB\";
            string destinationPath = @"D:\em\DB\";

            string sourceFileName = "emdb.db";
            string destinationFileName = "emdb.db";
            string sourceFile = System.IO.Path.Combine(sourcePath, sourceFileName);
            string destinationFile = System.IO.Path.Combine(destinationPath, destinationFileName);
            DateTime dateDestination = File.GetLastWriteTime(destinationFile);
            try
            {
                DateTime dateSource = File.GetLastWriteTime(sourceFile);
                if (dateSource > dateDestination)
                {
                    System.IO.File.Copy(sourceFile, destinationFile, true);
                    MessageBoxResult resOk = MessageBox.Show("База данных успешно синхронизирована", "Синхронизирование БД");
                }
                else
                {
                    MessageBoxResult resOk = MessageBox.Show("Синхронизация не требуется. Используется актуальная версия базы данных", "Синхронизирование БД");
                }
            }
            catch (IOException copyError)
            {
                MessageBoxResult resErr = MessageBox.Show(copyError.Message + "Синхронизация невозможна!", "Синхронизирование БД", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

    }
}
