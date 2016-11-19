using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class NextStream : IPluginCommand
    {
        public string PluginName => "NextStream";
        public string Command => "next";
        public string Help => "!n will give the estimated time untill the next stream.";
        public IEnumerable<string> Aliases => new[] { "n", "nextstream", "countdown" };
        public bool Loaded { get; set; } = true;

        Dictionary<DayOfWeek, TimeSpan> streamtimes = new Dictionary<DayOfWeek, TimeSpan>();

        public NextStream()
        {
            streamtimes.Add(DayOfWeek.Monday, TimeSpan.FromHours(21)); //Monday at 8 Norweeb time
            streamtimes.Add(DayOfWeek.Wednesday, TimeSpan.FromHours(21)); 
            streamtimes.Add(DayOfWeek.Saturday, TimeSpan.FromHours(21)); 
        }

        public async void Execute(ChatCommand command, TwitchClient client)
        {
            if (command.ChatMessage.Channel == "jefmajor")
            {
                var uptime = await TwitchApi.GetUptime(command.ChatMessage.Channel);
                if (uptime.Ticks == 0)
                {
                    List<DateTime> times = new List<DateTime>();
                    foreach (var item in streamtimes)
                    {
                        DateTime start = DateTime.Now;
                        DateTime then = start.AddDays(((int)item.Key - (int)start.DayOfWeek + 7) % 7);
                        then = then.Date + item.Value; // sets the time from whatever to the 20'th hour
                        times.Add(then);
                    }
                    times.Sort((a, b) => a.CompareTo(b)); //ascending sort
                    TimeSpan span = times[0].Subtract(DateTime.Now);
                    client.SendMessage(command.ChatMessage.Channel, $"Next stream in {span.TotalHours}:{(int)span.Minutes}:{(int)span.Seconds}:{(int)span.Milliseconds}~~ on the {times[0].Day}th");
                    var arktime = await TwitchApi.GetUptime("arkentosh");
                    if (arktime.Ticks > 0)
                    {
                        client.SendMessage(command.ChatMessage.Channel, $"But arkentosh is online, so check him out maybe? https://www.twitch.tv/arkentosh :)");
                    }
                }
                else
                {

                    client.SendMessage(command.ChatMessage.Channel, $"He's on right now silly");
                }
            }
        }

        public void Discord(Message arg)
        {
            arg.Channel.SendMessage("Not implemented yet");
        }
    }
}
