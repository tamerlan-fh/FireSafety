using FireSafety.Models;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace FireSafety.VisualModels
{
    class EvacuationPlanImage
    {     
        public EvacuationPlanImage(BitmapSource source)
        {
            BitmapSourceValue = source;
        }
        public double Height { get { return BitmapSourceValue.Height; } }
        public double Width { get { return BitmapSourceValue.Width; } }
        public Bitmap BitmapValue
        {
            get { return Settings.GetBitmap(BitmapSourceValue); }
        }
        public BitmapSource BitmapSourceValue { get; private set; }
    }
}
