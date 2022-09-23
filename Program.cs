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
            await StartScreen();
            //await ChooseCommands();
            //await ChooseLanguage();

        }
        static async Task StartScreen()
        {
            AnsiConsole.Write(
                new FigletText("Discord Bot Template Creator")
                    .Color(Color.Red));

            await ChooseLanguage();
        }

        static async Task ChooseLanguage()
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
                    break;

                case "Javascript":
                    if (!AnsiConsole.Confirm("Use Default output ?"))
                    {
                        var projectName = AnsiConsole.Ask<string>("What would you like to call your project: ");
                        var projectPath = AnsiConsole.Ask<string>("Path: ");
                        var validation = IsValidPath(projectPath);
                        if (validation)
                        {
                            await AnsiConsole.Status()
                                .Start("Creating Directory", async ctx =>
                                {
                                    await CreateDirectory(projectPath);
                                    Thread.Sleep(1000);
                                    AnsiConsole.MarkupLine("[green]Directory Created[/]");

                                    await SourceCodeGenerator.Instance.GenerateJavascriptCode(projectPath, projectName);
                                    ctx.Status("Generating JS Code");
                                    ctx.Spinner(Spinner.Known.Star);
                                    ctx.SpinnerStyle(Style.Parse("green"));
                                    Thread.Sleep(3000);
                                    AnsiConsole.MarkupLine("[green]JS Code Generated[/]");
                                });
                            var options = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Options")
                                    .PageSize(3)
                                    .AddChoices(new string[] {
                                        "Open in vscode", "Exit"
                                    })
                            );

                            switch (options)
                            {
                                case "Open in vscode":
                                    AnsiConsole.Status()
                                    .Start("Opening VS Code", ctx =>
                                    {
                                        Process vsProc = new Process()
                                        {
                                            StartInfo = new ProcessStartInfo()
                                            {
                                                FileName = "cmd",
                                                RedirectStandardInput = true,
                                                UseShellExecute = false,
                                                WorkingDirectory = Path.Combine(projectPath, projectName),
                                                CreateNoWindow = true,
                                            },
                                        };
                                        vsProc.Start();
                                        vsProc.StandardInput.WriteLine("code .");
                                        AnsiConsole.MarkupLine("[green]✅[/]");
                                        Thread.Sleep(2000);
                                    });
                                    break;

                                case "Exit":
                                    AnsiConsole.Status()
                                    .Start("Exiting", ctx =>
                                    {
                                        Thread.Sleep(2000);
                                        Environment.Exit(0);
                                        AnsiConsole.MarkupLine("[green]Bye[/]");
                                    });
                                    break;
                            }
                        }
                        else
                        {
                            AnsiConsole.Write("Invalid Path");
                            await StartScreen();
                        }
                    }
                    else
                    {
                        var defaultPath = CreateDefaultDirectory();
                        var projectName = AnsiConsole.Ask<string>("What would you like to call your project: ");
                        await AnsiConsole.Status()
                        .Start("Creating Directory", async ctx =>
                        {
                            Thread.Sleep(1000);
                            AnsiConsole.MarkupLine("[green]Directory Created[/]");

                            await SourceCodeGenerator.Instance.GenerateJavascriptCode(defaultPath, projectName);
                            ctx.Status("Generating JS Code");
                            ctx.Spinner(Spinner.Known.Star);
                            ctx.SpinnerStyle(Style.Parse("green"));
                            Thread.Sleep(3000);
                            AnsiConsole.MarkupLine("[green]JS Code Generated[/]");
                        });

                        var options = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Options")
                                    .PageSize(3)
                                    .AddChoices(new string[] {
                                        "Open in vscode", "Exit"
                                    })
                            );

                        switch (options)
                        {
                            case "Open in vscode":
                                AnsiConsole.Status()
                                .Start("Opening VS Code", ctx =>
                                {
                                    Process vsProc = new Process()
                                    {
                                        StartInfo = new ProcessStartInfo()
                                        {
                                            FileName = "cmd",
                                            RedirectStandardInput = true,
                                            UseShellExecute = false,
                                            WorkingDirectory = Path.Combine(defaultPath, projectName),
                                            CreateNoWindow = true,
                                        },
                                    };
                                    vsProc.Start();
                                    vsProc.StandardInput.WriteLine("code .");
                                    Thread.Sleep(2000);
                                });
                                break;

                            case "Exit":
                                AnsiConsole.Status()
                                .Start("Exiting", ctx =>
                                {
                                    Thread.Sleep(2000);
                                    Environment.Exit(0);
                                });
                                break;
                        }
                    }
                    break;

                case "Lua":

                    break;

                case "Python":
                    /// <summary>Placeholder Python generation Code</summary>
                    break;

                default:

                    break;
            }
        }

        // static Task ChooseCommands()
        // {
        //     var commands = AnsiConsole.Prompt(
        //         new MultiSelectionPrompt<string>()
        //             .Title("What default commands would you like?")
        //             .MoreChoicesText("Leave blank for no commands")
        //             .NotRequired()
        //             .AddChoiceGroup<string>("Moderation", new[] {
        //                 "Ban", "Kick", "Timeout", "All"
        //             })
        //             .AddChoiceGroup<string>("Music", new[] {
        //                 "Play", "Pause", "Join", "Leave", "Volume"
        //             })
        //     );

        //     return Task.CompletedTask;
        // }

        static string CreateDefaultDirectory()
        {
            var p1 = Directory.GetCurrentDirectory();
            var p2 = "Templates";
            var defaultPath = Path.Combine(p1, p2);
            Console.WriteLine(defaultPath);
            Directory.CreateDirectory(defaultPath);
            return defaultPath;
        }

        static Task CreateDirectory(string userdefinedPath)
        {
            Directory.CreateDirectory(userdefinedPath);
            return Task.CompletedTask;
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
