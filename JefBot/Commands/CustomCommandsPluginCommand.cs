using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using TwitchLib.Models.Client;
using Discord.WebSocket;
using Discord;


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

        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {
            var args = arg.Content.Split(' ').ToList().Skip(1).ToList();
            var command = arg.Content.Split(' ').First();
            var response = CustomCommand(command.Substring(1), args,((SocketGuildUser)arg.Author).GuildPermissions.Administrator , Convert.ToString(arg.Channel.Id) , arg.Author.Username);
            if (response != "null")
            {
                arg.Channel.SendMessageAsync(response);
            }
        }

        public void Twitch(ChatCommand command, TwitchClient client)
        {
            var response = CustomCommand(command.Command, command.ArgumentsAsList, command.ChatMessage.IsModerator, command.ChatMessage.Channel, command.ChatMessage.Username);
            if (response != "null")
            {
                client.SendMessage(command.ChatMessage.Channel, response);
            }
        }

        /// <summary>
        /// handles the custom command
        /// </summary>
        /// <param name="command">main command method</param>
        /// <param name="args">arguments</param>
        /// <param name="moderator">if the user is an administrator</param>
        /// <returns></returns>
        private string CustomCommand(string command, List<string> args, bool moderator, string channel = null, string username = null)
        {
            try
            {
                // Main command methods
                if (
                    string.Equals(command, "command", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command, "commands", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command, "cmd", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command, "cmds", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    if (args.Count > 0)
                    {
                       if (moderator)
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

                                        //client.SendMessage(command.ChatMessage.Channel, $"I'm sorry, Dave. I'm afraid I can't do that.");
                                        //client.SendWhisper(command.ChatMessage.Username, $"Custom commands cannot run chat slash commands...");
                                        return $"Custom commands cannot run chat slash commands...";
                                    }

                                    CCommand cmd = new CCommand(newCommand, response, channel);

                                    CustomCommands.RemoveAll(cmdx => cmdx.Channel == channel && cmdx.Command == newCommand);

                                    CustomCommands.Add(cmd);
                                    Save();
                                    return $"Command {newCommand} has been added";
                                }
                                else
                                {
                                    return "Usage !command add (command) (message)";
                                }
                            }

                            else if (string.Equals(args[0], "remove", StringComparison.OrdinalIgnoreCase))
                            {
                                if (args.Count >= 2)
                                {
                                    var newCommand = args[1];

                                    CustomCommands.RemoveAll(cmd => cmd.Channel == channel && cmd.Command == newCommand);
                                    Save();
                                   return $"Command {newCommand} has been removed";
                                }
                                else
                                {
                                    return  "Usage !command remove [command]";
                                }
                            }

                            else
                            {
                              return "Usage !command add/remove [command]";
                            }
                        }
                        else
                        {
                           return $"You're not a moderator {username} :)";
                        }
                    }
                    else
                    {
                        if (moderator)
                        {
                            return  $"Usage !command add [command] message]";
                        }

                        string commands = string.Join(", ", CustomCommands.Where(cmd => cmd.Channel == channel).Select(cmd => cmd.Command).ToArray());

                       return $"Commands: {commands}";
                    }
                }

                // Twitch custom command
                foreach (var item in CustomCommands)
                {
                    if (item.Command == command && item.Channel == channel)
                    {
                        var message = item.Response.Replace("{username}", username); //Display name will make the name blank if the user have no display name set;;
                        return message;
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.WriteLine(e.TargetSite);
                Console.ForegroundColor = ConsoleColor.White;
                return e.Message;
            }
            return "null";
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

                        //Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.WriteLine($"{ncmd[0]} --- {ncmd[1]} --- { ncmd[2] }");
                        //Console.ForegroundColor = ConsoleColor.White;
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
