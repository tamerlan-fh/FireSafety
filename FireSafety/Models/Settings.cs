using FireSafety.Properties;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace FireSafety.Models
{
    /// <summary>
    /// Settings
    /// </summary>
    class Settings
    {
        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                    instance = new Settings();
                return instance;
            }
        }
        private Settings()
        {
            ДверьIco = GetBitmapImage(Resources.icons8_Дверь_64, ImageFormat.Png);
            ВыходIco = GetBitmapImage(Resources.icons8_Выход_64, ImageFormat.Png);
            СтартIco = GetBitmapImage(Resources.icons8_Группа_пользователей__мужчина_и_женщина_64, ImageFormat.Png);
            ЗданиеIco = GetBitmapImage(Resources.buildings, ImageFormat.Png);
            ЭтажIco = GetBitmapImage(Resources.icons8_Поэтажный_план_64, ImageFormat.Png);
            УчастокПутиIco = GetBitmapImage(Resources.Без_имени_1, ImageFormat.Png);
            ПутьIco = GetBitmapImage(Resources.icons8_Ломаная_кривая_64__2_, ImageFormat.Png);
            УзелIco = GetBitmapImage(Resources.icons8_Круг_64, ImageFormat.Png);
            УказательIco = GetBitmapImage(Resources.icons8_Курсор_рука_64, ImageFormat.Png);
            УдалитьIco = GetBitmapImage(Resources.icons8_Удалить_64, ImageFormat.Png);
            ЛестницаIco = GetBitmapImage(Resources.icons8_Лестница_64, ImageFormat.Png);
            ОгоньIco = GetBitmapImage(Resources.icons8_Огонь_46, ImageFormat.Png);
            //ЛинейкаIco = GetBitmapImage(Resources.icons8_Линейка_46, ImageFormat.Png);
            //СлоиIco = GetBitmapImage(Resources.icons8_Листы_46, ImageFormat.Png);
            //ДокументIco = GetBitmapImage(Resources.icons8_Документ_64, ImageFormat.Png);
            //СохранитьIco = GetBitmapImage(Resources.icons8_Сохранить_64, ImageFormat.Png);
            //РискIco = GetBitmapImage(Resources.icons8_Безопасность_проверена_64, ImageFormat.Png);
            //БлокировкаIco = GetBitmapImage(Resources.icons8_Вопрос_защиты_64, ImageFormat.Png);
        }
        public static BitmapImage GetBitmapImage(Bitmap bitmap)
        {
            return GetBitmapImage(bitmap, ImageFormat.Bmp);
        }
        public static BitmapImage GetBitmapImage(Image image)
        {
            return GetBitmapImage(new Bitmap(image));
        }
        public static BitmapImage GetBitmapImage(Bitmap bitmap, ImageFormat format)
        {
            using (MemoryStream memory = new MemoryStream())
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

        public BitmapImage ДверьIco { get; private set; }
        public BitmapImage ВыходIco { get; private set; }
        public BitmapImage СтартIco { get; private set; }
        public BitmapImage ЗданиеIco { get; private set; }
        public BitmapImage ЭтажIco { get; private set; }
        public BitmapImage УчастокПутиIco { get; private set; }
        public BitmapImage ПутьIco { get; private set; }
        public BitmapImage УзелIco { get; private set; }
        public BitmapImage УказательIco { get; private set; }
        public BitmapImage УдалитьIco { get; private set; }
        public BitmapImage ЛестницаIco { get; private set; }
        public BitmapImage ОгоньIco { get; private set; }
        //public BitmapImage ЛинейкаIco { get; private set; }
        //public BitmapImage СлоиIco { get; private set; }
        //public BitmapImage СохранитьIco { get; private set; }
        //public BitmapImage ДокументIco { get; private set; }
        //public BitmapImage РискIco { get; private set; }
        //public BitmapImage БлокировкаIco { get; private set; }
    }
}
