using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace JefBot.Commands
{
    internal class VarPluginCommand : IPluginCommand
    {
        public string PluginName => "var";
        public string Command => "var";
		public IEnumerable<string> Help => new[] { "!v {commands}" };
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;
        
        async Task<string> IPluginCommand.Action(Message message)
        {
            string res = null;
            if (message.IsModerator)
                await Task.Run(() => { res = Commands(message); }).ConfigureAwait(false);
            return res;
        }

        public string Commands(Message message)
        {
            List<string> responses = new List<string>();

            var enabledPlugins = Bot._plugins.Where(plug => plug.Loaded).ToArray();

            //join args list into 1d string, and space them like a normal string, with spaces..
            string temp = string.Join(" ", message.Arguments);

            //make argument list holder, will skip each command
            //split at every prefix, and make this list of strings like !command arg1 arg2 arg3, aka regular unparsed commands
            List<string> args = Regex.Split(temp, $"(?={Bot.settings["prefix"][0]})").Skip(1).ToList();
            
            foreach (string commands in args)
            {
                string command = commands.Split(' ').First();
                if (command[0] == '!')
                {
                    command = command.Substring(1);
                }
                foreach (var plug in enabledPlugins)
                {
                    if (plug.Aliases.Contains(command) || plug.Command == command)
                    {
                        Message m = new Message {
                            Command = command,
                            IsModerator = message.IsModerator,
                            Channel = message.Channel,
                            Username = message.Username,
                            RawMessage = message.RawMessage, //and this is why you don't use raw for anything but debug, 'cause i just added potential bugs by nesting commands
                            Arguments = commands.Split(' ').Skip(1).ToList()
                        };
                        responses.Add(plug.Action(m).Result);
                    }
                }
            }
            return string.Join(Environment.NewLine, responses); 
        }
    }
}
