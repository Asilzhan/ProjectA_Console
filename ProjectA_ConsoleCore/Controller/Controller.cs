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
using ProjectA_ConsoleCore.Views;
using static System.Console;

namespace ProjectA_ConsoleCore.Controller
{
    public class Controller
    {
        View view = new View();
        Model model = new Model();
        public Student CurrentStudent;

        public void Main()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.MainMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        break;
                    case 2:
                        Clear();
                        break;
                    case 0:
                        return;
                }
            }
        }
        
        public void StudentCommand()
        {
            int cmd;
            while (true)
            {
                Clear();
                view.StudentMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        ShowProblems();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void ShowProblems()
        {
            Clear();
            view.Print(model.Problems);
            Problem p = model.Problems[view.ReadInt(">> ", maxValue: model.Problems.Count)];

            ProblemMenu(p);
        }

        private void ProblemMenu(Problem problem)
        {
            int cmd;
            while (true)
            {
                Clear();
                view.Print(problem);
                view.ProblemMenu();
                cmd = view.ReadInt();
                
                switch (cmd)
                {
                    case 1:
                        Submit(problem);
                        break;
                    case 2:
                        view.Print(model.GetAttemptsOfStudent(problem, CurrentStudent));
                        break;
                    case 0:
                        return;
                    default:
                        view.ShowError();
                        ReadKey();
                        break;
                }
                
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
                    writer.WriteString(
                        "version",
                        RuntimeInformation.FrameworkDescription.Replace(".NET Core ", "")
                    );
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        public void Submit(Problem problem)
        {
            string sourcePath = view.ReadString("Программаның файлы орналасқан адресті жазыңыз немесе тышқанмен сүйреп әкеліңіз\n", ConsoleColor.Green);
            sourcePath = sourcePath.Trim(new[] {'\'', '\"'});
            if (!File.Exists(sourcePath))
            {
                view.ShowError("Файл табылмады!");
                return;
            }

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
            
            Attempt attempt = model.AddAttemption(CurrentStudent, problem);
            
            if (!result.Success)
            {
                view.Print("Компиляция барысында қате шықты!\n", ConsoleColor.Red);
                attempt.Verdict = Verdict.Complation_error;
            }
            else
            {
                RunSolution(problem, assemblyPath, ref attempt);
            }
            model.Attempts.Add(attempt);
        }

        private void RunSolution(Problem problem, string assembly, ref Attempt attempt)
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
                // process.Kill();
            }
            attempt.Verdict = verdict;
            if(verdict == Verdict.Accepted)
                view.Print("Дұрыс жауап!\n", ConsoleColor.Green);
            else 
                view.Print("Қате жауап!\n", ConsoleColor.Green);
            ReadKey();
        }
    }
}