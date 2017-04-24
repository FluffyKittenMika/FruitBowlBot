using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class UnDeleteTrigger : IPluginCommand
    {
        public string PluginName => "Undelete";
        public string Command => "";
        public string Help => "Undeletes the deleted memes";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;

        public void Execute(ChatCommand command, TwitchClient client)
        {
           
        }
       
        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {

        }

    }
}
