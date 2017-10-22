namespace FireSafety.Models
{
    /// <summary>
    /// участок пути
    /// </summary>
    class RoadSection : Section
    {
        private static int index = 1;
        public RoadSection(string title, Entity parent) : base(title, parent)
        {
        }

        public RoadSection(Node first, Node last, Floor parent) : this(first, last, parent, string.Format("Участок пути {0}", index++)) { }
        public RoadSection(Entity first, Entity last, Floor parent) : this(first as Node, last as Node, parent) { }
        public RoadSection(Entity first, Entity last, Floor parent, string title) : this(first as Node, last as Node, parent, title) { }
        public RoadSection(Node first, Node last, Floor parent, string title) : base(title, parent)
        {
            this.First = first;
            this.Last = last;
            First.AddSection(this);
            Last.AddSection(this);
            Width = 1;
            Length = (Last.Position - First.Position).Length / 100;
        }
    }
}
