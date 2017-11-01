using FireSafety.Properties;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace FireSafety
{
    class SettingsManager
    {
        private static SettingsManager instance;
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SettingsManager();
                return instance;
            }
        }
        private SettingsManager()
        {
            EntryIco = GetBitmapImage(Resources.entry_icon_64, ImageFormat.Png);
            ExitIco = GetBitmapImage(Resources.exit_icon_64, ImageFormat.Png);
            StartIco = GetBitmapImage(Resources.start_icon_64, ImageFormat.Png);
            BuildingIco = GetBitmapImage(Resources.building_icon_64, ImageFormat.Png);
            FloorIco = GetBitmapImage(Resources.floor_icon_64, ImageFormat.Png);
            SectionRoadIco = GetBitmapImage(Resources.section_icon_64, ImageFormat.Png);
            RouteIco = GetBitmapImage(Resources.route_icon_64, ImageFormat.Png);
            NodeIco = GetBitmapImage(Resources.node_icon_64, ImageFormat.Png);
            StairsIco = GetBitmapImage(Resources.stairs_icon_64, ImageFormat.Png);
            FloorsConnectionIco = GetBitmapImage(Resources.floors_connection_icon_64, ImageFormat.Png);
        }

        public static BitmapImage GetBitmapImage(Image image)
        {
            return GetBitmapImage(new Bitmap(image), ImageFormat.Bmp);
        }
        public static BitmapImage GetBitmapImage(Bitmap bitmap, ImageFormat format)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, format);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static Bitmap GetBitmap(BitmapSource source)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);
                var bitmap = new Bitmap(stream);

                return new Bitmap(bitmap);
            }
        }

        public static byte[] GetBytesFromBitmap(Bitmap bitmap)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
        }

        public static byte[] GetBytesFromBitmapImage(BitmapImage bitmap)
        {
            if (bitmap == null) return new byte[] { };

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        public static BitmapImage GetBitmapImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;

            var imageSource = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                //  stream.Seek(0, SeekOrigin.Begin);
                stream.Position = 0;
                imageSource.BeginInit();
                imageSource.StreamSource = stream;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
            }

            return imageSource;
        }

        public BitmapImage EntryIco { get; private set; }
        public BitmapImage ExitIco { get; private set; }
        public BitmapImage StartIco { get; private set; }
        public BitmapImage BuildingIco { get; private set; }
        public BitmapImage FloorIco { get; private set; }
        public BitmapImage SectionRoadIco { get; private set; }
        public BitmapImage RouteIco { get; private set; }
        public BitmapImage NodeIco { get; private set; }
        public BitmapImage StairsIco { get; private set; }
        public BitmapImage FloorsConnectionIco { get; private set; }
    }
}
