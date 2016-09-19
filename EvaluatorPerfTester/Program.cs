using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EvaluatorPerfTester.TestProjects;

namespace EvaluatorPerfTester
{
    internal class Program
    {
        private const int RepetitionsPerProject = 3;
        private static readonly string BasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly ICollection<TestProject> _testProjects = new TestProject[]
        {
            //new IncludesScalability(BasePath),
            //new ItemTypeScalability(BasePath),
            //new ItemCountFromOneTypeAndIncludeScalability(BasePath),
            new ItemCountFromMultipleTypesAndIncludes(BasePath), 
            new ItemReferencingScalabilityOnUniqueItemType(BasePath),
            new ItemReferencingScalabilityOnDifferentItemTypes(BasePath)
        };

        private static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                var oldMsBuild = args[0];
                var newMsBuild = args[1];
                RunAnalyses(oldMsBuild, newMsBuild);
                return;
            }

            switch (args[0])
            {
                case "--materialize-projects":
                    MaterializeProjects();
                    break;
                default:
                    PrintUsage();
                    break;
            }

            Console.ReadKey();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("PerfTester --materialize-projects | oldMSBuild newMsBuild ");
            Console.WriteLine("\toldMsBuild newMsBuild\t paths to the root of the two msbuilds to evaluate; Shold be already built for Debug");
            Console.WriteLine("\t--materialize-projects\t materialize projects but do not run any analysis");
        }

        private static void RunAnalyses(string oldMsBuild, string newMsBuild)
        {
            try
            {
                MaterializeProjects();

                Console.WriteLine();
                Console.WriteLine($"Old MSBuild: {oldMsBuild}");
                Console.WriteLine($"New MsBuild {newMsBuild}");

                foreach (var project in _testProjects)
                {
                    Console.WriteLine();
                    Console.WriteLine($"------{project.Name()}------");
                    RunEvaluationAnalysisFor(project, oldMsBuild, newMsBuild);
                }
            }
            finally
            {
                //DeleteProjects();
            }
        }

        private static void MaterializeProjects()
        {
            Console.WriteLine($"Materializing projects in {BasePath}");
            foreach (var project in _testProjects)
            {
                project.MaterializeProject();
            }
        }

        private static void DeleteProjects()
        {
            foreach (var project in _testProjects)
            {
                project.DeleteProjects();
            }
        }

        private static void RunEvaluationAnalysisFor(TestProject testProject, string oldMsBuild, string newMsBuild)
        {
            var csvHeader = new List<string> {"MSBuild.Under.Test"};
            var oldCsvLine = new List<string> {"old"};
            var newCsvLine = new List<string> {"new"};

            csvHeader.AddRange(testProject.GetProjectDescriptions());

            oldCsvLine.AddRange(TestEvaluation(oldMsBuild, testProject));
            newCsvLine.AddRange(TestEvaluation(newMsBuild, testProject));

            PrintCSV(testProject.Name(), csvHeader, oldCsvLine, newCsvLine);

            Console.WriteLine(oldCsvLine.Aggregate((s1, s2) => s1 + "\t;\t" + s2));
            Console.WriteLine(newCsvLine.Aggregate((s1, s2) => s1 + "\t;\t" + s2));
        }

        private static void PrintCSV(string fileName, params List<string>[] lines)
        {
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(line.Aggregate((e1, e2) => e1 + "," + e2));
            }

            var csvPath = Path.Combine(BasePath, $"{fileName}.csv");
            File.WriteAllText(csvPath, sb.ToString());
        }

        private static List<string> TestEvaluation(string pathToMSBuild, TestProject testProject)
        {
            var csvLine = new List<string>(testProject.ProjectFiles().Count());

            foreach (var projectPath in testProject.ProjectFiles())
            {
                var time = MeasureProjectEvaluation(pathToMSBuild, projectPath);

                csvLine.Add(time.ToString(CultureInfo.InvariantCulture));
            }

            return csvLine;
        }

        private static double MeasureProjectEvaluation(string pathToMSBuild, string projectPath)
        {
            var times = new List<double>(RepetitionsPerProject);

            Console.WriteLine($"Measuring project: {projectPath}\n With {pathToMSBuild}");

            for (var i = 0; i < RepetitionsPerProject; i++)
            {
                var domain = AppDomain.CreateDomain(pathToMSBuild);
                domain.AssemblyResolve += new MSBuildAssemblyResolver(pathToMSBuild).ResolveAssembly;

                var watch = Stopwatch.StartNew();
                var project = domain.CreateInstanceFrom(
                    Path.Combine(pathToMSBuild, "Microsoft.Build.dll"),
                    "Microsoft.Build.Evaluation.Project",
                    false,
                    BindingFlags.CreateInstance,
                    null,
                    new object[] {projectPath},
                    CultureInfo.CurrentCulture,
                    null);

                watch.Stop();

                times.Add(watch.Elapsed.TotalSeconds);

                AppDomain.Unload(domain);
            }

            return times.Average();
        }

        [Serializable]
        private class MSBuildAssemblyResolver
        {
            private readonly string _pathToMsBuild;

            public MSBuildAssemblyResolver(string pathToMSBuild)
            {
                _pathToMsBuild = pathToMSBuild;
            }

            public Assembly ResolveAssembly(object sender, ResolveEventArgs args)
            {
                var assemblyName = new string(args.Name.TakeWhile(c => c != ',').ToArray());
                var requestedAssembly = Path.Combine(_pathToMsBuild, assemblyName + ".dll");

                if (File.Exists(requestedAssembly))
                {
                    return Assembly.LoadFrom(requestedAssembly);
                }
                return null;
            }
        }
    }
}