using System.Collections.Generic;

namespace EvaluatorPerfTester.TestProjects
{
    // vary number of item types with one include
    internal class ItemCountFromMultipleTypesAndIncludes : GeneratedTestProject
    {
        private static readonly ProjectConfiguration Prototype = new ProjectConfiguration
        {
            ItemTypes = 0,
            IncludesPerItemType = 100,
            ItemsPerInclude = 10,
            ProbabilityToReferenceAnotherItem = 0
        };

        public ItemCountFromMultipleTypesAndIncludes(string basePath) : base(basePath)
        {
        }

        public override string Name()
        {
            return nameof(ItemCountFromMultipleTypesAndIncludes);
        }

        public override IEnumerable<ProjectConfiguration> GetConfigurations()
        {
            foreach (var itemTypes in ExponentialSequence(1, 10, 3))
            {
                var configuration = Prototype;
                configuration.ItemTypes = itemTypes;

                yield return configuration;
            }
        }
    }
}