using em.DBAccess;
using em.Helpers;
using em.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace em.MenuPages
{
    public class TariffsEditModel : INotifyPropertyChanged
    {
        public bool IsNotAdmin { get; set; }
        public bool IsAdmin { get; set; }

        private List<Tariff> tarifList;

        public TariffsEditModel()
        {
            //SteamTemperature = 280;
            //SteamPressure = 1.30;
            //CondensateReturn = 100;

            //IsNotAdmin = !AdminSettings.IsAdmin;
            IsAdmin = true;
            tarifList = Tariff.ToList();
            Tariff950 = tarifList.Find(m => m.Id == 950).Tarif;
            Tariff951 = tarifList.Find(m => m.Id == 951).Tarif;
            Tariff954 = tarifList.Find(m => m.Id == 954).Tarif;
            Tariff955 = tarifList.Find(m => m.Id == 955).Tarif;
            Tariff966 = tarifList.Find(m => m.Id == 966).Tarif;
            Tariff991 = tarifList.Find(m => m.Id == 991).Tarif;
            Tariff85608 = tarifList.Find(m => m.Id == 85608).Tarif;
            Tariff85609 = tarifList.Find(m => m.Id == 85609).Tarif;
            SteamThermalEnergy = tarifList.Find(m => m.Id == 990002).Tarif;
            SteamTemperature = tarifList.Find(m => m.Id == 990003).Tarif;
            SteamPressure = tarifList.Find(m => m.Id == 990004).Tarif;
            CondensateReturn = tarifList.Find(m => m.Id == 990005).Tarif;

            Tariff990 = RetStreamTariff();
        }

        private double steamThermalEnergy;
        public double SteamThermalEnergy
        {
            get
            {
                return steamThermalEnergy;
            }
            set
            {
                steamThermalEnergy = value;
                Tariff990 = RetStreamTariff();
                OnPropertyChanged("SteamThermalEnergy");
            }
        }

        private double steamTemperature;
        public double SteamTemperature
        {
            get
            {
                return steamTemperature;
            }
            set
            {
                steamTemperature = value;
                Tariff990 = RetStreamTariff();
                OnPropertyChanged("SteamTemperature");
            }
        }

        private double steamPressure;
        public double SteamPressure
        {
            get
            {
                return steamPressure;
            }
            set
            {
                steamPressure = value;
                Tariff990 = RetStreamTariff();
                OnPropertyChanged("SteamPressure");
            }
        }

        private double condensateReturn;
        public double CondensateReturn
        {
            get
            {
                return condensateReturn;
            }
            set
            {
                condensateReturn = value;
                Tariff990 = RetStreamTariff();
                OnPropertyChanged("CondensateReturn");
            }
        }

        private double tariff85608;
        public double Tariff85608
        {
            get
            {
                return tariff85608;
            }
            set
            {
                tariff85608 = value;
                Tariff991 = value;
                Tariff990 = RetStreamTariff();
                OnPropertyChanged("Tarif85608");
            }
        }

        private double tariff85609;
        public double Tariff85609
        {
            get
            {
                return tariff85609;
            }
            set
            {
                tariff85609 = value;
                Tariff990 = RetStreamTariff();
                OnPropertyChanged("Tarif85609");
            }
        }

        private double tariff950;
        public double Tariff950
        {
            get
            {
                return tariff950;
            }
            set
            {
                tariff950 = value;
                OnPropertyChanged("Tariff950");
            }
        }
        private double tariff951;
        public double Tariff951
        {
            get
            {
                return tariff951;
            }
            set
            {
                tariff951 = value;
                OnPropertyChanged("Tariff951");
            }
        }
        private double tariff954;
        public double Tariff954
        {
            get
            {
                return tariff954;
            }
            set
            {
                tariff954 = value;
                OnPropertyChanged("Tariff954");
            }
        }
        private double tariff955;
        public double Tariff955
        {
            get
            {
                return tariff955;
            }
            set
            {
                tariff955 = value;
                OnPropertyChanged("Tariff955");
            }
        }
        private double tariff966;
        public double Tariff966
        {
            get
            {
                return tariff966;
            }
            set
            {
                tariff966 = value;
                OnPropertyChanged("Tariff966");
            }
        }
        private double tariff990;
        public double Tariff990
        {
            get
            {
                return tariff990;
            }
            set
            {
                tariff990 = value;
                OnPropertyChanged("Tariff990");
            }
        }
        private double tariff991;
        public double Tariff991
        {
            get
            {
                return tariff991;
            }
            set
            {
                tariff991 = value;
                OnPropertyChanged("Tariff991");
            }
        }
        private double H2_PT(double t, double p)
        // возвращает энтальпию пара по давлению и температуре
        {
            double tau;
            double pi;

            tau = (t + 273.15) / 647.14;
            pi = p / 22.064;

            return (10258.8 - 20231.3 / tau + 24702.8 / Math.Pow(tau, 2) - 16307.3 / Math.Pow(tau, 3) + 5579.31 / Math.Pow(tau, 4) - 777.285 / Math.Pow(tau, 5)) +
                    pi * (-355.878 / tau + 817.288 / Math.Pow(tau, 2) - 845.841 / Math.Pow(tau, 3)) - Math.Pow(pi, 2) * (160.276 / Math.Pow(tau, 3)) +
                    Math.Pow(pi, 3) * (-95607.5 / tau + 443740 / Math.Pow(tau, 2) - 767668 / Math.Pow(tau, 3) + 587261 / Math.Pow(tau, 4) - 167657 / Math.Pow(tau, 5)) +
                    Math.Pow(pi, 4) * (22542.8 / Math.Pow(tau, 2) - 84140.2 / Math.Pow(tau, 3) + 104198 / Math.Pow(tau, 4) - 42886.7 / Math.Pow(tau, 5));
        }
        private double RetStreamTariff()
        {
            double H2 = H2_PT(SteamTemperature, SteamPressure) / 4187;

            return Tariff85608 + (1 / H2) * (1 - CondensateReturn / 100) * Tariff85609;
        }

        private MyRelayCommand btnSave_Click;
        public MyRelayCommand BtnSave_Click => btnSave_Click ??
                    (btnSave_Click = new MyRelayCommand(obj =>
                    {
                        List<Tariff> tempTariff = new();
                        tempTariff.Add(new Tariff { Id = 950, Tarif = Tariff950 });
                        tempTariff.Add(new Tariff { Id = 951, Tarif = Tariff951 });
                        tempTariff.Add(new Tariff { Id = 954, Tarif = Tariff954 });
                        tempTariff.Add(new Tariff { Id = 955, Tarif = Tariff955 });
                        tempTariff.Add(new Tariff { Id = 966, Tarif = Tariff966 });
                        tempTariff.Add(new Tariff { Id = 990, Tarif = Tariff990 });
                        tempTariff.Add(new Tariff { Id = 991, Tarif = Tariff991 });
                        tempTariff.Add(new Tariff { Id = 85608, Tarif = Tariff85608 });
                        tempTariff.Add(new Tariff { Id = 85609, Tarif = Tariff85609 });
                        tempTariff.Add(new Tariff { Id = 990002, Tarif = SteamThermalEnergy });
                        tempTariff.Add(new Tariff { Id = 990003, Tarif = SteamTemperature });
                        tempTariff.Add(new Tariff { Id = 990004, Tarif = SteamPressure });
                        tempTariff.Add(new Tariff { Id = 990005, Tarif = CondensateReturn });
                        Tariff.Save(tempTariff);
                    }));
        private MyRelayCommand tariffsEditCloseCommand;
        public MyRelayCommand TariffsEditCloseCommand
        {
            get
            {
                return tariffsEditCloseCommand ??
                    (tariffsEditCloseCommand = new MyRelayCommand(obj =>
                    {
                        //window.MainFrame.Content = null;
                    }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
