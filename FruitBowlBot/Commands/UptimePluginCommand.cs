using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JefBot.Commands
{
	internal class UptimePluginCommand : IPluginCommand
	{
		public string PluginName => "Uptime";
		public string Command => "uptime";
		public IEnumerable<string> Help => new[] { "!u returns the streams uptime" };
		public IEnumerable<string> Aliases => new[] { "u", "up", "uppity", "downtime" };
		public bool Loaded { get; set; } = true;

		public async Task<string> Action(Message message)
		{
			string res = null;
			await Task.Run(() => { res = Uptime(message).Result; }).ConfigureAwait(true);
			return res;
		}

		public async Task<string> Uptime(Message message)
		{
			string res = "Offline";
			var channelid = Bot.GetChannelIDAsync(message.Channel).Result;
			TimeSpan? waittime = await Bot.twitchAPI.Streams.v5.GetUptimeAsync(channelid);
			TimeSpan uptime = waittime.GetValueOrDefault();
			if (uptime.TotalSeconds > 0 && uptime != null)
				res = $"Time: {uptime.Hours.ToString()}h:{uptime.Minutes.ToString()}m:{uptime.Seconds.ToString()}s";
			return res;
		}


	}
}
