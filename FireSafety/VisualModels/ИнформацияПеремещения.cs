using System.Windows;

namespace FireSafety.VisualModels
{
    class ИнформацияПеремещения
    {
        public ИнформацияПеремещения(VisualЭлемент элемент, Point позиция)
        {
            this.Элемент = элемент;
            this.НачальнаяПозиция = позиция;
        }

        public VisualЭлемент Элемент { get; private set; }
        public Point НачальнаяПозиция { get; set; }
    }
}
