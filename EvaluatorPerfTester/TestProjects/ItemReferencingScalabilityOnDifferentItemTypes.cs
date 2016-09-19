using System.Collections.Generic;

namespace EvaluatorPerfTester.TestProjects
{
    internal class ItemReferencingScalabilityOnDifferentItemTypes : GeneratedTestProject
    {
        private static readonly ProjectConfiguration Prototype = new ProjectConfiguration
        {
            ItemTypes = 0,
            IncludesPerItemType = 1,
            ItemsPerInclude = 1,
            ProbabilityToReferenceAnotherItem = 0.5
        };

        public ItemReferencingScalabilityOnDifferentItemTypes(string basePath) : base(basePath)
        {
        }

        public override string Name()
        {
            return nameof(ItemReferencingScalabilityOnDifferentItemTypes);
        }

        public override IEnumerable<ProjectConfiguration> GetConfigurations()
        {
            foreach (var itemTypes in LinearSequence(1, 10, 4))
            {
                var configuration = Prototype;
                configuration.ItemTypes = itemTypes;

                yield return configuration;
            }
        }
    }
}