using FireSafety.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FireSafety.IO
{
    class SaveBuildingManager
    {
        public static SaveBuildingManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SaveBuildingManager();
                return instance;
            }
        }
        private static SaveBuildingManager instance;
        private SaveBuildingManager() { }

        public void Save(Building building, string filename)
        {
            var files = new List<SavedFile>();

            files.Add(BuildingSave(building));

            foreach (var floor in building.Floors)
            {
                files.Add(SaveFloor(floor));
                files.Add(SaveZoomTool(floor.Scale));

                foreach (var obj in floor.Objects)
                {
                    if (obj is StartNode) { files.Add(SaveStartNode(obj as StartNode)); continue; }
                    if (obj is EntryNode) { files.Add(SaveEntryNode(obj as EntryNode)); continue; }
                    if (obj is StairsNode) { files.Add(SaveStairsNode(obj as StairsNode)); continue; }
                    if (obj is Node) { files.Add(SaveNode(obj as Node)); continue; }
                    if (obj is Section) { files.Add(SaveSection(obj as Section)); continue; }
                }
            }

            var formatter = new BinaryFormatter();
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, files);
            }
        }

        private BuildingSavedFile BuildingSave(Building building)
        {
            return new BuildingSavedFile()
            {
                FileType = building.GetType(),
                HashCode = building.GetHashCode(),
                Title = building.Title,
                FloorHashCodes = building.Floors.Select(f => f.GetHashCode()).ToArray()
            };
        }
        private FloorSavedFile SaveFloor(Floor floor)
        {
            return new FloorSavedFile()
            {
                FileType = floor.GetType(),
                HashCode = floor.GetHashCode(),
                Scale = floor.Scale.GetHashCode(),
                Title = floor.Title,
                ObjectHashCodes = floor.Objects.Select(f => f.GetHashCode()).ToArray(),
                FloorPlanImage = Settings.GetBytesFromBitmapImage(floor.FloorPlanImage)
            };
        }
        private ZoomToolSavedFile SaveZoomTool(ZoomTool scale)
        {
            return new ZoomToolSavedFile()
            {
                FileType = scale.GetType(),
                HashCode = scale.GetHashCode(),
                ActualLength = scale.ActualLength,
                GraphicLength = scale.GraphicLength
            };
        }
        private StartNodeSavedFile SaveStartNode(StartNode node)
        {
            return new StartNodeSavedFile()
            {
                FileType = node.GetType(),
                HashCode = node.GetHashCode(),
                AutoSize = node.AutoSize,
                Position = node.Position,
                Title = node.Title,
                ParentHashCode = node.Parent.GetHashCode(),
                ProjectionArea = node.ProjectionArea,
                PeopleCount = node.PeopleCount
            };
        }
        private EntryNodeSavedFile SaveEntryNode(EntryNode node)
        {
            return new EntryNodeSavedFile()
            {
                FileType = node.GetType(),
                HashCode = node.GetHashCode(),
                AutoSize = node.AutoSize,
                Position = node.Position,
                Title = node.Title,
                ParentHashCode = node.Parent.GetHashCode(),
                Width = node.Width
            };
        }
        private StairsNodeSavedFile SaveStairsNode(StairsNode node)
        {
            return new StairsNodeSavedFile()
            {
                FileType = node.GetType(),
                HashCode = node.GetHashCode(),
                AutoSize = node.AutoSize,
                Position = node.Position,
                Title = node.Title,
                ParentHashCode = node.Parent.GetHashCode(),
                IsFloorsConnected = node.IsFloorsConnected
            };
        }

        private NodeSavedFile SaveNode(Node node)
        {
            return new NodeSavedFile()
            {
                FileType = node.GetType(),
                HashCode = node.GetHashCode(),
                AutoSize = node.AutoSize,
                Position = node.Position,
                Title = node.Title,
                ParentHashCode = node.Parent.GetHashCode()
            };
        }
        private SectionSavedFile SaveSection(Section section)
        {
            return new SectionSavedFile()
            {
                FileType = section.GetType(),
                HashCode = section.GetHashCode(),
                AutoSize = section.AutoSize,
                Title = section.Title,
                ParentHashCode = section.Parent.GetHashCode(),
                FirstNodeHashCode = section.First.GetHashCode(),
                LastNodeHashCode = section.Last.GetHashCode(),
                Length = section.Length,
                Width = section.Width
            };
        }
        public Building Load(string filename)
        {
            var formatter = new BinaryFormatter();
            var files = new List<SavedFile>();

            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                files = (List<SavedFile>)formatter.Deserialize(fs);
            }

            Type t = typeof(Building);

            var sBuilding = (BuildingSavedFile)files.FirstOrDefault(x => x.FileType == typeof(Building));
            if (sBuilding == null) return null;

            Building building = new Building(sBuilding.Title);
            var dictionary = new Dictionary<int, dynamic>();

            foreach (var file in files.Where(x => x.FileType == typeof(ZoomTool)))
            {
                var sScale = file as ZoomToolSavedFile;
                dictionary.Add(sScale.HashCode, new ZoomTool(sScale.ActualLength, sScale.GraphicLength));
            }


            foreach (var file in files.Where(x => x.FileType == typeof(Floor)))
            {
                var sFloor = file as FloorSavedFile;
                var floor = new Floor(sFloor.Title, building)
                {
                    Scale = dictionary[sFloor.Scale],
                    FloorPlanImage = Settings.GetBitmapImage(sFloor.FloorPlanImage)
                };
                dictionary.Add(sFloor.HashCode, floor);
                building.AddFloor(floor);
            }



            foreach (var file in files)
            {
                if (file is StartNodeSavedFile)
                {
                    var sNode = file as StartNodeSavedFile;
                    Floor floor = dictionary[sNode.ParentHashCode];

                    var node = new StartNode(floor, sNode.Position, sNode.Title)
                    {
                        PeopleCount = sNode.PeopleCount,
                        ProjectionArea = sNode.ProjectionArea,
                        AutoSize = sNode.AutoSize
                    };
                    dictionary.Add(sNode.HashCode, node);
                    floor.AddObject(node);
                    continue;
                }
                if (file is EntryNodeSavedFile)
                {
                    var sNode = file as EntryNodeSavedFile;
                    Floor floor = dictionary[sNode.ParentHashCode];

                    var node = new EntryNode(floor, sNode.Position, sNode.Title)
                    {
                        AutoSize = sNode.AutoSize,
                        Width = sNode.Width
                    };
                    dictionary.Add(sNode.HashCode, node);
                    floor.AddObject(node);
                    continue;
                }

                if (file is StairsNodeSavedFile)
                {
                    var sNode = file as StairsNodeSavedFile;
                    Floor floor = dictionary[sNode.ParentHashCode];

                    var node = new StairsNode(floor, sNode.Position, sNode.Title)
                    {
                        AutoSize = sNode.AutoSize,
                        IsFloorsConnected = sNode.IsFloorsConnected
                    };
                    dictionary.Add(sNode.HashCode, node);
                    floor.AddObject(node);
                    continue;
                }

                if (file is NodeSavedFile)
                {
                    var sNode = file as NodeSavedFile;
                    Floor floor = dictionary[sNode.ParentHashCode];
                    Node node = null;
                    if (sNode.FileType == typeof(ExitNode))
                        node = new ExitNode(floor, sNode.Position, sNode.Title) { AutoSize = sNode.AutoSize };
                    if (sNode.FileType == typeof(RoadNode))
                        node = new RoadNode(floor, sNode.Position, sNode.Title) { AutoSize = sNode.AutoSize };

                    dictionary.Add(sNode.HashCode, node);
                    floor.AddObject(node);
                    continue;
                }

                if (file is SectionSavedFile)
                {
                    var sSection = file as SectionSavedFile;
                    Floor floor = dictionary[sSection.ParentHashCode];
                    Section section = null;
                    if (sSection.FileType == typeof(RoadSection))
                        section = new RoadSection((Node)dictionary[sSection.FirstNodeHashCode], (Node)dictionary[sSection.LastNodeHashCode], floor, sSection.Title)
                        {
                            AutoSize = sSection.AutoSize,
                            Length = sSection.Length,
                            Width = sSection.Width
                        };
                    else if (sSection.FileType == typeof(StairsSection))
                        section = new StairsSection((Node)dictionary[sSection.FirstNodeHashCode], (Node)dictionary[sSection.LastNodeHashCode], floor, sSection.Title)
                        {
                            AutoSize = sSection.AutoSize,
                            Length = sSection.Length,
                            Width = sSection.Width
                        };
                    else if (sSection.FileType == typeof(FloorsConnectionSection)) continue; // следует произвести добавление в самом конце

                    dictionary.Add(sSection.HashCode, section);
                    floor.AddObject(section);
                    continue;
                }
            }

            foreach (var file in files.Where(x=> x.FileType== typeof(FloorsConnectionSection)))
            {
                var sSection = file as SectionSavedFile;
                Floor floor = dictionary[sSection.ParentHashCode];
                var section = new FloorsConnectionSection((Node)dictionary[sSection.FirstNodeHashCode], (Node)dictionary[sSection.LastNodeHashCode], floor);
                dictionary.Add(sSection.HashCode, section);
                floor.AddObject(section);
            }

            return building;
        }
    }
}
