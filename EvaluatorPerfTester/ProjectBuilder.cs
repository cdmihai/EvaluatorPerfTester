using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvaluatorPerfTester
{
    internal class MsBuildProjectConstructor
    {
        private static readonly Random Random = new Random();

        private ProjectConfiguration _configuration;

        //probabilities: item reuse; item referencing

        public MsBuildProjectConstructor(ProjectConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string XmlHeader => @"<?xml version=""1.0"" encoding=""utf-8""?>";

        private string ProjectStart
            =>
                @"<Project ToolsVersion=""12.0"" DefaultTargets=""BuildAndTest"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">"
            ;

        public string ProjectEnd => @"</Project>";
        private string ItemGroupStart => @"<ItemGroup>";
        public string ItemGroupEnd => @"</ItemGroup>";

        public string Construct()
        {
            var sb = new StringBuilder();

            sb.AppendLine(XmlHeader);
            sb.AppendLine(ProjectStart);
            sb.AppendLine(ItemGroupStart);

            var items = GenerateItems();

            foreach (var item in items)
            {
                sb.AppendLine(item);
            }

            sb.AppendLine(ItemGroupEnd);
            sb.AppendLine(ProjectEnd);

            return sb.ToString();
        }

        private IEnumerable<string> GenerateItems()
        {
            var items = new List<IncludeOperation>(_configuration.ItemTypes * _configuration.IncludesPerItemType);

            for (var itemTypeID = 0; itemTypeID < _configuration.ItemTypes; itemTypeID++)
            {
                var itemTypeName = GenerateItemName(itemTypeID);

                for (var includeNumber = 0; includeNumber < _configuration.IncludesPerItemType; includeNumber++)
                {
                    var includeSpec = GenerateIncludeSpec(itemTypeName, items);
                    items.Add(new IncludeOperation(itemTypeName, includeSpec));
                }
            }

            return items.Select(i => i.ToXml());
        }

        private string GenerateItemName(int itemId)
        {
            return $"Item_{itemId}";
        }

        private string GenerateIncludeSpec(string itemType, IList<IncludeOperation> previousOperations)
        {
            if (previousOperations.Any() && ReferenceItem())
            {
                var itemTypeToReference = previousOperations[Random.Next(0, previousOperations.Count - 1)].ItemType;
                return $"@({itemTypeToReference})";
            }

            var itemFragments = GenerateItemFragments(previousOperations.Count);

            return itemFragments
                .AsParallel()
                .Aggregate((i1, i2) => i1 + ";" + i2);
        }

        private IEnumerable<string> GenerateItemFragments(int itemId)
        {

            return Enumerable
                .Range(0, _configuration.ItemsPerInclude)
                .AsParallel()
                .Select(i => $"i_{itemId}_{i}");
        }

        private bool ReferenceItem()
        {
            return Random.NextDouble() < _configuration.ProbabilityToReferenceAnotherItem;
        }

        private class IncludeOperation
        {
            public IncludeOperation(string itemType, string includeSpec)
            {
                ItemType = itemType;
                IncludeSpec = includeSpec;
            }

            public string ItemType { get; }
            public string IncludeSpec { get; }

            public string ToXml()
            {
                return $"<{ItemType} Include=\"{IncludeSpec}\"/>";
            }
        }
    }
}