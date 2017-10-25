using System;
using System.Windows;

namespace FireSafety.IO
{
    [Serializable]
    abstract class SavedFile
    {
        public Type FileType { get; set; }
        public int HashCode { get; set; }
    }
    [Serializable]
    abstract class EntitySavedFile : SavedFile
    {
        public int ParentHashCode { get; set; }
        public string Title { get; set; }
    }

    [Serializable]
    class BuildingSavedFile : EntitySavedFile
    {
        public int[] FloorHashCodes { get; set; }
    }

    [Serializable]
    class NodeSavedFile : EntitySavedFile
    {
        public Point Position { get; set; }
        public bool AutoSize { get; set; }
    }

    [Serializable]
    class EntryNodeSavedFile : NodeSavedFile
    {
        public double Width { get; set; }
    }

    [Serializable]
    class StartNodeSavedFile : NodeSavedFile
    {
        public int PeopleCount { get; set; }
        public double ProjectionArea { get; set; }
    }

    [Serializable]
    class StairsNodeSavedFile : NodeSavedFile
    {
        public bool IsFloorsConnected { get; set; }
    }

    [Serializable]
    class FloorSavedFile : EntitySavedFile
    {
        public byte[] FloorPlanImage { get; set; }
        public int[] ObjectHashCodes { get; set; }
        public int Scale { get; set; }
    }

    [Serializable]
    class SectionSavedFile : EntitySavedFile
    {
        public double Length { get; set; }
        public double Width { get; set; }
        public int FirstNodeHashCode { get; set; }
        public int LastNodeHashCode { get; set; }
        public bool AutoSize { get; set; }
    }

    [Serializable]
    class ZoomToolSavedFile : SavedFile
    {
        public double ActualLength { get; set; }
        public double GraphicLength { get; set; }
    }
}
