using System.Collections.Generic;

namespace EvaluatorPerfTester
{
    internal abstract class TestProject
    {
        protected readonly string _basePath;

        protected TestProject(string basePath)
        {
            _basePath = basePath;
        }

        public abstract void MaterializeProject();
        public abstract void DeleteProjects();
        public abstract string Name();
        public abstract IEnumerable<string> ProjectFiles();
        public abstract IEnumerable<string> GetProjectDescriptions();
    }
}