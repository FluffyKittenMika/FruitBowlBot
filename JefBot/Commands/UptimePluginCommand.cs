using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class UptimePluginCommand : IPluginCommand
    {
        public string PluginName => "Uptime";
        public string Command => "uptime";
        public string Help => "!u returns the streams uptime";
        public IEnumerable<string> Aliases => new[] { "u", "up" };
        public bool Loaded { get; set; } = true;

        public async void Execute(ChatCommand command, TwitchClient client)
        {
            var uptime = await TwitchApi.GetUptime(command.ChatMessage.Channel);
            if (uptime.Ticks > 0)
            {
                client.SendMessage(
                    command.ChatMessage.Channel,
                    $"Time: {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s"
                    );
            }
            else
            {
                client.SendMessage(
                    command.ChatMessage.Channel,
                    "He's offline I think? :)"
                    );
                var arktime = await TwitchApi.GetUptime("arkentosh");
                if (arktime.Ticks > 0)
                {
                    client.SendMessage(command.ChatMessage.Channel, $"But arkentosh is online, so check him out maybe? https://www.twitch.tv/arkentosh :)");
                }
            }
        }
    }
}
