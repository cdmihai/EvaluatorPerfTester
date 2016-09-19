using System.Collections.Generic;

namespace EvaluatorPerfTester.TestProjects
{
    // vary number of item types with one include
    internal class ItemTypeScalability : GeneratedTestProject
    {
        private static readonly ProjectConfiguration Prototype = new ProjectConfiguration
        {
            ItemTypes = 0,
            IncludesPerItemType = 1,
            ItemsPerInclude = 1,
            ProbabilityToReferenceAnotherItem = 0
        };

        public ItemTypeScalability(string basePath) : base(basePath)
        {
        }

        public override string Name()
        {
            return nameof(ItemTypeScalability);
        }

        public override IEnumerable<ProjectConfiguration> GetConfigurations()
        {
            foreach (var itemTypes in ExponentialSequence(3, 10, 5))
            {
                var configuration = Prototype;
                configuration.ItemTypes = itemTypes;

                yield return configuration;
            }
        }
    }
}