namespace EvaluatorPerfTester
{
    internal struct ProjectConfiguration
    {
        public int ItemTypes;
        public int IncludesPerItemType;
        public int ItemsPerInclude;
        public double ProbabilityToReferenceAnotherItem;

        public override string ToString()
        {
            return
                ItemTypes + "_" +
                IncludesPerItemType + "_" +
                ItemsPerInclude + "_" +
                ProbabilityToReferenceAnotherItem;
        }
    }
}