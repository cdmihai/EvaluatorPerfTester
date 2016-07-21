using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvaluatorPerfTester
{
    internal class MSBuildProjectConstructor
    {
        private int ItemCount { get; }
        private Dictionary<int, string> ItemNames { get; }
        public string XMLHeader => @"<?xml version=""1.0"" encoding=""utf-8""?>";

        private string ProjectStart
            =>
                @"<Project ToolsVersion=""12.0"" DefaultTargets=""BuildAndTest"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">"
            ;

        public string ProjectEnd => @"</Project>";
        private string ItemGroupStart => @"<ItemGroup>";
        public string ItemGroupEnd => @"</ItemGroup>";
        public Random Random { get; }
        public double ReferenceItemProbability => 0.4;

        //probabilities: item reuse; item referencing

        public MSBuildProjectConstructor(int itemCount)
        {
            ItemCount = itemCount;
            ItemNames = new Dictionary<int, string>();

            Random = new Random();
        }


        public string Construct()
        {
            var sb = new StringBuilder();

            sb.AppendLine(XMLHeader);

            sb.AppendLine(ProjectStart);

            sb.AppendLine(ItemGroupStart);

            for (var i = 0; i < ItemCount; i++)
            {
                sb.AppendLine(ConstructItem(i));
            }

            sb.AppendLine(ItemGroupEnd);

            sb.AppendLine(ProjectEnd);

            return sb.ToString();
        }


        private string ConstructItem(int itemId)
        {
            var itemName = GenerateItemName(itemId);
            var itemFragments = GenerateItemFragments(itemId);
            ItemNames[itemId] = itemName;

            var itemSpec = "";

            if (ReferenceItem())
            {
                var itemIdToReference = ItemNames.Keys.ElementAt(Random.Next(0, ItemNames.Count - 1));
                itemSpec = $"@({ItemNames[itemIdToReference]})";
            }
            else
            {
                itemSpec = itemFragments.Aggregate((i1, i2) => i1 + ";" + i2);
            }

            return $"<{itemName} Include=\"{itemSpec}\"/>";
        }

        private bool ReferenceItem()
        {
            return Random.NextDouble() < ReferenceItemProbability;
        }


        private string GenerateItemName(int itemId)
        {
            return $"Item_{itemId}";
        }

        private IEnumerable<string> GenerateItemFragments(int itemId)
        {
            for (var i = 0; i < 10; i++)
            {
                yield return $"i_{itemId}_{i}";
            }
        }
    }
}