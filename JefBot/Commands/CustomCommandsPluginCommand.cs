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
    internal class CustomCommandsPluginCommand : IPluginCommand
    {
        public string PluginName => "Custom Commands";
        public string Command => "command";
        public IEnumerable<string> Aliases => CustomCommands.Keys;
        public Dictionary<string, string> CustomCommands = new Dictionary<string, string>();
        public bool Loaded { get; set; } = true;

        private string memoryPath = "./customCommands.txt";

        public CustomCommandsPluginCommand()
        {
            Load();
        }

        public void Execute(ChatCommand command, TwitchClient client)
        {
            // Main command methods
            if (string.Equals(command.Command, "command", StringComparison.OrdinalIgnoreCase))
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
                                var respone = string.Join(" ", args.Skip(2));

                                // todo: Currently we just remove it if exits, could perhaps make it so we have to remove first.
                                if (CustomCommands.ContainsKey(newCommand))
                                    CustomCommands.Remove(newCommand);

                                CustomCommands.Add(newCommand, respone);
                                Save();
                                client.SendMessage(command.ChatMessage.Channel, $"Command {newCommand} has been added");
                            }
                            else
                            {
                                client.SendMessage(command.ChatMessage.Channel, "Usage !command add [command] [message]");
                            }
                        }

                        else if (string.Equals(args[0], "remove", StringComparison.OrdinalIgnoreCase))
                        {
                            if (args.Count >= 2)
                            {
                                var newCommand = args[1];
                                CustomCommands.Remove(newCommand);
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

                    client.SendMessage(command.ChatMessage.Channel, $"Commands: {string.Join(", ", CustomCommands.Keys)}");
                }
            }

            // Execute custom command
            if (CustomCommands.ContainsKey(command.Command))
            {
                client.SendMessage(command.ChatMessage.Channel, CustomCommands[command.Command]);
            }
        }

        private void Save()
        {
            File.Create(memoryPath).Close();

            using (StreamWriter w = new StreamWriter(memoryPath))
            {
                foreach (var cmd in CustomCommands)
                {
                    w.WriteLine($"{cmd.Key}!@!~!@!{cmd.Value}");
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

                        CustomCommands.Add(ncmd[0], ncmd[1]);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{ncmd[0]} --- {ncmd[1]}");
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
}
