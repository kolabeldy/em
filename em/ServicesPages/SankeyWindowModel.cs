using em.Models;
using Kant.Wpf.Controls.Chart;
using Kant.Wpf.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FlowDirection = Kant.Wpf.Controls.Chart.FlowDirection;

namespace em.ServicesPages
{
    public class SankeyModel : ViewModelBase
    {
        public List<DataChart> cdata;
        public string ctype;

        #region Slider

        private int sliderValue;
        public int SliderValue
        {
            get
            {
                return sliderValue;
            }
            set
            {
                sliderValue = value;
                SliderValueTxt = ((double)value / 10).ToString("N1") + "%";
                ChartFill();
                RaisePropertyChanged(() => SliderValue);
            }
        }
        private string sliderValueTxt;
        public string SliderValueTxt
        {
            get
            {
                return sliderValueTxt;
            }
            set
            {
                sliderValueTxt = value;
                RaisePropertyChanged(() => SliderValueTxt);
            }
        }

        #endregion


        #region Constructor

        public SankeyModel(List<DataChart> cdata)
        {
            this.cdata = cdata;
            SankeyLabelStyle = (Style)Application.Current.FindResource("SankeyLabelStyle");
            SankeyToolTipTemplate = (ControlTemplate)Application.Current.FindResource("SankeyToolTipTemplate");
            SliderValue = 10;
        }
        public void ChartFill()
        {
            List<DataChart> chartData = new List<DataChart>();
            double total = cdata.Sum(n => Math.Abs(n.YParam1));

            var qry1 = from o in cdata
                       where Math.Abs(o.YParam1) >= Math.Abs(total * SliderValue / 1000)
                       orderby o.YParam1 descending
                       select new
                       {
                           o.YParam1,
                           o.XParam
                       };
            var qry2 = from o in cdata
                       where Math.Abs(o.YParam1) < Math.Abs(total * SliderValue / 1000)
                       select new
                       {
                           o.YParam1,
                           o.XParam
                       };
            double total1 = 0;
            if (cdata.Count > 1)
            {
                total1 = qry2.Sum(n => n.YParam1);
            }
            foreach(var r in qry1)
            {
                DataChart n = new DataChart();
                n.YParam1 = r.YParam1;
                n.XParam = r.XParam;
                chartData.Add(n);
            }
            chartData.Add(new DataChart { YParam1 = total1, XParam = "прочие" });

            List<SankeyData> datas = new List<SankeyData>();
            foreach (var r in chartData)
            {
                if (r.YParam1 > 1)
                {
                    if (ctype == "CC")
                        datas.Add(new SankeyData(r.XParam, "Центры затрат", Math.Round(r.YParam1, 1)));
                    if (ctype == "ER")
                        datas.Add(new SankeyData("Энергресурсы", r.XParam, Math.Round(r.YParam1, 1)));
                    if (ctype == "PR")
                        datas.Add(new SankeyData("Энергресурсы", r.XParam, Math.Round(r.YParam1, 1)));
                    if (ctype == "Loss")
                        datas.Add(new SankeyData(r.XParam, "Потери", Math.Round(r.YParam1, 1)));
                }
            }
            SankeyDatas = datas;
            SankeyShowLabels = true;
            SankeyLinkCurvature = 0.95;
            //SankeyFlowDirection = FlowDirection.TopToBottom;

        }

        #endregion


        #region Fields & Properties

        private List<SankeyData> sankeyDatas;
        public IReadOnlyList<SankeyData> SankeyDatas
        {
            get
            {
                return sankeyDatas;
            }
            private set
            {
                if (value != sankeyDatas)
                {
                    sankeyDatas = value == null ? null : value.ToList();
                    RaisePropertyChanged(() => SankeyDatas);
                }
            }
        }

        private double sankeyLinkCurvature;
        public double SankeyLinkCurvature
        {
            get
            {
                return sankeyLinkCurvature;
            }
            set
            {
                if (value != sankeyLinkCurvature)
                {
                    sankeyLinkCurvature = value;
                    RaisePropertyChanged(() => SankeyLinkCurvature);
                }
            }
        }

        private Dictionary<string, Brush> sankeyNodeBrushes;
        public IReadOnlyDictionary<string, Brush> SankeyNodeBrushes
        {
            get
            {
                return sankeyNodeBrushes;
            }
            private set
            {
                if(value != sankeyNodeBrushes)
                {
                    sankeyNodeBrushes = value == null ? null : value.ToDictionary(item => item.Key, item => item.Value);
                    RaisePropertyChanged(() => SankeyNodeBrushes);
                }
            }
        }

        private string highlightSankeyNode;
        public string HighlightSankeyNode
        {
            get
            {
                return highlightSankeyNode;
            }
            set
            {
                highlightSankeyNode = value;
                RaisePropertyChanged(() => HighlightSankeyNode);
            }
        }

        private SankeyLinkFinder highlightSankeyLink;
        public SankeyLinkFinder HighlightSankeyLink
        {
            get
            {
                return highlightSankeyLink;
            }
            set
            {
                highlightSankeyLink = value;
                RaisePropertyChanged(() => HighlightSankeyLink);
            }
        }

        private FlowDirection sankeyFlowDirection;
        public FlowDirection SankeyFlowDirection
        {
            get
            {
                return sankeyFlowDirection;
            }
            set
            {
                if(value != sankeyFlowDirection)
                {
                    sankeyFlowDirection = value;
                    RaisePropertyChanged(() => SankeyFlowDirection);
                }
            }
        }

        private bool sankeyShowLabels;
        public bool SankeyShowLabels
        {
            get
            {
                return sankeyShowLabels;
            }
            set
            {
                if (value != sankeyShowLabels)
                {
                    sankeyShowLabels = value;
                    RaisePropertyChanged(() => SankeyShowLabels);
                }
            }
        }

        private HighlightMode sankeyHighlightMode;
        public HighlightMode SankeyHighlightMode
        {
            get
            {
                return sankeyHighlightMode;
            }
            set
            {
                if (value != sankeyHighlightMode)
                {
                    sankeyHighlightMode = value;
                    RaisePropertyChanged(() => SankeyHighlightMode);
                }
            }
        }

        private Style sankeyLabelStyle;
        public Style SankeyLabelStyle
        {
            get
            {
                return sankeyLabelStyle;
            }
            set
            {
                if (value != sankeyLabelStyle)
                {
                    sankeyLabelStyle = value;
                    RaisePropertyChanged(() => SankeyLabelStyle);
                }
            }
        }

        private ControlTemplate sankeyToolTipTemplate;
        public ControlTemplate SankeyToolTipTemplate
        {
            get
            {
                return sankeyToolTipTemplate;
            }
            set
            {
                if (value != sankeyToolTipTemplate)
                {
                    sankeyToolTipTemplate = value;
                    RaisePropertyChanged(() => SankeyToolTipTemplate);
                }
            }
        }

        private Style sankeyLabelStyle1;

        private Style sankeyLabelStyle2;

        private ControlTemplate sankeyToolTipTemplate1;

        private ControlTemplate sankeyToolTipTemplate2;

        private Random random;

        #endregion

    }
}
