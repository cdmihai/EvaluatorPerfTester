using System;
using System.Collections.Generic;
using System.IO;

namespace EvaluatorPerfTester
{
    internal class ProjectFileProvider
    {
        public ProjectFileProvider(IEnumerable<ProjectConfiguration> configurations)
        {
            Configurations = configurations;
            GeneratedFiles = new List<string>();
        }

        public IEnumerable<ProjectConfiguration> Configurations { get; }
        public List<string> GeneratedFiles { get; }

        internal string BasePath { get; set; } = "GeneratedProjects";

        public void ConstructProjects()
        {
            Directory.CreateDirectory(BasePath);

            Console.WriteLine($"Generating projects in: {BasePath}");

            foreach (var configuration in Configurations)
            {
                var projectPath = ConstructProject(configuration, BasePath);
                GeneratedFiles.Add(projectPath);
            }
        }

        private string ConstructProject(ProjectConfiguration configuration, string basePath)
        {
            var projectContents = new MsBuildProjectConstructor(configuration).Construct();
            var projectPath = Path.Combine(basePath, GetProjectName(configuration));
            File.WriteAllText(projectPath, projectContents);

            return projectPath;
        }

        public void DeleteProjects()
        {
            foreach (var generatedFile in GeneratedFiles)
            {
                File.Delete(generatedFile);
            }

            Directory.Delete(BasePath, true);
        }

        private static string GetProjectName(ProjectConfiguration configuration)
        {
            return $"Proj_{configuration}.proj";
        }
    }
}