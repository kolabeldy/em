using Kant.Wpf.Controls.Chart;
using Kant.Wpf.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FlowDirection = Kant.Wpf.Controls.Chart.FlowDirection;

namespace em.MyCharts
{
    public class MySankeyModel : ViewModelBase
    {
        public List<SankeyData> cdata;

        #region Constructor

        public MySankeyModel(List<SankeyData> cdata)
        {
            this.cdata = cdata;
            SankeyLabelStyle = (Style)Application.Current.FindResource("SankeyLabelStyle");
            SankeyToolTipTemplate = (ControlTemplate)Application.Current.FindResource("SankeyToolTipTemplate");
        }
        public void ChartFill()
        {
            SankeyDatas = cdata;
            SankeyShowLabels = true;
            SankeyLinkCurvature = 0.95;
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
                if (value != sankeyNodeBrushes)
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
                if (value != sankeyFlowDirection)
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
