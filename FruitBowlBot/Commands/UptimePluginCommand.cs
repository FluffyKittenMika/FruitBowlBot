using Discord;
using Discord.WebSocket;
using System;
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
		public IEnumerable<string> Help => new[] { "!u returns the streams uptime" };
        public IEnumerable<string> Aliases => new[] { "u", "up", "uppity"};
        public bool Loaded { get; set; } = true;

        public string Res(string channel)
        {
            TimeSpan? uptime = TwitchAPI.Streams.v5.GetUptime(TwitchAPI.Channels.v3.GetChannelByName(channel).Result.Id).Result;
            if (uptime.HasValue)
                return $"Time: {uptime.Value.Hours}h {uptime.Value.Minutes}m {uptime.Value.Seconds}s";
            else
                return $"Channel offline";
        }

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = Uptime(message); }).ConfigureAwait(false);
            return res;
        }

        public string Uptime(Message message)
        {
            if (!message.MessageIsFromDiscord)
                return Res(message.Channel);

            if (message.Arguments.Count > 0)
                return Res(message.Arguments[0]);

            return "Please format it like this '!uptime channelname' example '!u jefmajor' or '!u arkentosh'";
        }
    }
}
