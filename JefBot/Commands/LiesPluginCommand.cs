using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class LiesPluginCommand : IPluginCommand
    {
        public string PluginName => "Lies";
        public string Command => "lies";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = false;

        public void Execute(ChatCommand command, TwitchClient client)
        {
            client.SendMessage(command.ChatMessage.Channel, "Fucking Lies! http://imgur.com/B5reLPR");
        }
    }
}
