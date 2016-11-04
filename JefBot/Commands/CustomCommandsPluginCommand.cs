using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    class CustomCommandsPluginCommand : IPluginCommand
    {
        public string PluginName => "Custom Commands";
        public string Command => "command";
        public string Help => "!cmd add {command name} {command result}";
        public string[] OtherAliases = {"commands", "cmd", "cmds"};
        public IEnumerable<string> Aliases => CustomCommands.Select(command => command.Command).ToArray().Concat(OtherAliases);
        public List<CCommand> CustomCommands = new List<CCommand>();
        public bool Loaded { get; set; } = true;
        private string memoryPath = "./customCommands.txt";

        public CustomCommandsPluginCommand()
        {
            Load();
        }

        public void Execute(ChatCommand command, TwitchClient client)
        {
            try
            {



                // Main command methods
                if (
                    string.Equals(command.Command, "command", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command.Command, "commands", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command.Command, "cmd", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command.Command, "cmds", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    if (command.ArgumentsAsList.Count > 0)
                    {
                        var args = command.ArgumentsAsList;
                        if (command.ChatMessage.IsModerator || command.ChatMessage.IsBroadcaster)
                        {
                            if (string.Equals(args[0], "add", StringComparison.OrdinalIgnoreCase))
                            {
                                if (args.Count >= 3)
                                {
                                    var newCommand = args[1];
                                    var response = string.Join(" ", args.Skip(2));

                                    // Looks like someone is trying to run a command!
                                    if (response.StartsWith("/") && !response.StartsWith("/me"))
                                    {
                                        client.SendMessage(command.ChatMessage.Channel, $"I'm sorry, Dave. I'm afraid I can't do that.");
                                        client.SendWhisper(command.ChatMessage.Username, $"Custom commands cannot run chat slash commands...");
                                        return;
                                    }

                                    CCommand cmd = new CCommand(newCommand, response, command.ChatMessage.Channel);
                                    
                                    CustomCommands.RemoveAll(cmdx => cmdx.Channel == command.ChatMessage.Channel && cmdx.Command == newCommand);

                                    CustomCommands.Add(cmd);
                                    client.SendMessage(command.ChatMessage.Channel, $"Command {newCommand} has been added");
                                    Save();
                                }
                                else
                                {
                                    client.SendMessage(command.ChatMessage.Channel, "Usage !command add (command) (message)");
                                }
                            }

                            else if (string.Equals(args[0], "remove", StringComparison.OrdinalIgnoreCase))
                            {
                                if (args.Count >= 2)
                                {
                                    var newCommand = args[1];

                                    CustomCommands.RemoveAll(cmd => cmd.Channel == command.ChatMessage.Channel && cmd.Command == newCommand);
                                    Save();
                                    client.SendMessage(command.ChatMessage.Channel, $"Command {newCommand} has been removed");
                                }
                                else
                                {
                                    client.SendMessage(command.ChatMessage.Channel, "Usage !command remove [command]");
                                }
                            }

                            else
                            {
                                client.SendMessage(command.ChatMessage.Channel, "Usage !command add/remove [command]");
                            }
                        }
                        else
                        {
                            client.SendMessage(command.ChatMessage.Channel, $"You're not a moderator {command.ChatMessage.Username} :)");
                        }
                    }
                    else
                    {
                        if (command.ChatMessage.IsModerator)
                        {
                            client.SendMessage(command.ChatMessage.Channel, $"Usage !command add [command] message]");
                        }

                        string commands = string.Join(", ", CustomCommands.Where(cmd => cmd.Channel == command.ChatMessage.Channel).Select(cmd => cmd.Command).ToArray());

                        client.SendMessage(command.ChatMessage.Channel, $"Commands: {commands}");
                    }
                }

                // Execute custom command
                foreach (var item in CustomCommands)
                {
                    if (item.Command == command.Command && item.Channel == command.ChatMessage.Channel)
                    {
                        var message = item.Response.Replace("{username}", command.ChatMessage.Username); //Display name will make the name blank if the user have no display name set;;
                        client.SendMessage(command.ChatMessage.Channel, message);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.WriteLine(e.TargetSite);

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void Save()
        {
            File.Create(memoryPath).Close();

            using (StreamWriter w = new StreamWriter(memoryPath))
            {
                foreach (var cmd in CustomCommands)
                {
                    w.WriteLine($"{cmd.Command}!@!~!@!{cmd.Response}!@!~!@!{cmd.Channel}");
                }
            }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(memoryPath)) return;

                using (StreamReader r = new StreamReader(memoryPath))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        string[] ncmd = line.Split(new[] { "!@!~!@!" }, StringSplitOptions.None);

                        CustomCommands.Add(new CCommand(ncmd[0],ncmd[1],ncmd[2]));

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{ncmd[0]} --- {ncmd[1]} --- { ncmd[2] }");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }

    /// <summary>
    /// Just a bit expandable, and keeps most of the old code A-OK :)
    /// </summary>
    class CCommand
    {
        public string Channel { get; set; }
        public string Response { get; set; }
        public string Command { get; set; }
        public CCommand(string command, string response, string channel)
        {
            Command = command;
            Response = response;
            Channel = channel;
        }
    }
}
