using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using System.Net;
using Discord;
using System.Linq;
using TwitchLib.Models.Client;
using MySql.Data.MySqlClient;

namespace JefBot.Commands
{
    internal class QuotePluginCommand : IPluginCommand
    {
        public string PluginName => "Quote";
        public string Command => "quote";
        public string Help => "Just do !q and some text after it to create a quote, and don't add your own \" tags :) ";
        public IEnumerable<string> Aliases => new[] { "q" };
        public bool Loaded { get; set; } = true;

        //Non default definitions
        public List<string> quotes = new List<string>();
        Random rnd = new Random();

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
            if (command.ArgumentsAsList.Count > 0)
            {
                    client.SendMessage(command.ChatMessage.Channel, quote(command.ArgumentsAsList,command.ChatMessage.Channel,command.ChatMessage.Username));
            }
        }

        public void Discord(MessageEventArgs arg)
        {
            var args = arg.Message.Text.Split(' ').ToList().Skip(1).ToList(); //this is probably so wrong

            if (args.Count > 0)
            {
                arg.Channel.SendMessage(quote(args, "jefmajor", arg.User.Name));
            }
        }

        public string quote(List<string> args, string channel, string username)
        {
            if (args.Count > 0)
            {

                string quote = string.Join(" ", args.ToArray());

                //passive agressie anti double quote checker
                var quoted = args.ToString()[0] == '"';

                using (MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
                {
                    con.Open();
                    MySqlCommand _cmd = con.CreateCommand();
                    _cmd.CommandText = "INSERT INTO `Quotes` (`ID`, `QUOTE`, `SUBMITTER`, `CHANNEL`, `TIMESTAMP`) VALUES (NULL, @QUOTE, @SUBMITTER, @CHANNEL, CURRENT_TIMESTAMP)";
                    _cmd.Parameters.AddWithValue("@QUOTE", quote);
                    _cmd.Parameters.AddWithValue("@SUBMITTER", username);
                    _cmd.Parameters.AddWithValue("@CHANNEL", channel);
                    _cmd.ExecuteNonQuery();
                }

                //keeping this for safekeeping quotes
                using (var w = File.AppendText(channel + "_quotes.txt"))
                    w.Write($"\"{quote.Replace('|', ' ')}\"| {DateTime.Now} submitted by {username}" + Environment.NewLine);

                if (!quoted)
                    switch (rnd.Next(3))
                    {
                        case 0:
                            return "Quote submitted! 👌";
                        case 1:
                            return "👌 Thanks!";
                        case 2:
                            return "Thanks for the quote!";
                        default:
                            return "Quote sent for review! 👌";
                    }
                else
                    return "👌 please don't add \" to the quotes yourself :)";

            }
            return "no";
        }
    }
}
