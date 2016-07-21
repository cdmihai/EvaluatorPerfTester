using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace EvaluatorPerfTester
{
    class MSBuildTestProjectProvider
    {
        public List<string> GeneratedFiles { get; }
        private string BasePath { get; }
        //public Lazy<IEnumerable<int>> ItemCounts => new Lazy<IEnumerable<int>>(() => GetLinearItemCounts(1000, 1000, 21));
        public Lazy<IEnumerable<int>> ItemCounts => new Lazy<IEnumerable<int>>(() => GetExponentialItemCounts(10, 10, 5));

        public MSBuildTestProjectProvider()
        {
            GeneratedFiles = new List<string>();
            BasePath = Path.Combine(Path.GetTempPath(), "generatedMSBuildProjects");
            Directory.CreateDirectory(BasePath);
        }

        public void ConstructMSBuildProjects()
        {
            Console.WriteLine($"Generating projects in: {BasePath}");

            foreach (var itemCount in ItemCounts.Value)
            {
                var projectPath = ConstructMSbuildProject(itemCount, BasePath);
                GeneratedFiles.Add(projectPath);
            }
        }

        public void DeleteMSBuildProjects()
        {
            foreach (var generatedFile in GeneratedFiles)
            {
                File.Delete(generatedFile);
            }

            Directory.Delete(BasePath);
        }

        private string ConstructMSbuildProject(int itemCount, string basePath)
        {
            var projectContents = new MSBuildProjectConstructor(itemCount).Construct();
            var projectPath = Path.Combine(basePath, GetProjectName(itemCount));
            File.WriteAllText(projectPath, projectContents);

            return projectPath;
        }

        private static string GetProjectName(int itemCount)
        {
            return $"Project_With_{itemCount}_items.proj";
        }

        private IEnumerable<int> GetLinearItemCounts(int start, int step, int repetitions)
        {
            yield return 10;
            yield return 100;
            yield return 500;

            for (var i = 0; i < repetitions; i++)
            {
                yield return start + step*i;
            }
        }

        private IEnumerable<int> GetExponentialItemCounts(int start, int exponentBase, int repetitions)
        {
            for (var i = 0; i < repetitions; i++)
            {
                yield return start * (int) Math.Pow(exponentBase, i);
            }
        }
    }
}