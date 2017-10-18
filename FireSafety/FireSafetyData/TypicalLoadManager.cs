﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSafety.FireSafetyData
{
    class ТаблицаТиповаяНагрузка
    {
        private static ТаблицаТиповаяНагрузка instance;
        public static ТаблицаТиповаяНагрузка Instance
        {
            get
            {
                if (instance == null)
                    instance = new ТаблицаТиповаяНагрузка();
                return instance;
            }
        }
        public List<ТиповаяНагрузка> ТиповыеНагрузки { get; private set; }
        private ТаблицаТиповаяНагрузка()
        {
            ТиповыеНагрузки = new List<ТиповаяНагрузка>();
            #region Добавление данных
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Здания I-II ст. огнест.; мебель+бытовые изделия", 13.8000000000, 0.0108000000, 0.0145000000, 270.0000000000, 1.0300000000, 0.2030000000, 0.0022000000, 0.0140000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Здания I-II ст. огнест.; мебель+ткани", 14.7000000000, 0.0108000000, 0.0145000000, 82.0000000000, 1.4370000000, 1.2850000000, 0.0022000000, 0.0060000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Здания III-IV ст. огнест.; мебель+бытовые изделия", 13.8000000000, 0.0465000000, 0.0344000000, 270.0000000000, 1.0300000000, 0.2030000000, 0.0022000000, 0.0140000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Здание III-IV ст. огнест.; мебель+ткани", 14.7000000000, 0.0465000000, 0.0344000000, 82.0000000000, 1.4370000000, 1.2850000000, 0.0022000000, 0.0060000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Здание I ст. огнест.; мебель+ткани (0.75+0.25)", 14.9000000000, 0.0125000000, 0.0162000000, 58.5000000000, 1.4370000000, 1.3200000000, 0.0193000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Здание III ст. огнест.; мебель+ткани (0.75+0.25)", 14.9000000000, 0.0400000000, 0.0162000000, 58.5000000000, 1.4370000000, 1.3200000000, 0.0193000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Кабинет; мебель+бумага (0.75+0.25)", 14.0020000000, 0.0420000000, 0.0129000000, 53.0000000000, 1.1610000000, 0.6420000000, 0.0317000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Помещение. облицованное панелями; панели ДВП", 18.1000000000, 0.0405000000, 0.0143000000, 130.0000000000, 1.1500000000, 0.6860000000, 0.0215000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Админ. помещение; мебель+бумага (0.75+0.25)", 14.0020000000, 0.0220000000, 0.0210000000, 53.0000000000, 1.1610000000, 1.4340000000, 0.0430000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Лекарств. препарат; этил. спирт+глицерин (0.95+0.05)", 26.6000000000, 812.8000000000, 0.0330000000, 88.1000000000, 2.3040000000, 1.9120000000, 0.2620000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Обществ. здания; мебель+линолеум ПВХ (0.9+0.1)", 14.0000000000, 0.0150000000, 0.0137000000, 47.7000000000, 1.3690000000, 1.4780000000, 0.0300000000, 0.0058000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Библиотеки. архивы; книги. журналы на стеллажах", 14.5000000000, 0.0103000000, 0.0110000000, 49.5000000000, 1.1540000000, 1.1087000000, 0.0974000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Сценическая часть зрительного зала; древесина", 13.8000000000, 0.0368000000, 0.0145000000, 57.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Верхняя одежда; ворс. ткани (шерсть+нейлон)", 23.3000000000, 0.0835000000, 0.0130000000, 129.0000000000, 3.6980000000, 0.4670000000, 0.0145000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Резинотехн. изделия; резина. изделия из нее", 36.0000000000, 0.0184000000, 0.0112000000, 850.0000000000, 2.9900000000, 0.4160000000, 0.0150000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Окрашенные полы. стены; дерево+краска РХО (0.9+0.1)", 14.1000000000, 0.0151000000, 0.0145000000, 71.3000000000, 1.2180000000, 1.4700000000, 0.0349000000, 0.0010000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Выст. зал. мастерская; дерево+ткани+краски (0.9+0.09+0.01)", 14.0000000000, 0.0163000000, 0.0152000000, 53.0000000000, 1.2180000000, 1.4230000000, 0.0230000000, 0.0001000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Издательства. типографии", 15.4000000000, 0.0040000000, 0.0061000000, 41.0000000000, 1.1580000000, 0.7710000000, 0.1690000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Каюта с синтет. отделкой; дерево+ткани+отделка", 15.8000000000, 0.0177000000, 0.0150000000, 133.5000000000, 1.2490000000, 0.8450000000, 0.0425000000, 0.0230000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Мебель; дерево+облицовка (0.9+0.1)", 14.4000000000, 0.0154000000, 0.0135000000, 84.1000000000, 1.2880000000, 1.5500000000, 0.0367000000, 0.0036000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Промтовары; текстильные изделия", 16.7000000000, 0.0071000000, 0.0244000000, 60.6000000000, 2.5600000000, 0.8790000000, 0.0626000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Кабельный подвал/поток; кабели АВВГ+АПВГ", 30.7000000000, 0.0071000000, 0.0244000000, 521.0000000000, 2.1900000000, 0.6500000000, 0.1295000000, 0.0202000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Химическое вещество; толуол", 40.9000000000, 860.0000000000, 0.0430000000, 562.0000000000, 3.0980000000, 3.6770000000, 0.1480000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Химическое вещество; ксилол", 41.2000000000, 860.0000000000, 0.0900000000, 402.0000000000, 3.6230000000, 3.6570000000, 0.1480000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Бензин А-76", 43.2000000000, 745.0000000000, 0.0585000000, 256.0000000000, 3.4050000000, 2.9200000000, 0.1750000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Керосин", 43.3000000000, 794.0000000000, 0.0415000000, 438.1000000000, 3.3410000000, 2.9200000000, 0.1480000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Дизельное топливо; соляр", 45.4000000000, 853.0000000000, 0.0425000000, 620.1000000000, 3.3680000000, 3.1630000000, 0.1220000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Радиоматериалы; поли(этилен. стирол. пропил). гетинакс", 34.8000000000, 0.0137000000, 0.0177000000, 381.0000000000, 3.3120000000, 0.7640000000, 0.1000000000, 0.0073000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Электротехн. материалы; текстолит. карболит", 20.9000000000, 0.0125000000, 0.0076000000, 327.0000000000, 1.9500000000, 0.3750000000, 0.0556000000, 0.0054000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Теплоноситель; турбинное масло ТП-22", 41.9000000000, 883.0000000000, 0.0300000000, 243.0000000000, 0.2820000000, 0.7000000000, 0.1220000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Электрокабель АВВГ; ПВХ оболочка+изоляция", 25.0000000000, 0.0071000000, 0.0244000000, 635.0000000000, 2.1900000000, 0.3980000000, 0.1090000000, 0.0245000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Электрокабель АПВГ; ПВХ-оболочка+полиэтилен", 36.4000000000, 0.0071000000, 0.0244000000, 407.0000000000, 2.1900000000, 0.9030000000, 0.1500000000, 0.0160000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Телефонный кабель ТВП; ПВХ+полиэтилен", 34.6000000000, 0.0022000000, 0.0085000000, 556.0000000000, 2.1900000000, 0.9030000000, 0.1240000000, 0.0156000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Сырье для нефтехимии; нефть", 44.2000000000, 885.0000000000, 0.0241000000, 438.0000000000, 3.2400000000, 3.1040000000, 0.1610000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Химическое вещество: ацетон", 29.0000000000, 790.0000000000, 0.0440000000, 80.0000000000, 2.2200000000, 2.2930000000, 0.2690000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Химическое вещество; этиловый спирт", 27.5000000000, 789.3000000000, 0.0310000000, 80.0000000000, 2.3620000000, 1.9370000000, 0.2690000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Лесопильный цех I-III ст. огнест.; древесина", 13.8000000000, 0.0396000000, 0.0145000000, 57.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Лесопильный цех IV-V ст. огнест.; древесина", 13.8000000000, 0.0583000000, 0.0145000000, 57.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Цех деревообработки; древесина", 13.8000000000, 0.0220000000, 0.0145000000, 57.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Цех сушки древесины; древесина", 13.8000000000, 0.0375000000, 0.0145000000, 57.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Производство фанеры; древесина+фанера (0.5+0.5)", 16.1000000000, 0.0191000000, 0.0117000000, 80.5000000000, 1.1770000000, 1.0550000000, 0.0720000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Штабель древесины; хвойный+лиственный лес", 13.8000000000, 0.0585000000, 0.0145000000, 57.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Хвойные древесные стройматериалы; штабель", 13.8000000000, 0.0585000000, 0.0063000000, 61.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Лиственные древесные стройматериалы; штабель", 13.8000000000, 0.0585000000, 0.0140000000, 53.0000000000, 1.1500000000, 1.5700000000, 0.0240000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Клееные стройматериалы; фанера", 18.4000000000, 0.0167000000, 0.0089000000, 104.0000000000, 1.2050000000, 0.5400000000, 0.1210000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Сырье для легкой промышл.; хлопок разрыхл.", 16.4000000000, 0.0445000000, 0.0213000000, 0.6000000000, 2.3000000000, 0.5700000000, 0.0052000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Сырье для легкой промышл.; лен разрыхл.", 15.7000000000, 0.0500000000, 0.0213000000, 3.3700000000, 1.8300000000, 0.3600000000, 0.0039000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Сырье для легкой промышл.; хлопок+капрон (0.75+0.25)", 15.7000000000, 0.0280000000, 0.0125000000, 4.3000000000, 3.5500000000, 1.0450000000, 0.0120000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Сырье для легкой промышл.; шерсть", 21.8000000000, 0.0280000000, 0.0200000000, 164.0000000000, 1.7590000000, 0.7150000000, 0.0153000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Пищ. промышл.; пшеница. рис. гречиха и мука из них", 17.0000000000, 0.0050000000, 0.0080000000, 1096.0000000000, 0.9680000000, 0.8120000000, 0.1630000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Сырье и изделия из синтетического каучука", 43.0000000000, 0.0143000000, 0.0110000000, 212.0000000000, 2.9850000000, 1.4080000000, 0.1500000000, 0.0050000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Склад льноволокна", 15.7000000000, 0.0710000000, 0.0213000000, 3.4000000000, 1.8300000000, 0.3600000000, 0.0039000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Склад хлопка в тюках", 16.7000000000, 0.0042000000, 0.0167000000, 0.6000000000, 1.1500000000, 0.5780000000, 0.0052000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Склад бумаги в рулонах", 15.1000000000, 0.0050000000, 0.0080000000, 41.0000000000, 1.1580000000, 0.6635000000, 0.1077000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Провода в резиновой изоляции типа КПРТ. ПТ. ВПРС", 37.8000000000, 0.0050000000, 0.1917000000, 850.0000000000, 2.9900000000, 0.4160000000, 0.0150000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Склад оргстекла (ПММА)", 26.4000000000, 0.0080000000, 0.0041000000, 78.0000000000, 2.0900000000, 1.7950000000, 0.1266000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Кабели+провода; 0.75* (АВВГ. АПВГ. ТПВ)+0.25*(КПРТ. ПР. ШРПС)", 33.5000000000, 0.0054000000, 0.0622000000, 612.0000000000, 2.3890000000, 0.6550000000, 0.0995000000, 0.0140000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Дерево+лак. покрытие; 0.95*древесина+0.05*(ФЛ+РХО)", 13.9000000000, 0.0151000000, 0.0225000000, 64.1000000000, 1.1910000000, 0.7240000000, 0.0205000000, 0.0005000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Автомобиль; 0.3*(резина. бензин)+0.15*(ППУ. искожа ПВХ)+0.1*эмаль", 31.7000000000, 0.0068000000, 0.0233000000, 487.0000000000, 2.6400000000, 1.2950000000, 0.0970000000, 0.0109000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Зал; 0.5*ДВП+0.1*(ткань. искожа ПВХ. ППУ)+0.2*дерево с покрытием", 16.2000000000, 0.0293000000, 0.0123000000, 175.6000000000, 1.5740000000, 0.8170000000, 0.0410000000, 0.0143000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Тара: древесина+картон+полистирол (0.5+0.25+0.25)", 20.7100000000, 0.0100000000, 0.0180000000, 155.0000000000, 1.5200000000, 0.9700000000, 0.0940000000, 0.0046000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Упаковка: бумага+картон+(этилен+стирол) (0.4+0.3+0.15+0.15)", 23.5400000000, 0.0040000000, 0.0132000000, 172.0000000000, 1.7000000000, 0.6790000000, 0.1120000000, 0.0037000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Индустриальное масло", 42.7000000000, 920.0000000000, 0.0430000000, 480.0000000000, 1.5890000000, 1.0700000000, 0.1220000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Вешала текстильных изделий", 16.7200000000, 0.0078000000, 0.0245000000, 61.0000000000, 2.5600000000, 0.8790000000, 0.0630000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Отделка: ковролин", 15.3970000000, 0.0210000000, 0.0130000000, 150.0000000000, 2.5500000000, 1.2250000000, 0.2070000000, 0.0039000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Мебель+бумага (0.8)+ковровое покрытие (0.2)", 14.2800000000, 0.0340000000, 0.0129000000, 72.4000000000, 1.4390000000, 0.7590000000, 0.0680000000, 0.0008000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Занавес зрительного зала кинотеатра", 13.8000000000, 0.0500000000, 0.0115000000, 50.0000000000, 1.0300000000, 0.2030000000, 0.0022000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Жилые помещения гостиниц. общежитий и т.д.", 13.8000000000, 0.0045000000, 0.0145000000, 270.0000000000, 1.0300000000, 0.2030000000, 0.0022000000, 0.0140000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Столовая. зал ресторана и т.д.", 13.8000000000, 0.0045000000, 0.0145000000, 82.0000000000, 1.4370000000, 1.2850000000, 0.0022000000, 0.0060000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Зал театра. кинотеатра. клуба. цирка и т.д.", 13.8000000000, 0.0055000000, 0.0145000000, 270.0000000000, 1.0300000000, 0.2030000000, 0.0022000000, 0.0140000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Гардеробы", 16.7000000000, 0.0070000000, 0.0090000000, 61.0000000000, 2.5600000000, 0.8800000000, 0.0630000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Хранилища библиотек. архивы", 14.5000000000, 0.0060000000, 0.0110000000, 49.5000000000, 1.1540000000, 1.1087000000, 0.0974000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Музеи. выставки", 13.8000000000, 0.0055000000, 0.0145000000, 270.0000000000, 1.0300000000, 0.2030000000, 0.0022000000, 0.0140000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Подсобные и бытовые помещения", 14.0000000000, 0.0042000000, 0.0129000000, 53.0000000000, 1.1610000000, 0.6420000000, 0.0317000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Административные помещения. уч. Кл. школ. ВУЗов. каб. поликлиники", 14.0000000000, 0.0045000000, 0.0137000000, 47.7000000000, 1.3690000000, 1.4780000000, 0.0300000000, 0.0058000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Магазины", 15.8000000000, 0.0055000000, 0.0150000000, 270.0000000000, 1.2500000000, 0.8500000000, 0.0430000000, 0.0230000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Зал вокзала", 13.8000000000, 0.0055000000, 0.0145000000, 270.0000000000, 1.0300000000, 0.2030000000, 0.0022000000, 0.0140000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Стоянки легковых автомобилей", 31.7000000000, 0.0068000000, 0.0230000000, 487.0000000000, 2.6400000000, 1.3000000000, 0.0970000000, 0.0110000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Стоянки легковых автомобилей с двухуровневым хранением", 31.7000000000, 0.0136000000, 0.0230000000, 487.0000000000, 2.6400000000, 1.3000000000, 0.0970000000, 0.0110000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Стадионы", 26.4000000000, 0.0040000000, 0.0140000000, 78.0000000000, 2.0900000000, 1.8000000000, 0.1270000000, 0.0000000000));
            ТиповыеНагрузки.Add(new ТиповаяНагрузка("Спортзалы", 16.7000000000, 0.0045000000, 0.0140000000, 61.0000000000, 2.5600000000, 0.8800000000, 0.0630000000, 0.0000000000));
            #endregion
        }
    }
}
