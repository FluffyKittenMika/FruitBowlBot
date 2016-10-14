using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using TwitchLib.TwitchClientClasses;
using System.Net;

namespace JefBot.Commands
{
    internal class QuotePluginCommand : IPluginCommand
    {
        public string PluginName => "Quote";
        public string Command => "quote";
        public IEnumerable<string> Aliases => new[] { "q" };
        public bool Loaded { get; set; } = true;

        //Non default definitions
        public List<string> quotes = new List<string>();

        /// <summary>
        /// The Constructor in this case just downloads the pre exsisting quote list off the website
        /// </summary>
        public QuotePluginCommand()
        {
            WebClient web = new WebClient();
            var quotefile = @"./RemoteQuotes.dat";
            web.DownloadFile(@"https://jefmajor.com/Quotes/FlatFileDatabase.memes", quotefile);
           
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
            }else
            {
                Console.WriteLine($"Could not Download remote quotes, please check if jefmajor.com is accessible for memery");
            }
        }

        public void Execute(ChatCommand command, TwitchClient client)
        {
            //passive agressie anti double quote checker
            var quoted = command.ArgumentsAsString[0] == '"';

            using (var w = File.AppendText(command.ChatMessage.Channel+"_quotes.txt"))
                w.Write($"\"{command.ArgumentsAsString.Replace('|',' ')}\"| {DateTime.Now} submitted by {command.ChatMessage.Username}" + Environment.NewLine);

            if (!quoted)
                client.SendMessage(command.ChatMessage.Channel, "👌 Thanks!");
            else
                client.SendMessage(command.ChatMessage.Channel, "👌 please don't add \" to the quotes yourself :)");

        }
    }
}
