using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EvaluatorPerfTester
{
    internal abstract class GeneratedTestProject : TestProject
    {
        private ProjectFileProvider _fileProvider;
        private IEnumerable<ProjectConfiguration> _projectConfigurations;

        protected GeneratedTestProject(string basePath) : base(basePath)
        {
        }

        public abstract IEnumerable<ProjectConfiguration> GetConfigurations();

        protected static IEnumerable<int> LinearSequence(int start, int sequenceBase, int repetitions)
        {
            for (int i = start; i <= repetitions; i++)
            {
                yield return sequenceBase*i;
            }
        }

        protected static IEnumerable<int> ExponentialSequence(int start, int exponentBase, int repetitions)
        {
            Debug.Assert(start <= repetitions);

            for (var i = start; i <= repetitions; i++)
            {
                yield return (int) Math.Pow(exponentBase, i);
            }
        }

        public override void MaterializeProject()
        {
            _projectConfigurations = GetConfigurations();
            _fileProvider = new ProjectFileProvider(_projectConfigurations);
            _fileProvider.BasePath = Path.Combine(_basePath, Name());
            _fileProvider.ConstructProjects();
        }

        public override IEnumerable<string> ProjectFiles()
        {
            return _fileProvider.GeneratedFiles;
        }

        public override IEnumerable<string> GetProjectDescriptions()
        {
            return _projectConfigurations.Select(c => c.ToString());
        }

        public override void DeleteProjects()
        {
            _fileProvider.DeleteProjects();
        }
    }
}