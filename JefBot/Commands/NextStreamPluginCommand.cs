using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class NextStreamPluginCommand : IPluginCommand
    {
        public string PluginName => "NextStream";
        public string Command => "next";
        public string Help => "!next will give the estimated time untill the next stream.";
        public IEnumerable<string> Aliases => new[] { "n", "nextstream", "countdown" };
        public bool Loaded { get; set; } = true;

        Dictionary<DayOfWeek, TimeSpan> streamtimes = new Dictionary<DayOfWeek, TimeSpan>();

        public NextStreamPluginCommand()
        {
            streamtimes.Add(DayOfWeek.Monday, TimeSpan.FromHours(21)); //+1 hour during norwegian winter time.
            streamtimes.Add(DayOfWeek.Wednesday, TimeSpan.FromHours(21)); 
            streamtimes.Add(DayOfWeek.Saturday, TimeSpan.FromHours(21)); 
        }

        public string Action(Message message)
        {
            if (message.Channel == "jefmajor")
                return Time();
            return null;
        }

        public string Time()
        {
            TimeSpan uptime = new TimeSpan();
            try
            {
                uptime = TwitchApi.Streams.GetUptime("jefmajor");
            }
            catch (Exception){}
            
            if (uptime.Ticks == 0)
            {
                List<DateTime> times = new List<DateTime>();
                DateTime start = DateTime.Now;

                foreach (var item in streamtimes)
                {
                    DateTime then = start.AddDays(((int)item.Key - (int)start.DayOfWeek + 7) % 7);
                    then = then.Date + item.Value; // sets the time from whatever to the 20'th hour
                    times.Add(then);
                }
                times.Sort((a, b) => a.CompareTo(b)); //ascending sort
                TimeSpan span = times[0].Subtract(DateTime.Now);
                if (span.Minutes < 0)
                {
                    span = times[1].Subtract(DateTime.Now);
                }
                return $"Next stream might be in {span.Days} Day(s), {span.Hours} Hour(s), {span.Minutes} Minute(s), {span.Seconds} Second(s), on the {(start + span).Day}{GetSuffix((start + span).Day)}. That being a total of {span.TotalHours} Hour(s) from now.";
            }
            else
                return $"He's on right now silly";
        }

        private string GetSuffix(int day)
        {
            return (day == 11 || day == 12 || day == 13) ? "th" : (day == 1) ? "st" : (day == 2) ? "nd" : (day == 3) ? "rd" : "th";
        }
    }
}
