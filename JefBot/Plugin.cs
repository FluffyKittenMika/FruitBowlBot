using Discord;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.Models.Client;

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
        void Discord(MessageEventArgs arg);
    }
}
