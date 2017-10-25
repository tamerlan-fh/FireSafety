namespace FireSafety.Models
{
    /// <summary>
    /// связь между этажами
    /// </summary>

    class FloorsConnectionSection : Section
    {
        public FloorsConnectionSection(Node first, Node last, Floor parent) : base("Спуск", parent)
        {
            this.First = first;
            this.Last = last;
            First.AddSection(this);
            Last.AddSection(this);
            Width = -1;
            Length = 0;
            AutoSize = false;
        }
    }
}
