using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using System.Text.RegularExpressions;
using Discord;
using TwitchLib.Models.Client;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace JefBot.Commands
{
    internal class MigoPluginCommand : IPluginCommand
    {
        public string PluginName => "Migo";
        public string Command => "migo";
        public string Help => "!m {search string}";
        public IEnumerable<string> Aliases => new[] { "m" };
        public bool Loaded { get; set; } = true;

        List<Quote> quotes = new List<Quote>();
        List<Quote> pickedquotes = new List<Quote>();
       // string quotefile = @"./RemoteQuotes.dat";
        Random rng = new Random();

        DateTime timestampTwitch;
        DateTime timestampDiscord;
        
        int minutedelay = 1;

        /*
        static string DatePattern = @"((\d{2}.\d{2}.\d{4}) (\d{2}.\d{2}.\d{2}))";
        static string SubmitterPattern = @"(?:submitted by )([a-zA-Z0-9_-]+)";
        Regex dateregex = new Regex(DatePattern, RegexOptions.IgnoreCase);
        Regex submitterregex = new Regex(SubmitterPattern, RegexOptions.IgnoreCase);
        */

        public MigoPluginCommand()
        {
            timestampDiscord = DateTime.UtcNow;
            timestampTwitch = DateTime.UtcNow;
            try
            {
                using (MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
                {
                    con.Open();
                    MySqlCommand _cmd = con.CreateCommand();
                    _cmd.CommandText = @"SELECT * FROM `Quotes`";
                    using (MySqlDataReader reader = _cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var quote = reader.GetString(reader.GetOrdinal("QUOTE"));
                            int id = reader.GetInt32(reader.GetOrdinal("ID"));
                            var submitter = reader.GetString(reader.GetOrdinal("SUBMITTER"));
                            DateTime timestamp = reader.GetDateTime(reader.GetOrdinal("TIMESTAMP"));
                            var channel = reader.GetString(reader.GetOrdinal("CHANNEL"));
                            quotes.Add(new Quote(quote, timestamp, submitter, channel, id));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Loaded = false;
                Console.WriteLine(e.Message);
            }
         
        }

        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (timestampTwitch.AddMinutes(minutedelay) < DateTime.UtcNow || command.ChatMessage.IsModerator || command.ChatMessage.IsBroadcaster)
            {
                if (!command.ChatMessage.IsModerator && !command.ChatMessage.IsBroadcaster)
                {
                    timestampTwitch = DateTime.UtcNow;
                }
                if (command.ArgumentsAsList.Count == 0)
                {
                    Quote qu = migo();
                    if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                    {
                        qu.SubmittedBy = "Unknown";
                    }
                    string q = $"{qu.Quotestring} submitted by {qu.SubmittedBy} #{qu.id}";
                    client.SendMessage(command.ChatMessage.Channel, q);
                }
                else
                {
                    Quote qu = searchMigo(command.ArgumentsAsString);
                    if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                    {
                        qu.SubmittedBy = "Unknown";
                    }
                    string q = $"{qu.Quotestring} QuoteID:{qu.id}";
                    client.SendMessage(q);
                }
            }
        }

        public void Discord(MessageEventArgs arg, DiscordClient client)
        {

            var args = arg.Message.Text.Split(' ').ToList().Skip(1).ToList();
            string argstring = string.Join(" ", args.ToArray());

            if (args.Count == 0)
            {
                timestampDiscord = DateTime.UtcNow;
                Quote qu = migo();
                if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                {
                    qu.SubmittedBy = "Unknown";
                }
                string q = $"```{qu.Quotestring}{Environment.NewLine}#{qu.id} by {qu.SubmittedBy}```";
                arg.Channel.SendMessage(q);
            }else
            {
                Quote qu = searchMigo(argstring);
                if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                {
                    qu.SubmittedBy = "Unknown";
                }
                string q = $"```{qu.Quotestring}{Environment.NewLine}#{qu.id} by {qu.SubmittedBy}```";
                arg.Channel.SendMessage(q);
            }
            
        }

        public Quote searchMigo(string search)
        {
            using (MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
            {
                con.Open();
                MySqlCommand _cmd = con.CreateCommand();
                _cmd.CommandText = @"SELECT * FROM Quotes WHERE MATCH(Quote) AGAINST(@input IN BOOLEAN MODE)";
                _cmd.Parameters.AddWithValue("@input", search);
                List<Quote> quotes = new List<Quote>();
                using (MySqlDataReader reader = _cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var quote = reader.GetString(reader.GetOrdinal("QUOTE"));
                        int id = reader.GetInt32(reader.GetOrdinal("ID"));
                        var submitter = reader.GetString(reader.GetOrdinal("SUBMITTER"));
                        DateTime timestamp = reader.GetDateTime(reader.GetOrdinal("TIMESTAMP"));
                        var channel = reader.GetString(reader.GetOrdinal("CHANNEL"));
                        quotes.Add(new Quote(quote, timestamp, submitter, channel, id));
                    }
                    reader.Close();
                    if (quotes.Count > 0)
                    {
                        return quotes[rng.Next(quotes.Count)];
                    }
                    else
                    {
                        Quote nonefound = migo();
                        nonefound.Quotestring = "Found no results, have this one instead: " + nonefound.Quotestring;
                        return nonefound;
                    }
                }
            }
          
        }
        public Quote migo()
        {
            var derp = quotes.ElementAt(rng.Next(0, quotes.Count));
            pickedquotes.Add(derp);
            quotes.Remove(derp);
            if (quotes.Count() < 5)
            {
                quotes.AddRange(pickedquotes);
                pickedquotes.Clear();
            }
            return derp;
        }
    }

    class Quote
    {
        public string Quotestring { get; set; }
        public DateTime Datesubmitted { get; set; }
        public string SubmittedBy { get; set; }
        public string Channel { get; set; }
        public int id { get; set; }
        public Quote(string quote, DateTime date, string submitter ="", string channel="",int id = 0)
        {
            Quotestring = quote;
            Datesubmitted = date;
            SubmittedBy = submitter;
            Channel = channel;
            this.id = id;
        }
    }
}
