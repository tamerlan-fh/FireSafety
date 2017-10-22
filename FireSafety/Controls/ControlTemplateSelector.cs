using FireSafety.Models;
using System.Windows;
using System.Windows.Controls;

namespace FireSafety.Controls
{
    public class ControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ControlTemplateОбъект { get; set; }
        public DataTemplate ControlTemplateДверь { get; set; }
        public DataTemplate ControlTemplateЭтаж { get; set; }
        public DataTemplate ControlTemplateСтарт { get; set; }
        public DataTemplate ControlTemplateПуть { get; set; }
        public DataTemplate ControlTemplateУчастокПути { get; set; }
        public DataTemplate ControlTemplateЗдание { get; set; }
        public DataTemplate ControlTemplateУзелЛестницы { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if (item is EntryNode) return ControlTemplateДверь;
            if (item is Floor) return ControlTemplateЭтаж;
            if (item is Building) return ControlTemplateЗдание;
            if (item is StartNode) return ControlTemplateСтарт;
            if (item is Route) return ControlTemplateПуть;
            if (item is RoadSection) return ControlTemplateУчастокПути;
            if (item is StairsNode) return ControlTemplateУзелЛестницы;

            return ControlTemplateОбъект;
        }
    }
}
