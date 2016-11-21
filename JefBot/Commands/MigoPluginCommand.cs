using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using System.Text.RegularExpressions;
using Discord;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class MigoPluginCommand : IPluginCommand
    {
        public string PluginName => "Migo";
        public string Command => "migo";
        public string Help => "Gets a random quote from the stream";
        public IEnumerable<string> Aliases => new[] { "m" };
        public bool Loaded { get; set; } = true;



        List<Quote> quotes = new List<Quote>();
        string quotefile = @"./RemoteQuotes.dat";
        Random rng = new Random();
        DateTime timestamp;
        int minutedelay = 1;
        static string DatePattern = @"((\d{2}.\d{2}.\d{4}) (\d{2}.\d{2}.\d{2}))";
        static string SubmitterPattern = @"(?:submitted by )([a-zA-Z0-9_-]+)";
        Regex dateregex = new Regex(DatePattern, RegexOptions.IgnoreCase);
        Regex submitterregex = new Regex(SubmitterPattern, RegexOptions.IgnoreCase);
        public MigoPluginCommand()
        {
            timestamp = DateTime.UtcNow;
        }
        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (command.ChatMessage.Channel == "jefmajor")
            {
                if (timestamp.AddMinutes(minutedelay) < DateTime.UtcNow || command.ChatMessage.IsModerator || command.ChatMessage.IsBroadcaster)
                {
                    timestamp = DateTime.UtcNow;
                    client.SendMessage(command.ChatMessage.Channel, migo());
                }
            }
        }

        public void Discord(MessageEventArgs arg)
        {
            arg.Channel.SendMessage(migo());
        }



        public string migo()
        {
            if (File.Exists(quotefile))
            {
                using (StreamReader r = new StreamReader(quotefile))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        string[] split = line.Split('|'); //Split the quotes
                        string date = "";
                        string submitter = "";

                        try
                        {
                            date = dateregex.Match(split[1]).Value;
                            submitter = submitterregex.Match(split[1]).Groups[1].Value;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        quotes.Add(new Quote(split[0], date, submitter)); //Add the quotes to the list, we don't care for the other part.
                    }
                }
            }

            var derp = quotes.ElementAt(rng.Next(0, quotes.Count));
            return $"{derp.Quotestring} -submitted by: {derp.SubmittedBy}";
            //client.SendMessage(command.ChatMessage.Channel, $"{derp.Quotestring} -submitted by: {derp.SubmittedBy}");
        }

    }






    class Quote
    {
        public string Quotestring { get; set; }
        public string Datesubmitted { get; set; }
        public string SubmittedBy { get; set; }
        public string Channel { get; set; }
        public Quote(string quote, string date = "", string submitter ="", string channel="")
        {
            Quotestring = quote;
            Datesubmitted = date;
            SubmittedBy = submitter;
            Channel = channel;
        }
    }
}
