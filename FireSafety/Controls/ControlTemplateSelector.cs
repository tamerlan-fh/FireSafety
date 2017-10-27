using FireSafety.Models;
using System.Windows;
using System.Windows.Controls;

namespace FireSafety.Controls
{
    public class ControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ControlTemplateEntity { get; set; }
        public DataTemplate ControlTemplateEntryNode { get; set; }
        public DataTemplate ControlTemplateFloor { get; set; }
        public DataTemplate ControlTemplateStartNode { get; set; }
        public DataTemplate ControlTemplateRoute { get; set; }
        public DataTemplate ControlTemplateRoadSection { get; set; }
        public DataTemplate ControlTemplateBuilding { get; set; }
        public DataTemplate ControlTemplateStairsNode { get; set; }
        public DataTemplate ControlTemplateFloorsConnectionSection { get; set; }
        public DataTemplate ControlTemplateEntityReadOnly { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if (item is EntryNode) return ControlTemplateEntryNode;
            if (item is Floor) return ControlTemplateFloor;
            if (item is Building) return ControlTemplateBuilding;
            if (item is StartNode) return ControlTemplateStartNode;
            if (item is Route) return ControlTemplateRoute;
            if (item is FloorsConnectionSection) return ControlTemplateFloorsConnectionSection;
            if (item is RoadSection) return ControlTemplateRoadSection;
            if (item is StairsNode) return ControlTemplateStairsNode;
            if (item is BuildingComposition || item is EvacuationPlan) return ControlTemplateEntityReadOnly;

            return ControlTemplateEntity;
        }
    }
}
