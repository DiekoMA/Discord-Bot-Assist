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