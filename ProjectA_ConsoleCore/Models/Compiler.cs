using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ProjectA_ConsoleCore.Models
{
    public class Compiler
    {
        public delegate void ConsoleRewriteDelegate(ConsoleRewriteEventArgs args);
        public event ConsoleRewriteDelegate StatusChanged;
        
        public void RunSolution(Problem problem, string assembly, ref Attempt attempt)
        {
            Verdict verdict = Verdict.Wrong_answer;
            foreach (var testCase in problem.TestCases)
            {
                Process process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = assembly;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.ErrorDialog = false;
                process.Start();
            
                StreamWriter stdInputWriter  = process.StandardInput;
                StreamReader stdOutputReader  = process.StandardOutput;
                
                stdInputWriter.WriteLine(testCase.Input.Replace(' ', '\n'));
                string res = stdOutputReader.ReadToEnd();
                
                if (string.Compare(res.Trim(), testCase.Output, StringComparison.InvariantCultureIgnoreCase)==0)
                {
                    verdict = Verdict.Accepted;
                }
                else
                {
                    verdict = Verdict.Wrong_answer;
                    break;
                }

                var result = new AttemptionResult(testCase.Input, testCase.Output, res);
                attempt.TestCases.Add(result);
            }
            
            if (verdict == Verdict.Accepted)
            {
                OnStatusChanged(new ConsoleRewriteEventArgs("Accepted".PadLeft(20), 3,49,Console.CursorLeft, Console.CursorTop, ConsoleColor.Green));
            }
            else
            {
                OnStatusChanged(new ConsoleRewriteEventArgs("Wrong_answer".PadLeft(20), 3,49, Console.CursorLeft, Console.CursorTop,ConsoleColor.Red));
            }

            attempt.Verdict = verdict;
        }

        public void Sumbit(Attempt attempt, Problem problem, string sourcePath)
        {
            
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(File.ReadAllText(sourcePath)));
            var assemblyPath = "test.exe";
            var dotNetCoreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var compilation = CSharpCompilation.Create(Path.GetFileName(assemblyPath))
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Path.Combine(dotNetCoreDir, "System.Runtime.dll"))
                )
                .AddSyntaxTrees(syntaxTree);
            var result = compilation.Emit(assemblyPath);
            File.WriteAllText(
                Path.ChangeExtension(assemblyPath, "runtimeconfig.json"),
                GenerateRuntimeConfig()
            );

            
            if (!result.Success)
            {
                // view.Print("Компиляция барысында қате шықты!\n", ConsoleColor.Red);
                attempt.Verdict = Verdict.Complation_error;
                OnStatusChanged(new ConsoleRewriteEventArgs("Complation_error".PadLeft(20), 3,49, Console.CursorLeft, Console.CursorTop, ConsoleColor.Red));    // TODO: Setup x and y
            }
            else
            {
                RunSolution(problem, assemblyPath, ref attempt);
            }
        }
        private string GenerateRuntimeConfig()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(
                    stream,
                    new JsonWriterOptions() { Indented = true }
                ))
                {
                    writer.WriteStartObject();
                    writer.WriteStartObject("runtimeOptions");
                    writer.WriteStartObject("framework");
                    writer.WriteString("name", "Microsoft.NETCore.App");
                    string ver = RuntimeInformation.FrameworkDescription.Replace(".NET Core ", "").Replace(".NET ", ""); 
                    writer.WriteString(
                        "version",
                        ver
                    );
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        protected virtual void OnStatusChanged(ConsoleRewriteEventArgs args)
        {
            StatusChanged?.Invoke(args);
        }
    }
}