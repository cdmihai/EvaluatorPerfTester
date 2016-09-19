using System.Collections.Generic;

namespace EvaluatorPerfTester.TestProjects
{
    internal class ItemReferencingScalabilityOnUniqueItemType : GeneratedTestProject
    {
        private static readonly ProjectConfiguration Prototype = new ProjectConfiguration
        {
            ItemTypes = 1,
            IncludesPerItemType = 0,
            ItemsPerInclude = 1,
            ProbabilityToReferenceAnotherItem = 0.5
        };

        public ItemReferencingScalabilityOnUniqueItemType(string basePath) : base(basePath)
        {
        }

        public override string Name()
        {
            return nameof(ItemReferencingScalabilityOnUniqueItemType);
        }

        public override IEnumerable<ProjectConfiguration> GetConfigurations()
        {
            foreach (var includesPerItemType in LinearSequence(1, 10, 4))
            {
                var configuration = Prototype;
                configuration.IncludesPerItemType = includesPerItemType;

                yield return configuration;
            }
        }
    }
}