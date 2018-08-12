using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib;

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
			/*TimeSpan uptime = getUptime(stream.Stream.CreatedAt);
			string HHmmss = uptime.Hours.Tostring() + ":" + uptime.Minutes.ToString() + ":" + uptime.Seconds.ToString();

			public TimeSpan getUptime(DateTime dt)
			{
				return DateTime.Now - dt;
			}*/

			TimeSpan? uptime = TwitchAPI.Streams.v5.GetUptimeAsync(TwitchAPI.Channels.v3.GetChannelByNameAsync(channel).Result.Id).Result;
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

			TimeSpan uptime = getUptime(channel.ChannelStream.CreatedAt);
			string HHmmss = uptime.Hours.Tostring() + ":" + uptime.Minutes.ToString() + ":" + uptime.Seconds.ToString();


			return Res(message.Channel);
        }

		public TimeSpan getUptime(DateTime dt)
		{
			return DateTime.Now - dt;
		}
	}
}
