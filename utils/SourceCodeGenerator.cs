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

    /*
    Needs reworking
    */
    // public Task GenerateCsharpCode(string path, string projectName)
    // {
    //     var directory = Directory.CreateDirectory(Path.Combine(path, projectName));
    //     Process csProc = new Process()
    //     {
    //         StartInfo = new ProcessStartInfo()
    //         {
    //             FileName = "cmd",
    //             RedirectStandardInput = true,
    //             UseShellExecute = false,
    //             WorkingDirectory = directory.FullName,
    //             CreateNoWindow = true,
    //         },
    //     };
    //     csProc.Start();
    //     csProc.StandardInput.WriteLine("dotnet new console");
    //     csProc.Close();

    //     CSharpCodeProvider provider = new CSharpCodeProvider();

    //     // Build the output file name.
    //     string sourceFile;
    //     if (provider.FileExtension[0] == '.')
    //     {
    //         sourceFile = projectName + provider.FileExtension;
    //     }
    //     else
    //     {
    //         sourceFile = $"{projectName}." + provider.FileExtension;
    //     }

    //     // Create a TextWriter to a StreamWriter to the output file.
    //     using (StreamWriter sw = new StreamWriter(Path.Combine(directory.FullName, sourceFile), false))
    //     {
    //         IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

    //         // Generate source code using the code provider.
    //         provider.GenerateCodeFromCompileUnit(compileUnit, tw,
    //             new CodeGeneratorOptions()
    //             {
    //                 IndentString = "   ",
    //             });

    //         // Close the output file.
    //         tw.Close();
    //     }

    //     return Task.CompletedTask;
    // }

    /// <summary>Generates a basic node js project to the selected directory and installs the required packages</summary>
    public async Task GenerateJavascriptCode(string path, string projectName)
    {
        var directory = Directory.CreateDirectory(Path.Combine(path, projectName));
        Process jsProc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                UseShellExecute = false,
                WorkingDirectory = directory.FullName,
                CreateNoWindow = true,
            },
        };
        jsProc.Start();
        jsProc.StandardInput.WriteLine("npm init -y");
        jsProc.StandardInput.WriteLine("npm install discord.js");
        jsProc.Close();

        var commandsDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "commands"));
        var utilsDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "utils"));
        var handlersDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "handlers"));
        var configDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "config"));
        File.WriteAllText(Path.Combine(directory.FullName, ".gitignore"), "node_modules");
        File.WriteAllText(Path.Combine(directory.FullName, "index.js"), "");
        using StreamWriter indexFile = new(Path.Combine(directory.FullName, "index.js"), append: true);
        await indexFile.WriteLineAsync(@"
const Discord = require('discord.js');
const { Client, MessageEmbed } = require('discord.js');
const { prefix, token } = require('./config/config.json'); 
const client = new Client();
const fs = require('fs');
            
client.commands = new Discord.Collection();
client.aliases = new Discord.Collection();

[""command""].forEach(handler => {
    require(`./ handlers /${ handler}`)(client);
});

fs.readdir('./events/', (err, files) => {
    if (err) return console.error(err);
    files.forEach(file => {
    const event = require(`./events/${file
}`);
let eventName = file.split(""."")[0];
console.log(`Loading event ${ eventName}`);
    client.on(eventName, event.bind(null, client));
    });
});

fs.readdir('./commands/', (err, files) =>
{
    if (err) return console.error(err);
    files.forEach(file =>
    {
        if (!file.endsWith("".js"")) return;
        let props = require(`./ commands /${ file}`);
        let commandName = file.split(""."")[0];
        console.log(`Loading command ${ commandName}`);
        client.commands.set(commandName, props);
    });
});

client.login(token);
");
        //return Task.CompletedTask;
    }


    public Task GenerateTypescriptCode(string path, string projectName)
    {
        var directory = Directory.CreateDirectory(Path.Combine(path, projectName));
        Process tsProc = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd",
                RedirectStandardInput = true,
                UseShellExecute = false,
                WorkingDirectory = directory.FullName,
                CreateNoWindow = true,
            },
        };
        tsProc.Start();
        tsProc.StandardInput.WriteLine("npm init -y");
        tsProc.Close();

        var commandsDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "commands"));
        var utilsDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "utils"));
        var handlersDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "handlers"));
        var configDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "config"));
        File.WriteAllText(Path.Combine(directory.FullName, ".gitignore"), "node_modules");
        File.WriteAllText(Path.Combine(directory.FullName, "index.ts"), "");

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
