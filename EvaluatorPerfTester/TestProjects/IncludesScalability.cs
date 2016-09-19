using System.Collections.Generic;

namespace EvaluatorPerfTester.TestProjects
{
    // Vary number of includes on one item type
    internal class IncludesScalability : GeneratedTestProject
    {
        private static readonly ProjectConfiguration Prototype = new ProjectConfiguration
        {
            ItemTypes = 1,
            IncludesPerItemType = 0,
            ItemsPerInclude = 1,
            ProbabilityToReferenceAnotherItem = 0
        };

        public IncludesScalability(string basePath) : base(basePath)
        {
        }

        public override string Name()
        {
            return nameof(IncludesScalability);
        }

        public override IEnumerable<ProjectConfiguration> GetConfigurations()
        {
            foreach (var includesPerItemType in ExponentialSequence(3, 10, 5))
            {
                var configuration = Prototype;
                configuration.IncludesPerItemType = includesPerItemType;

                yield return configuration;
            }
        }
    }
}