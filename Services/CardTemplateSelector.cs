using ShipyardDashboard.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ShipyardDashboard.Services
{
    public class CardTemplateSelector : DataTemplateSelector
    {
        // General Templates
        public DataTemplate? SmallCardTemplate { get; set; }
        public DataTemplate? WideCardTemplate { get; set; }

        // Specific Templates
        public DataTemplate? CncPlasmaCutterTemplate { get; set; }
        public DataTemplate? BendingRollerTemplate { get; set; }
        public DataTemplate? Co2WelderTemplate { get; set; }
        public DataTemplate? GantryCraneTemplate { get; set; }
        public DataTemplate? TempHumidityTemplate { get; set; }
        public DataTemplate? PipeBenderTemplate { get; set; }
        public DataTemplate? GoliathCraneTemplate { get; set; }
        public DataTemplate? TransporterTemplate { get; set; }
        public DataTemplate? MainEngineTemplate { get; set; }
        public DataTemplate? ShipGeneratorTemplate { get; set; }
        public DataTemplate? WeldingRobotTemplate { get; set; }
        public DataTemplate? ShotBlastingMachineTemplate { get; set; }
        public DataTemplate? BallastPumpTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not EquipmentCardViewModel vm) return base.SelectTemplate(item, container);

            return vm.Equipment.Name switch
            {
                "골리앗 크레인" => GoliathCraneTemplate,
                "주 엔진" => MainEngineTemplate,
                "CNC 플라즈마 절단기" => CncPlasmaCutterTemplate,
                "강판 벤딩 롤러" => BendingRollerTemplate,
                "CO2 용접기" => Co2WelderTemplate,
                "갠트리 크레인" => GantryCraneTemplate,
                "온/습도 모니터링 시스템" => TempHumidityTemplate,
                "파이프 자동 벤딩기" => PipeBenderTemplate,
                "트랜스포터" => TransporterTemplate,
                "선박 발전기" => ShipGeneratorTemplate,
                "용접 로봇" => WeldingRobotTemplate,
                "쇼트 블라스팅기" => ShotBlastingMachineTemplate,
                "밸러스트 펌프" => BallastPumpTemplate,
                _ => vm.ColumnSpan > 1 ? WideCardTemplate : SmallCardTemplate
            };
        }
    }
}