using System.Collections.Generic;

namespace EvaluatorPerfTester.TestProjects
{
    // vary number of item types with one include
    internal class ItemCountFromOneTypeAndIncludeScalability : GeneratedTestProject
    {
        private static readonly ProjectConfiguration Prototype = new ProjectConfiguration
        {
            ItemTypes = 1,
            IncludesPerItemType = 1,
            ItemsPerInclude = 0,
            ProbabilityToReferenceAnotherItem = 0
        };

        public ItemCountFromOneTypeAndIncludeScalability(string basePath) : base(basePath)
        {
        }

        public override string Name()
        {
            return nameof(ItemCountFromOneTypeAndIncludeScalability);
        }

        public override IEnumerable<ProjectConfiguration> GetConfigurations()
        {
            foreach (var itemsPerInclude in ExponentialSequence(3, 10, 5))
            {
                var configuration = Prototype;
                configuration.ItemsPerInclude = itemsPerInclude;

                yield return configuration;
            }
        }
    }
}