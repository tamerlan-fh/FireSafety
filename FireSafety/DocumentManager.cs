using FireSafety.Models;
using Novacode;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FireSafety
{
    class DocumentManager
    {
        private static DocumentManager instance;
        public static DocumentManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new DocumentManager();
                return instance;
            }
        }
        private DocumentManager() { }

        private string filename = "document";
        public async Task<DocX> CreateDocument(EvacuationPlan plan)
        {
            return await Task.Factory.StartNew(() =>
             {
                 var document = DocX.Create(filename, DocumentTypes.Document);
                 var h = document.PageHeight;
                 var w = document.PageWidth;


                 document.InsertParagraph("Содержание").FontSize(15d).Bold().SpacingAfter(50d).Alignment = Alignment.center;
                 document.InsertTableOfContents(null, TableOfContentsSwitches.O | TableOfContentsSwitches.U | TableOfContentsSwitches.Z | TableOfContentsSwitches.H, "Normal", 2);

                 AddPageBreak(document);

                 document.InsertParagraph("Наименование использованной методики").SpacingBefore(0d).Heading(HeadingType.Heading1);
                 var contextMethodologies = @"Для определения расчетных величин пожарного риска на объекте защиты использована «Методика определения расчетных величин пожарного риска на производственных объектах», утвержденная приказом МЧС РФ № 404 от 10.07.2009 г";
                 document.InsertParagraph(contextMethodologies).SpacingBefore(10d).Alignment = Alignment.both;

                 AddPageBreak(document);

                 document.InsertParagraph("Описание объекта защиты, в отношении которого проводилась оценка пожарного риска").SpacingBefore(0d).Heading(HeadingType.Heading1);
                 SetDescriptionPlan(document, plan);

                 AddPageBreak(document);

                 document.InsertParagraph("Результаты проведения расчетов по оценке пожарного риска").SpacingBefore(0d).Heading(HeadingType.Heading1);
                 document.InsertParagraph("Результат определения расчетного времени эвакуации").SpacingBefore(10d).Heading(HeadingType.Heading2);
                 SetTablesOfRouteInformation(document, plan);
                 AddPageBreak(document);

                 document.InsertParagraph("Результат расчета индивидуального пожарного риска в здании").SpacingBefore(0d).Heading(HeadingType.Heading2);
                 ResultFireRiskInformation(document);

                 AddPageBreak(document);

                 document.InsertParagraph("Перечень исходных данных и используемых справочных источников информации").SpacingBefore(0d).Heading(HeadingType.Heading1);
                 SetListOfInitialData(document);

                 AddPageBreak(document);

                 document.InsertParagraph("Вывод об условиях соответствия (несоответствия) объекта защиты требованиям пожарной безопасности").SpacingBefore(0d).SpacingAfter(10d).Heading(HeadingType.Heading1);

                 document.InsertParagraph(string.Format(conclusionPattern1, plan.Param1 ? "не " : string.Empty)).Alignment = Alignment.both;
                 document.InsertParagraph(string.Format(conclusionPattern2, plan.Param2 ? "не " : string.Empty)).Alignment = Alignment.both;
                 return document;
             });
        }

        private void AddPageBreak(DocX document)
        {
            var paragraph = document.InsertParagraph(string.Empty, false);
            paragraph.InsertPageBreakAfterSelf();
        }

        private void SetDescriptionPlan(DocX document, EvacuationPlan plan)
        {

            foreach (var floor in plan.ParentBuilding.Floors)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var planImage = floor.GetEvacuationPlanImage();

                    using (var ms = new MemoryStream(Settings.GetBytesFromBitmap(planImage.BitmapValue)))
                    {
                        // double width = 0.9 * document.PageWidth;
                        double width = 672;
                        double height = planImage.Height * width / planImage.Width;

                        var image = document.AddImage(ms);
                        var picture = image.CreatePicture((int)height, (int)width);

                        var paragraph = document.InsertParagraph(string.Format("Схема эвакуации '{0}'", floor.Title), false);
                        paragraph.InsertPicture(picture, 0); // Insert picture into paragraph.
                    }
                });
            }
        }

        private void SetTablesOfRouteInformation(DocX document, EvacuationPlan plan)
        {
            var headerFormat = new Formatting() { Bold = true, Size = 12 };
            var normalFormat = new Formatting() { Bold = false, Size = 12 };

            foreach (var route in plan.Routes)
            {
                document.InsertParagraph(route.Title).Heading(HeadingType.Heading3);
                var rowsCount = route.Sections.Count - route.Sections.Count(x => x is FloorsConnectionSection) + 1;
                var table = document.AddTable(rowsCount, 7);

                table.Rows[0].Cells[0].Paragraphs.First().InsertText("Участок", false, headerFormat);
                table.Rows[0].Cells[1].Paragraphs.First().InsertText("Длина, м", false, headerFormat);
                table.Rows[0].Cells[2].Paragraphs.First().InsertText("Ширина, м", false, headerFormat);
                table.Rows[0].Cells[3].Paragraphs.First().InsertText("Площадь, м2", false, headerFormat);
                table.Rows[0].Cells[4].Paragraphs.First().InsertText("Интенсивность движения людского потока, м/мин", false, headerFormat);
                table.Rows[0].Cells[5].Paragraphs.First().InsertText("Скорость движения людского потока, м/мин", false, headerFormat);
                table.Rows[0].Cells[6].Paragraphs.First().InsertText("Время прохождения участка, мин", false, headerFormat);

                var rowIndex = 1;
                foreach (var section in route.Sections)
                {
                    if (section is FloorsConnectionSection) continue;
                    table.Rows[rowIndex].Cells[0].Paragraphs.First().InsertText(section.Title, false, normalFormat);
                    table.Rows[rowIndex].Cells[1].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.Length, 3)), false, normalFormat);
                    table.Rows[rowIndex].Cells[2].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.Width, 3)), false, normalFormat);
                    table.Rows[rowIndex].Cells[4].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.IntensityHumanFlow, 3)), false, normalFormat);
                    table.Rows[rowIndex].Cells[5].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.MovementSpeed, 3)), false, normalFormat);
                    table.Rows[rowIndex].Cells[6].Paragraphs.First().InsertText(string.Format("{0}", Math.Round(section.MovementTime, 3)), false, normalFormat);
                    rowIndex++;
                }

                document.InsertTable(table);
                document.InsertParagraph(string.Format("Общее время t: {0} ", TimeSpan.FromMinutes(route.MovementTime)), false, headerFormat);
            }
        }

        private void ResultFireRiskInformation(DocX document)
        {

        }

        private void SetListOfInitialData(DocX document)
        {
            var context1 = @"Использована аналитическая модель движения людского потока (определение расчетного времени эвакуации людей из помещений здания по расчету времени движения одного или нескольких людских потоков через эвакуационные выходы от наиболее удаленных мест размещения людей). Расчетное время эвакуации людей tр  установлено по расчету времени движения нескольких людских потоков через эвакуационные выходы от наиболее удаленных мест размещения людей. При расчете весь путь движения людского потока разделен на участки длиной li и шириной δi. Начальными участками являются проходы между рабочими местами, оборудованием. При определении расчетного времени эвакуации людей длина и ширина каждого участка пути эвакуации для проектируемого пожарного отсека здания принята по проекту.";
            document.InsertParagraph(context1).SpacingBefore(10d).Alignment = Alignment.both;

            document.InsertParagraph("Принято при расчете:").SpacingBefore(10d).Alignment = Alignment.both;

            var list1 = document.AddList("все нарушения нормативных документов пожарной безопасности на путях эвакуации и эвакуационных выходах, за пределами проектируемого участка, устранены;", 0, ListItemType.Bulleted);
            document.AddListItem(list1, "ширина выходов из лестничной клетки (ЛК-2 по техническому паспорту) принята не менее 1200 мм в свету, ширина всех эвакуационных выходов в свету из помещений, не входящих в проектируемый участок, не менее 800 мм;", 0);
            document.AddListItem(list1, "все двери помещений рассчитываемого пожарного отсека здания без нормируемого предела огнестойкости и при расчете времени блокирования путей эвакуации приравнены к открытым дверным проемам, так как двери без нормируемого предела огнестойкости не являются  преградой для опасных факторов пожара;", 0);
            document.AddListItem(list1, "двери выходов в коридоры при эвакуации из них менее 15 человек в помещениях, не входящих в проектируемый участок, установлены открывающимися вовнутрь для повышения расчетной ширины пути эвакуации;", 0);
            document.AddListItem(list1, "в помещениях, не входящих в проектируемый участок, оборудование и материалы расположены таким образом, что эвакуация  людей из них организуется  из центра помещений;", 0);
            document.AddListItem(list1, "все средства противопожарной защиты запроектированы согласно норм и требований по пожарной безопасности и имеют сертификаты пожарной безопасности.", 0);

            document.InsertList(list1);

            document.InsertParagraph("Справочные источники информации:").SpacingBefore(10d).SpacingAfter(10d).Heading(HeadingType.Heading2);

            var list2 = document.AddList("все нарушения нормативных документов пожарной безопасности на путях эвакуации и эвакуационных выходах, за пределами проектируемого участка, устранены;", 0, ListItemType.Numbered);
            document.AddListItem(list2, "Федеральный закон Российской Федерации от 22 июля 2008 г. №123-ФЗ «Технический регламент о требованиях пожарной безопасности».", 0);
            document.AddListItem(list2, "Постановление Правительства Российской Федерации от 31 марта 2009 г. № 272 «О порядке проведения расчетов по оценке пожарного риска»", 0);
            document.AddListItem(list2, "Приказ МЧС России от 10 июля 2009 г. № 404 «Методика определения расчетных величин пожарного риска на производственных объектах» (Зарегистрировано в Минюсте РФ 17 августа 2009 г. Регистрационный N 14541).", 0);
            document.AddListItem(list2, "ГОСТ 12.1.004-91*. ССБТ. Пожарная безопасность. Общие требования.", 0);
            document.AddListItem(list2, "ГОСТ Р 12.3.047-98. ССБТ. Пожарная безопасность технологических процессов. Общие требования. Методы контроля.", 0);
            document.AddListItem(list2, "СП 1.13130.2009. Системы противопожарной защиты. Эвакуационные пути и выходы.", 0);
            document.AddListItem(list2, "СП 131.13330.2012. Строительная климатология. Актуализированная версия СНиП 23-01-99.", 0);
            document.AddListItem(list2, "Пожаровзрывоопасность веществ и материалов и средства их тушения: Справ. изд.: в 2 книгах; кн. 2 / А.Н.Баратов, А.Я.Корольченко, Г.Н.Кравчук и др. – М., Химия, 1990. – 384 с.", 0);
            document.AddListItem(list2, "Пожаровзрывоопасность веществ и материалов и средства их тушения: Справ. изд.: в 2 книгах; кн. 1 / А.Н.Баратов, А.Я.Корольченко, Г.Н.Кравчук и др. – М., Химия, 1990. – 496 с.", 0);
            document.InsertList(list2);
        }

        private string conclusionPattern1 = @"Эвакуационные пути на проектируемом участке {0}обеспечивают безопасную эвакуацию людей. Для обоснования проектных решений по обеспечению безопасности людей при возникновении пожара выполнены расчеты необходимого времени эвакуации. Интервал времени от момента обнаружения пожара до завершения процесса эвакуации людей не превышает необходимого времени эвакуации людей при пожаре";
        private string conclusionPattern2 = @"Величина индивидуального пожарного риска в проектируемом пожарном отсеке здания составляет 3ˑ10-7 год-1/чел., что {0}превышает одной миллионной в год и соответствует требованиям части 1 статьи 93 федерального закона Российской Федерации от 22 июля 2008 г. №123-ФЗ «Технический регламент о требованиях пожарной безопасности»";
    }
}
