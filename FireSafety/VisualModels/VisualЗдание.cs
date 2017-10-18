using FireSafety.Models;

namespace FireSafety.VisualModels
{
    class VisualЗдание
    {
        public Building Модель { get; private set; }
        public VisualЗдание(Building модель)
        {
            this.Модель = модель;
        }
    }
}
