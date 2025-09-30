
using ShipyardDashboard.Models;
using ShipyardDashboard.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ShipyardDashboard.Services
{
    public class MetricTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextTemplate { get; set; }
        public DataTemplate? PlotTemplate { get; set; }
        public DataTemplate? ProgressBarTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is MetricViewModel vm)
            {
                return vm.VizType switch
                {
                    VisualizationType.Gauge => PlotTemplate,
                    VisualizationType.Sparkline => PlotTemplate, // 스파크라인도 PlotTemplate 사용
                    VisualizationType.ProgressBar => ProgressBarTemplate,
                    _ => TextTemplate,
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
