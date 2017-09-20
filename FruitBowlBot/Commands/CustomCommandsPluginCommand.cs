using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using TwitchLib.Models.Client;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;

namespace JefBot.Commands
{
	class CustomCommandsPluginCommand : IPluginCommand
	{
		public string PluginName => "Custom Commands";
		public string Command => "command";
		public IEnumerable<string> Help => new[] { "!cmd add {command name} {command result}" };
        public string[] OtherAliases = { "commands", "cmd", "cmds" };
        public IEnumerable<string> Aliases => CustomCommands.Select(command => command.Command).ToArray().Concat(OtherAliases);
        public List<CCommand> CustomCommands = new List<CCommand>();
        public bool Loaded { get; set; } = true;
		

		private readonly string memoryPath = "./customCommands.txt";

        public CustomCommandsPluginCommand()
        {
            Load();
        }

        async Task<string> IPluginCommand.Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = CustomCommand(message); }).ConfigureAwait(false);
            if (res != "null" && res != null)
                return res;
            else
                return null;
        }

        /// <summary>
        /// handles the custom command
        /// </summary>
        /// <param name="command">main command method</param>
        /// <param name="args">arguments</param>
        /// <param name="moderator">if the user is an administrator</param>
        /// <returns></returns>
        private string CustomCommand(Message msg)
        {
            try
            {
				
                // Main command methods
                if (
                    string.Equals(msg.Command, "command", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(msg.Command, "commands", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(msg.Command, "cmd", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(msg.Command, "cmds", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    if (msg.Arguments.Count > 0)
                    {
						if (msg.Arguments[0].ToLower() == "list")
						{
							return string.Join(", ", CustomCommands.Where(cmd => cmd.Channel == msg.Channel).Select(cmd => cmd.Command).ToArray());
						}

						if (msg.IsModerator)
                        {
                            if (string.Equals(msg.Arguments[0], "add", StringComparison.OrdinalIgnoreCase))
                            {
                                if (msg.Arguments.Count >= 3)
                                {
                                    var newCommand = msg.Arguments[1];
                                    var response = string.Join(" ", msg.Arguments.Skip(2));

                                    // Looks like someone is trying to run a command!
                                    if (response.StartsWith("/") && !response.StartsWith("/me"))
                                        return $"Custom commands cannot run chat slash commands...";
									
                                    CCommand cmd = new CCommand(newCommand, response, msg.Channel);

									//removes existing commands if they have the same command name, also checks if the command is in the same channel.
                                    CustomCommands.RemoveAll(cmdx => cmdx.Channel == msg.Channel && cmdx.Command == newCommand);

                                    CustomCommands.Add(cmd);
                                    Save();
                                    return $"Command {newCommand} has been added";
                                }
                                else
                                {
                                    return "Usage !command add (command) (message)";
                                }
                            }

                            else if (string.Equals(msg.Arguments[0], "remove", StringComparison.OrdinalIgnoreCase))
                            {
                                if (msg.Arguments.Count >= 2)
                                {
                                    var oldCommand = msg.Arguments[1];
                                    CustomCommands.RemoveAll(cmd => cmd.Channel == msg.Channel && cmd.Command == oldCommand);
                                    Save();
                                    return $"Command {oldCommand} has been removed";
                                }
                                else
                                    return "Usage !command remove [command]";
                            }
                            else
                                return "Usage !command add/remove [command]";
                        }
                        else
                            return $"You're not a moderator {msg.Username} :)";
                    }
                    else
                    {
						string commands = string.Empty;
						string list = string.Join(", ", CustomCommands.Where(cmd => cmd.Channel == msg.Channel).Select(cmd => cmd.Command).ToArray());

						if (msg.IsModerator)
							commands = $"Usage !command add [command] [message], also the current commands are : " + list;
						else
							commands = "Custom Commands are: " + list;
						
						return commands;
                    }
                }

                // Keyword replacement parser
                foreach (var item in CustomCommands)
                {
					if (string.Equals(item.Command, msg.Command, StringComparison.OrdinalIgnoreCase) && item.Channel == msg.Channel)
                    {
                        var message = item.Response.Replace("{username}", msg.Username); //Display name will make the name blank if the user have no display name set
							message = message.Replace("{args}", String.Join(" ", msg.Arguments));
							message = message.Replace("{args_}", String.Join("_", msg.Arguments));
							message = message.Replace("{args+}", String.Join("+", msg.Arguments));

						//TODO: there's definitly a better way to do this
						if (msg.Arguments.Count >= 1)
                            message = message.Replace("{arg1}", msg.Arguments[0]);
                        else
                            message = message.Replace("{arg1}", string.Empty);
                        if (msg.Arguments.Count >= 2)
                            message = message.Replace("{arg2}", msg.Arguments[1]);
                        else
                            message = message.Replace("{arg2}", string.Empty);
                        if (msg.Arguments.Count >= 3)
                            message = message.Replace("{arg3}", msg.Arguments[2]);
                        else
                            message = message.Replace("{arg3}", string.Empty);
                        if (msg.Arguments.Count >= 4)
                            message = message.Replace("{arg4}", msg.Arguments[3]);
                        else
                            message = message.Replace("{arg3}", string.Empty);
                        if (msg.Arguments.Count >= 5)
                            message = message.Replace("{arg5}", msg.Arguments[4]);
                        else
                            message = message.Replace("{arg5}", string.Empty);
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
                    w.WriteLine($"{cmd.Command}!@!~!@!{cmd.Response}!@!~!@!{cmd.Channel}");
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
                        CustomCommands.Add(new CCommand(ncmd[0], ncmd[1], ncmd[2]));
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
