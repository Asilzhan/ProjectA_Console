using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ProjectA_ConsoleCore.Models;

namespace ProjectA_ConsoleCore.Helper
{
    public class Compiler
    {
        // Есептің вердиктін өзгергенін өңдейтін методты көрсететін делегат
        public delegate void ConsoleRewriteDelegate(ConsoleRewriteEventArgs args);
        // Есептің вердиктін өзгергенін көрсететін оқиға
        public event ConsoleRewriteDelegate StatusChanged;

        // Берілген есептің кодын компиляңиялап, тесттерден өткізеді
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
                attempt.Verdict = Verdict.Complation_error;
                // Егер вердикт Complation_error болса, он сәйкес параметрлермен OnStatusChanged оқиғасы шақырылады
                OnStatusChanged(new ConsoleRewriteEventArgs("Complation_error".PadLeft(20), 3,49, Console.CursorLeft, Console.CursorTop, ConsoleColor.Red)); 
            }
            else
            {
                RunSolution(problem, assemblyPath, ref attempt);
            }
        }
        
        /// <summary>
        /// Берілген есептің бағдарламасын тесткейстерде тексереді. Тексеру нәтижесін attempt айнымалысына жазады
        /// </summary>
        /// <param name="problem">Есептің берілгені және тесттері</param>
        /// <param name="assembly">Есептің бағдарламасының аты</param>
        /// <param name="attempt">Осы айнымалыға есептің тесттерден көткендегі вердикттері жазылады</param>
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
            // Вердикт өзгертілгендіктен, OnStatusChanged оқиғасы шақырылады
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