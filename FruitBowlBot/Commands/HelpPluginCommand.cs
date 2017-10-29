using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JefBot.Commands
{
    internal class HelpPluginCommand : IPluginCommand
    {
        public string PluginName => "Help";
        public string Command => "help";
		public IEnumerable<string> Help => new[] { "!help {command}" };
        public IEnumerable<string> Aliases => new[] { "h" };
        public bool Loaded { get; set; } = true;

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = CommandHelp(message); }).ConfigureAwait(false);
            return res;
        }
        
        public string CommandHelp(Message message)
        {
            try
            {
                if (message.Arguments.Count > 0)
                {
                    var args = message.Arguments;
                    var result = "";
                    List<IPluginCommand> plug = new List<IPluginCommand>();

                    plug.AddRange(Bot._plugins.Where(p => p.Aliases.Contains(args[0])).ToList());
                    plug.AddRange(Bot._plugins.Where(p => p.Command == args[0]).ToList());

                    foreach (var item in plug)
                    {
                        if (item.Command == args[0] || item.Aliases.Contains(args[0]))
                        {
                            result = string.Join(Environment.NewLine, item.Help);
                            break;
                        }
                    }
                    if (result == "" || result == null)
                        result = $"No command / alias found for {args[0]} and therefore no help can be given";
                    return $"{result}";
                }
                return $"{Help}";
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
                return e.Message;
            }
        }
    }
}
