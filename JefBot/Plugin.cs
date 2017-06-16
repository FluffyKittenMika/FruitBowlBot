using System.Collections.Generic;
using TwitchLib;
using TwitchLib.Models.Client;
using Discord.WebSocket;

namespace JefBot
{
    public interface IPluginCommand
    {
        string PluginName { get; }
        string Command { get; }
        IEnumerable<string> Aliases { get; }
        bool Loaded { get; set; }
        string Help { get; }

        string Action(Message message);
    }


    public class Message
    {
        public string RawMessage { get; set; }
        public string Channel { get; set; }
        public string Command { get; set; }
        public string Username { get; set; }
        public bool MessageIsFromDiscord { get; set; }

        public bool IsModerator { get; set; }
        public List<string> Arguments { get; set; }
    }
}
