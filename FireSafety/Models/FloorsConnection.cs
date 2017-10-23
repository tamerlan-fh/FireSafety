namespace FireSafety.Models
{
    /// <summary>
    /// связь между этажами
    /// </summary>
    class FloorsConnection : Section
    {
        //public FloorsConnection(Node начало, Node конец, Floor parent) : base(начало, конец, parent, "Спуск")
        //{

        //}

        public FloorsConnection(Node first, Node last, Floor parent) : base("Спуск", parent)
        {
            this.First = first;
            this.Last = last;
            First.AddSection(this);
            Last.AddSection(this);
            Width = 1;
            Length = 0;
        }
    }
}
