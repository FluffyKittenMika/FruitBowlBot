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
        public IEnumerable<string> Aliases => new[] { "n", "nextstream", "countdown" };
        public bool Loaded { get; set; } = true;

        Dictionary<DayOfWeek, TimeSpan> streamtimes = new Dictionary<DayOfWeek, TimeSpan>();

        public NextStream()
        {
            streamtimes.Add(DayOfWeek.Friday, TimeSpan.FromHours(20)); //Friday at 8 Norweeb time
            streamtimes.Add(DayOfWeek.Monday, TimeSpan.FromHours(20)); 
            streamtimes.Add(DayOfWeek.Wednesday, TimeSpan.FromHours(20)); 
            streamtimes.Add(DayOfWeek.Saturday, TimeSpan.FromHours(20)); 

        }

        public void Execute(ChatCommand command, TwitchClient client)
        {
            Console.WriteLine("works?");
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


            client.SendMessage(command.ChatMessage.Channel, $"Time to next stream H:{(int)span.TotalHours} M:{(int)span.Minutes} S:{(int)span.Seconds}");

            foreach (var item in times)
            {
                Console.WriteLine(item.Date.Day); 
            }

        }

    }
}
