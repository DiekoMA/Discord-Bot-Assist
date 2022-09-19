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
    public Task GenerateCsharpCode(string path, string projectName)
    {
        var directory = Directory.CreateDirectory(Path.Combine(path, projectName));
        Process csProc = new Process()
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
        csProc.Start();
        csProc.StandardInput.WriteLine("dotnet new console");
        csProc.StandardInput.WriteLine($"dotnet add package Discord.Net");
        csProc.StandardInput.WriteLine($"dotnet add package Discord.Net.Commands");
        csProc.StandardInput.WriteLine($"dotnet add package Discord.Net.Interactions");
        csProc.StandardInput.WriteLine($"dotnet add package Discord.Net.Webhook");
        csProc.Close();

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
        using (StreamWriter sw = new StreamWriter(Path.Combine(directory.FullName, sourceFile), false))
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
        jsProc.StandardInput.WriteLine("npm install --save-dev eslint");
        jsProc.StandardInput.WriteLine("npm install dotenv");
        jsProc.Close();

        var commandsDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "commands"));
        var eventsDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "events"));
        File.WriteAllText(Path.Combine(directory.FullName, ".gitignore"),
@"node_modules
.env
config.json        
        ");
        File.WriteAllText(Path.Combine(directory.FullName, ".env"),
@"CLIENT_ID=""""
GUILD_ID="""" 
DISCORD_TOKEN=""""//Put your bot token here and add this file to your .gitignore, it should be there but check just incase.
        ");
        File.WriteAllText(Path.Combine(directory.FullName, "index.js"),
@"const fs = require('node:fs');
const path = require('node:path');
const { Client, Collection, GatewayIntentBits } = require('discord.js');
const { token } = require('dotenv').config();

const client = new Client({ intents: [GatewayIntentBits.Guilds] });

client.commands = new Collection();

const commandsPath = path.join(__dirname, 'commands');
const eventsPath = path.join(__dirname, 'events');

const commandFiles = fs.readdirSync(commandsPath).filter(file => file.endsWith('.js'));
const eventFiles = fs.readdirSync(eventsPath).filter(file => file.endsWith('.js'));

for (const file of commandFiles) {
    const filePath = path.join(commandsPath, file);
    const command = require(filePath);
    // Set a new item in the Collection
    // With the key as the command name and the value as the exported module
    client.commands.set(command.data.name, command);
}

for (const file of eventFiles) {
    const filePath = path.join(eventsPath, file);
    const event = require(filePath);
    if (event.once) {
        client.once(event.name, (...args) => event.execute(...args));
    } else {
        client.on(event.name, (...args) => event.execute(...args));
    }
}

client.once('ready', c => {
    console.log(`${c.user.tag} is Ready!`);
});

client.on('interactionCreate', async interaction => {

});

client.login(process.env.DISCORD_TOKEN);


//run node deploy-commands.js first to register your new commands.
//run node . to start the bot.");

        File.WriteAllText(Path.Combine(directory.FullName, "deploy-commands.js"),
@"const fs = require('node:fs');
const path = require('node:path');
const { REST } = require('@discordjs/rest');
const { Routes } = require('discord.js');
const { clientId, guildId, token } = require('dotenv').config();

const rest = new REST({ version: '10' }).setToken(token);

const commands = [];
const commandsPath = path.join(__dirname, 'commands');
const commandFiles = fs.readdirSync(commandsPath).filter(file => file.endsWith('.js'));

for (const file of commandFiles) {
    const filePath = path.join(commandsPath, file);
    const command = require(filePath);
    commands.push(command.data.toJSON());
}
        ");

        File.WriteAllText(Path.Combine(commandsDir.FullName, "ping.js"),
@"const { SlashCommandBuilder } = require('discord.js');

module.exports = {
    data: new SlashCommandBuilder()
        .setName('ping')
        .setDescription('Replies with Pong!'),
    async execute(interaction) {
        await interaction.reply('Pong!');
    }
}
        ");

        File.WriteAllText(Path.Combine(eventsDir.FullName, "ready.js"),
@"module.exports = {
    name: 'ready',
    once: true,
    execute(client) {
        console.log(`Ready! Logged in as ${client.user.tag}`);
    },
};
        ");

        File.WriteAllText(Path.Combine(eventsDir.FullName, "interactionCreate.js"),
@"module.exports = {
    name: 'interactionCreate',
    async execute(interaction) {
        if (!interaction.isChatInputCommand()) return;

        const command = interaction.client.commands.get(interaction.commandName);

        if (!command) return;

        try {
            await command.execute(interaction);
        } catch (error) {
            console.error(error);
            await interaction.reply({ content: 'There was an error while executing this command!', ephemeral: true });
        }
        console.log(`${interaction.user.tag} in #${interaction.channel.name} triggered an interaction.`);
    },
};
        ");
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
        tsProc.StandardInput.WriteLine("npm install discord.js");
        tsProc.Close();

        var sourceDir = Directory.CreateDirectory(Path.Combine(directory.FullName, "src"));
        var commandsDir = Directory.CreateDirectory(Path.Combine(sourceDir.FullName, "commands"));
        var listenersDir = Directory.CreateDirectory(Path.Combine(sourceDir.FullName, "listeners"));
        File.WriteAllText(Path.Combine(directory.FullName, ".gitignore"), "node_modules");
        File.WriteAllText(Path.Combine(directory.FullName, "tsconfig.json"),
@"{
""compilerOptions"": {
    ""target"": ""ESNext"",
    ""module"": ""commonjs"",
    ""rootDir"": ""./src/"",
    ""outDir"": ""./dist/"",
    ""strict"": true,
    ""moduleResolution"": ""node"",
    ""importHelpers"": true,
    ""experimentalDecorators"": true,
    ""esModuleInterop"": true,
    ""skipLibCheck"": true,
    ""allowSyntheticDefaultImports"": true,
    ""resolveJsonModule"": true,
    ""forceConsistentCasingInFileNames"": true,
    ""removeComments"": true,
    ""typeRoots"": [
        ""node_modules/@types""
    ],
        ""sourceMap"": false,
        ""baseUrl"": ""./""
    },
        ""files"": [
            ""src/Bot.ts""
        ],
        ""include"": [
            ""./**/*.ts""
        ],
        ""exclude"": [
            ""node_modules"",
                ""dist""
        ],
    }
");
        File.WriteAllText(Path.Combine(sourceDir.FullName, "Bot.ts"),

@"import { Client, ClientOptions } from ""discord.js"";
import interactionCreate from ""./listeners/interactionCreate"";
import ready from ""./listeners/ready"";

const token = """"; // add your token here

console.log(""Bot is starting..."");

const client = new Client({
    intents: []
});

ready(client);
interactionCreate(client);

client.login(token);

        ");
        File.WriteAllText(Path.Combine(listenersDir.FullName, "ready.ts"), @"
import { Client } from ""discord.js"";
import { Commands } from "".. / Commands"";

export default (client: Client): void => {
    client.on(""ready"", async () => {
        if (!client.user || !client.application) {
            return;
        }

        await client.application.commands.set(Commands);

        console.log(`${client.user.username is online`});
    });
};
        ");

        File.WriteAllText(Path.Combine(listenersDir.FullName, "interactionCreate.ts"),
@"import { BaseCommandInteraction, Client, Interaction } from ""discord.js"";

export default(client: Client): void => {
    client.on(""interactionCreate"", async(interaction: Interaction) => {
        if (interaction.isCommand() || interaction.isContextMenu())
        {
            await handleSlashCommand(client, interaction);
        }
    });
};

const handleSlashCommand = async(client: Client, interaction: CommandInteraction): Promise<void> => {
    // handle slash command here
};
        ");

        File.WriteAllText(Path.Combine(sourceDir.FullName, "Command.ts"), @"
import { CommandInteraction, ChatInputApplicationCommandData, Client } from ""discord.js"";

export interface Command extends ChatInputApplicationCommandData{
    run: (client: Client, interaction: CommandInteraction) => void;
}
        ");

        File.WriteAllText(Path.Combine(commandsDir.FullName, "Hello.ts"), @"
import { BaseCommandInteraction, Client } from ""discord.js"";
import { Command } from ""../Command"";

export const Hello: Command = {
    name: ""hello"",
    description: ""Returns a greeting"",
    type: ""CHAT_INPUT"",
    run: async(client: Client, interaction: BaseCommandInteraction) => {
        const content = ""Hello there!"";

        await interaction.followUp({
            ephemeral: true,
            content
        });
    }
};
        ");

        File.WriteAllText(Path.Combine(sourceDir.FullName, "Commands.ts"),
@"import { Command } from ""./Command"";
import { Hello } from ""./commands/Hello"";

//register the rest of your commands here

export const Commands: Command[] = [Hello];
        ");
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
