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
using Discord.WebSocket;

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




        public void Twitch(ChatCommand command, TwitchClient client)
        {
            if (timestampTwitch.AddMinutes(minutedelay) < DateTime.UtcNow || command.ChatMessage.IsModerator || command.ChatMessage.IsBroadcaster)
            {
                if (!command.ChatMessage.IsModerator && !command.ChatMessage.IsBroadcaster)
                {
                    timestampTwitch = DateTime.UtcNow;
                }
                if (command.ArgumentsAsList.Count == 0)
                {
                    Quote qu = Migo();
                    if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                    {
                        qu.SubmittedBy = "Unknown";
                    }
                    string q = $"{qu.Quotestring} submitted by {qu.SubmittedBy} #{qu.Id}";
                    client.SendMessage(command.ChatMessage.Channel, q);
                }
                else
                {
                    if (Int32.TryParse(command.ArgumentsAsString, out int x))
                    {
                        Quote qu = SearchMigo(x);
                        if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                        {
                            qu.SubmittedBy = "Unknown";
                        }
                        string q = $"{qu.Quotestring} QuoteID:{qu.Id}";
                        client.SendMessage(q);
                    }
                    else
                    {
                        Quote qu = SearchMigo(command.ArgumentsAsString);
                        if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                        {
                            qu.SubmittedBy = "Unknown";
                        }
                        string q = $"{qu.Quotestring} QuoteID:{qu.Id}";
                        client.SendMessage(q);
                    }
                  
                }
            }
        }

        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {

            var args = arg.Content.Split(' ').ToList().Skip(1).ToList();
            string argstring = string.Join(" ", args.ToArray());
            Quote qu;
            if (args.Count == 0)
            {
                timestampDiscord = DateTime.UtcNow;
                qu = Migo();
                if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                {
                    qu.SubmittedBy = "Unknown";
                }
                string q = $"```{qu.Quotestring}{Environment.NewLine}#{qu.Id} by {qu.SubmittedBy}```";
                arg.Channel.SendMessageAsync(q);
            }else
            {
                if (Int32.TryParse(argstring, out int x))
                {
                    qu = SearchMigo(x); //id search
                }
                else
                {
                    qu = SearchMigo(argstring);
                }
                if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                {
                    qu.SubmittedBy = "Unknown";
                }
                string q = $"```{qu.Quotestring}{Environment.NewLine}#{qu.Id} by {qu.SubmittedBy}```";
                arg.Channel.SendMessageAsync(q);
            }
            
        }

        public Quote SearchMigo(int search)
        {
            using (MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
            {
                con.Open();
                MySqlCommand _cmd = con.CreateCommand();
                _cmd.CommandText = @"SELECT * FROM `Quotes` WHERE `ID` = @input";
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
                        Quote nonefound = Migo();
                        nonefound.Quotestring = "Found no results, have this one instead: " + nonefound.Quotestring;
                        return nonefound;
                    }
                }
            }
        }

        public Quote SearchMigo(string search)
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
                        Quote nonefound = Migo();
                        nonefound.Quotestring = "Found no results, have this one instead: " + nonefound.Quotestring;
                        return nonefound;
                    }
                }
            }
          
        }
        public Quote Migo()
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
        public int Id { get; set; }
        public Quote(string quote, DateTime date, string submitter ="", string channel="",int id = 0)
        {
            Quotestring = quote;
            Datesubmitted = date;
            SubmittedBy = submitter;
            Channel = channel;
            this.Id = id;
        }
    }
}
