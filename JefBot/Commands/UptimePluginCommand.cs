using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public string Res(string channel)
        {
            var uptime = TwitchApi.Streams.GetUptime(channel);
            if (uptime.Ticks > 0)
                return $"Time: {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
            else
                return $"Channel offline";
        }

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = Uptime(message); });
            return res;
        }

        public string Uptime(Message message)
        {
            if (!message.MessageIsFromDiscord)
                return Res(message.Channel);

            if (message.Arguments.Count > 0)
                return Res(message.Arguments[0]);

            return "Can't do that sir, I require an argument after the command.";
        }
    }
}
