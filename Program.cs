namespace DiscordBotTemplateCreator
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            //await StartScreen();
            //await SourceCodeGenerator.Instance.GeneratePythonCode(@"C:\Users\dieko\Documents\Projects\Coding Projects\Python Projects", "Testproject");

            //await ChooseCommands();
            await ChooseLanguage();
        }
        static async Task StartScreen()
        {
            AnsiConsole.Write(
                new FigletText("Discord Bot Template Creator")
                    .Color(Color.Red));

            if (!AnsiConsole.Confirm("Use Default output ?"))
            {
                AnsiConsole.MarkupLine("Please input a directory location");
                var path = AnsiConsole.Ask<string>("Path: ");
                var validation = IsValidPath(path);
                if (validation)
                {
                    await CreateDirectory(path);
                }
                else
                {
                    AnsiConsole.Write("Invalid Path");
                    await StartScreen();
                }
            }
            else
            {
                //await Flow();
                //CreateDefaultDirectory();
            }
        }


        static async Task Flow()
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Triangle)
                .SpinnerStyle(Style.Parse("green bold"))
                .StartAsync("Creating Directory...", async ctx =>
                {
                    await CreateDefaultDirectory();
                });
            await ChooseLanguage();
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Triangle)
                .SpinnerStyle(Style.Parse("green bold"))
                .StartAsync("Generating Source code...", ctx =>
                {
                    CodeCompileUnit compileUnit = new CodeCompileUnit();
                    CodeNamespace samples = new CodeNamespace("Testing");
                    samples.Imports.Add(new CodeNamespaceImport("System"));
                    compileUnit.Namespaces.Add(samples);
                    CodeTypeDeclaration class1 = new CodeTypeDeclaration("Class1");
                    samples.Types.Add(class1);
                    GenerateCSharpCode(compileUnit);
                    return Task.CompletedTask;
                });

        }

        static Task ChooseLanguage()
        {
            var languages = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What is your prefered language")
                    .PageSize(7)
                    .AddChoices(new String[] {
                        "C#", "Javascript", "Typescript",  "Lua", "Python"
                    })
            );

            switch (languages)
            {
                case "C#":
                    AnsiConsole.WriteLine("Selected C#");
                    /// <summary>Placeholder C# generation Code</summary>
                    break;

                case "Javascript":
                    /// <summary>Placeholder Javascript generation Code</summary>
                    break;

                case "Typescript":
                    /// <summary>Placeholder Typescript generation Code</summary>
                    break;

                case "Lua":
                    /// <summary>Placeholder Lua generation Code</summary>
                    break;

                case "Python":
                    /// <summary>Placeholder Python generation Code</summary>
                    break;

                default:

                    break;
            }

            return Task.CompletedTask;
        }

        static Task ChooseCommands()
        {
            var commands = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("What default commands would you like?")
                    .MoreChoicesText("Leave blank for no commands")
                    .NotRequired()
                    .AddChoiceGroup<string>("Moderation", new[] {
                        "Ban", "Kick", "Timeout", "All"
                    })
                    .AddChoiceGroup<string>("Music", new[] {
                        "Play", "Pause", "Join", "Leave", "Volume"
                    })
            );

            return Task.CompletedTask;
        }

        static Task CreateDefaultDirectory()
        {
            var p1 = Directory.GetCurrentDirectory();
            var p2 = "Templates";
            var defaultPath = Path.Combine(p1, p2);
            Console.WriteLine(defaultPath);
            Directory.CreateDirectory(defaultPath);
            //File.WriteAllText(defaultPath, "");
            return Task.CompletedTask;
        }

        static Task CreateDirectory(string userdefinedPath)
        {
            Directory.CreateDirectory(userdefinedPath);
            return Task.CompletedTask;
        }


        /// <summary>Generate the project source code</summary>
        public static string GenerateCSharpCode(CodeCompileUnit compileunit)
        {
            // Generate the code with the C# code provider.
            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Build the output file name.
            string sourceFile;
            if (provider.FileExtension[0] == '.')
            {
                sourceFile = "Bot" + provider.FileExtension;
            }
            else
            {
                sourceFile = "Bot." + provider.FileExtension;
            }

            // Create a TextWriter to a StreamWriter to the output file.
            using (StreamWriter sw = new StreamWriter(sourceFile, false))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

                // Generate source code using the code provider.
                provider.GenerateCodeFromCompileUnit(compileunit, tw,
                    new CodeGeneratorOptions()
                    {
                        IndentString = "   ",
                    });

                // Close the output file.
                tw.Close();
            }

            return sourceFile;
        }

        static bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}