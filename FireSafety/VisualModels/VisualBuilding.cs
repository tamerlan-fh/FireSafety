using FireSafety.Models;

namespace FireSafety.VisualModels
{
    class VisualBuilding
    {
        public Building Model { get; private set; }
        public VisualBuilding(Building model)
        {
            this.Model = model;
        }
    }
}
