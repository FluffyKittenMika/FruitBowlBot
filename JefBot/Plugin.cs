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
    }
}
