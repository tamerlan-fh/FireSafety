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
        double Speed { get; set; }

        /// <summary>
        /// Время движения 
        /// </summary>
        double MovementTime { get; set; }
    }
}
