using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using TwitchLib;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class UptimePluginCommand : IPluginCommand
    {
        public string PluginName => "Uptime";
        public string Command => "uptime";
        public string Help => "!u returns the streams uptime";
        public IEnumerable<string> Aliases => new[] { "u", "up" };
        public bool Loaded { get; set; } = true;

        public void Twitch(ChatCommand command, TwitchClient client)
        {
            client.SendMessage(Res(command.ChatMessage.Channel));
        }

        public string Res(string channel)
        {
            var uptime = TwitchApi.Streams.GetUptime(channel);
            if (uptime.Ticks > 0)
                return $"Time: {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
            else
                return $"Channel offline";
        }

        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {
            var args = arg.Content.Split(' ').ToList().Skip(1).ToList();
            if (args.Count == 0)
            {
                arg.Channel.SendMessageAsync("no channel specified");
            }
            else
            {
                arg.Channel.SendMessageAsync(Res(args[0]));
            }

        }
    }
}
