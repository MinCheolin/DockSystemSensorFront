using ShipyardDashboard.Models;
using ShipyardDashboard.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ShipyardDashboard.Services
{
    public class DetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextTemplate { get; set; }
        public DataTemplate? GaugeTemplate { get; set; }
        public DataTemplate? ProgressBarTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            // This class is obsolete and should not be used.
            // The logic has been moved to MetricTemplateSelector.
            // This implementation is just to allow the project to compile.
            return TextTemplate;
        }
    }
}