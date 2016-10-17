using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class TrollPluginCommand : IPluginCommand
    {
        public string PluginName => "Troll";
        public string Command => "troll";
        public IEnumerable<string> Aliases => new[] { "t" };
        public bool Loaded { get; set; } = true;
        List<string> quotes = new List<string>();

        public async void Execute(ChatCommand command, TwitchClient client)
        {
            var quotefile = @"./RemoteQuotes.dat";
            Random rng = new Random();
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
