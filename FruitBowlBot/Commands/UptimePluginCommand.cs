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
			await Task.Run(() => { res = Uptime(message); }).ConfigureAwait(false);
			return res;
		}

		public string Uptime(Message message)
		{
			string res = "Offline";
			TimeSpan uptime = Bot.twitchAPI.Streams.v5.GetUptimeAsync(Bot.GetChannelIDAsync(message.Channel).Result).Result.Value;
			if (uptime.TotalSeconds > 0)
				res = uptime.Hours.ToString() + ":" + uptime.Minutes.ToString() + ":" + uptime.Seconds.ToString();
			return res;
		}


	}
}
