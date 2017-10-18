namespace FireSafety.Models
{
    class УчастокПути : Section
    {
        private static int индекс = 1;
        public УчастокПути(Node начало, Node конец, Floor parent) : this(начало, конец, parent, string.Format("Участок пути {0}", индекс++)) { }
        public УчастокПути(Entity начало, Entity конец, Floor parent) : this(начало as Node, конец as Node, parent) { }
        public УчастокПути(Entity начало, Entity конец, Floor parent, string название) : this(начало as Node, конец as Node, parent, название) { }
        public УчастокПути(Node начало, Node конец, Floor parent, string название) : base(название, parent)
        {
            this.First = начало;
            this.Last = конец;
            First.AddSection(this);
            Last.AddSection(this);
            Width = 1;
            Length = (Last.Position - First.Position).Length / 100;
        }
    }
}
