using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class MigoPluginCommand : IPluginCommand
    {
        public string PluginName => "Migo";
        public string Command => "migo";
        public IEnumerable<string> Aliases => new[] { "m" };
        public bool Loaded { get; set; } = true;
        public bool OffWhileLive { get; set; } = true;



        List<string> quotes = new List<string>();
        string quotefile = @"./RemoteQuotes.dat";
        Random rng = new Random();
        DateTime timestamp;
        int minutedelay = 5;
        public MigoPluginCommand()
        {
            timestamp = DateTime.UtcNow;
        }
        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (timestamp.AddMinutes(minutedelay) < DateTime.UtcNow || command.ChatMessage.IsModerator || command.ChatMessage.IsBroadcaster)
            {
                timestamp = DateTime.UtcNow;
                if (File.Exists(quotefile))
                {
                    using (StreamReader r = new StreamReader(quotefile))
                    {
                        string line;
                        while ((line = r.ReadLine()) != null)
                        {
                            string[] split = line.Split('|'); //Split the quotes
                            quotes.Add(split[0]); //Add the quotes to the list, we don't care for the other part.
                        }
                    }
                }

                string derp = quotes.ElementAt(rng.Next(0, quotes.Count));
                client.SendMessage(command.ChatMessage.Channel, $"{derp}");
            }

        }
    }
}
