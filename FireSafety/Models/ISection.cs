namespace FireSafety.Models
{
    /// <summary>
    /// Интерфейс единицы участка пути
    /// </summary>
    interface ISection
    {
        /// <summary>
        /// Длина
        /// </summary>
        double Length { get; }

        /// <summary>
        /// Ширина
        /// </summary>
        double Width { get; }

        /// <summary>
        /// Площадь
        /// </summary>
        double Area { get; }

        /// <summary>
        /// Начало участка
        /// </summary>
        Node First { get; }

        /// <summary>
        /// Конец участка
        /// </summary>
        Node Last { get; }

        /// <summary>
        /// Отсутствуют входящие
        /// </summary>
        bool NoIncoming { get; }

        /// <summary>
        /// Отсутствуют исходящие
        /// </summary>
        bool NoOutgoing { get; }

        /// <summary>
        /// Плотность людского потока 
        /// </summary>
        double DensityHumanFlow { get; set; }

        /// <summary>
        /// Интенсивность людского потока  
        /// </summary>
        double IntensityHumanFlow { get; set; }

        /// <summary>
        /// Скорость движения 
        /// </summary>
        double MovementSpeed { get; set; }

        /// <summary>
        /// Время движения 
        /// </summary>
        double MovementTime { get; set; }

        /// <summary>
        /// Время задержки (актуально при слияния участков)
        /// </summary>
        double DelayTime { get; set; }

        /// <summary>
        /// Тип участка
        /// </summary>
        SectionTypes SectionType { get; }

        /// <summary>
        /// Является участком, начало которого является началом пути эвакуации
        /// </summary>
        bool IsStartingSection { get; }
    }

    enum SectionTypes
    {
        /// <summary>
        /// Горизонтальный участок
        /// </summary>
        HorizontalSection,
        /// <summary>
        /// Дверной проем
        /// </summary>
        Doorway,
        /// <summary>
        /// Лестничный участок
        /// </summary>
        StaircaseSection,
        /// <summary>
        /// Другое - Игнорируется при вычислении
        /// </summary>
        Other
    }
}
