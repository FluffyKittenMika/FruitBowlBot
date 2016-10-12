using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class HelpPluginCommand : IPluginCommand
    {
        public string PluginName => "Help";
        public string Command => "help";
        public IEnumerable<string> Aliases => new[] { "h" };
        public bool Loaded { get; set; } = true;

        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (command.ChatMessage.IsModerator)
            {
                client.SendRaw($"PRIVMSG #{command.ChatMessage.Channel} :/w {command.ChatMessage.Username} hey mod!, you can also do !set modlist [text] without brackets, to change that, or !command add/remove [command] [result] for custom commands (don't do !command add uptime, it's untested help)");
            }

            client.SendMessage(new JoinedChannel(command.ChatMessage.Channel), "Just do !quote or !q and some text after it to send a quote in for review");
        }
    }
}
