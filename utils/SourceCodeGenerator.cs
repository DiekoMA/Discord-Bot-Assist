public sealed class SourceCodeGenerator
{
    CodeCompileUnit compileUnit;
    private static readonly SourceCodeGenerator _instance = new SourceCodeGenerator();

    static SourceCodeGenerator() { }

    private SourceCodeGenerator()
    {
        /// <summary>Exclusively for C#</summary>
        CodeCompileUnit compileUnit = new CodeCompileUnit();
        CodeNamespace samples = new CodeNamespace("Testing");
        samples.Imports.Add(new CodeNamespaceImport("System"));
        compileUnit.Namespaces.Add(samples);
        CodeTypeDeclaration class1 = new CodeTypeDeclaration("Class1");
        samples.Types.Add(class1);
    }

    public Task GenerateCsharpCode(string path, string projectName)
    {
        CSharpCodeProvider provider = new CSharpCodeProvider();

        // Build the output file name.
        string sourceFile;
        if (provider.FileExtension[0] == '.')
        {
            sourceFile = projectName + provider.FileExtension;
        }
        else
        {
            sourceFile = $"{projectName}." + provider.FileExtension;
        }

        // Create a TextWriter to a StreamWriter to the output file.
        using (StreamWriter sw = new StreamWriter(sourceFile, false))
        {
            IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

            // Generate source code using the code provider.
            provider.GenerateCodeFromCompileUnit(compileUnit, tw,
                new CodeGeneratorOptions()
                {
                    IndentString = "   ",
                });

            // Close the output file.
            tw.Close();
        }

        return Task.CompletedTask;
    }

    /// <summary>Generates a basic node js project to the selected directory</summary>
    public Task GenerateJavascriptCode(string path, string projectName)
    {
        var directory = Directory.CreateDirectory(Path.Combine(path, projectName));
        Process jsProc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "npm init",
                UseShellExecute = false
            }
        };
        return Task.CompletedTask;
    }

    public Task GenerateTypescriptCode(string path, string projectName)
    {
        var directory = Directory.CreateDirectory(Path.Combine(path, projectName));
        Process tsProc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "npm init",
                UseShellExecute = false
            }
        };
        tsProc.Close();
        Process tsProc2 = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "npm install --save-dev typescript",
                UseShellExecute = false
            }
        };
        tsProc2.Close();
        Process tsProc3 = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                ArgumentList = { $"cd {directory}", "nano tsconfig.json" },
                UseShellExecute = false
            }
        };
        return Task.CompletedTask;
    }

    public Task GeneratePythonCode(string path, string projectName)
    {
        var directory = Directory.CreateDirectory(Path.Combine(path, projectName));
        File.WriteAllText(Path.Combine(directory.FullName, "test.py"), "blah blah");

        return Task.CompletedTask;
    }

    public static SourceCodeGenerator Instance { get { return _instance; } }

}