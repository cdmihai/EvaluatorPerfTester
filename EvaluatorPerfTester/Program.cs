using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EvaluatorPerfTester
{
    class Program
    {
        private static int _repetitionsPerProject = 20;

        static void Main(string[] args)
        {
            var csvHeader = new List<string> {"branch"};
            var masterCsvLine = new List<string> {"master"};
            var lazyCsvLine = new List<string> {"Lazy"};

            var projectProvider = new MSBuildTestProjectProvider();

            csvHeader.AddRange(projectProvider.ItemCounts.Value.Select(i => i.ToString()));

            try
            {
                projectProvider.ConstructMSBuildProjects();

                masterCsvLine.AddRange(TestEvaluation(@"C:\projects\msbuild_2\bin\x86\Windows_NT\Debug\", projectProvider));
                lazyCsvLine.AddRange(TestEvaluation(@"C:\projects\msbuild\bin\x86\Windows_NT\Debug\", projectProvider));
            }
            finally
            {
                projectProvider.DeleteMSBuildProjects();
            }

            PrintCSV(csvHeader, masterCsvLine, lazyCsvLine);

            Console.WriteLine(masterCsvLine.Aggregate((s1, s2) => s1 + "\t;\t" + s2));
            Console.WriteLine(lazyCsvLine.Aggregate((s1, s2) => s1 + "\t;\t" + s2));

            Console.ReadKey();
        }

        private static void PrintCSV(params List<string>[] lines)
        {
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(line.Aggregate((e1, e2) => e1 + "," + e2));
            }

            var csvPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "results.csv");
            File.WriteAllText(csvPath, sb.ToString());
        }

        private static List<string> TestEvaluation(string pathToMSBuild, MSBuildTestProjectProvider projectProvider)
        {
            var csvLine = new List<string>(projectProvider.GeneratedFiles.Count);

            foreach (var projectPath in projectProvider.GeneratedFiles)
            {
                var time = MeasureProjectEvaluation(pathToMSBuild, projectPath);

                csvLine.Add(time.ToString(CultureInfo.InvariantCulture));
            }

            return csvLine;
        }

        private static double MeasureProjectEvaluation(string pathToMSBuild, string projectPath)
        {
            var times = new List<int>(_repetitionsPerProject);

            Console.WriteLine($"Measuring project: {projectPath}");

            for (var i = 0; i < _repetitionsPerProject; i++)
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

                times.Add(watch.Elapsed.Milliseconds);

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
                else
                {
                    return null;
                }
            }
        }
    }
}