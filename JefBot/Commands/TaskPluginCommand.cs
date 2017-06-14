using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using TwitchLib;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class TaskPluginCommand : IPluginCommand
    {
        public string PluginName => "Task";
        public string Command => "task";
        public string Help => "!task {minutes} {message} (repeats a message every X minutes)";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;
        
        //this is just a class for the future
        //not implementing this right now

        public string Action(Message message)
        {
            return null;
        }


    }
}
