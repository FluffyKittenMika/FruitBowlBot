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
        bool Loaded { get; }
        string Help { get; }

        void Execute(ChatCommand command, TwitchClient client);
        void Discord(SocketMessage arg, DiscordSocketClient discordClient);
        void MessageHandler(Message msg);
    }


    public class Message
    {
        public string MessageText { get; set; }
        public string Channel { get; set; }
        public string Command { get; set; }
        public string Username { get; set; }

    }
}
