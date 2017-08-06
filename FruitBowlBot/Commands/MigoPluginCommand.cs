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
using System.Threading.Tasks;

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
        Random rng = new Random();

        DateTime timestampTwitch;
        readonly int minutedelay = 1;
        
        public MigoPluginCommand()
        {
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

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = MigoAction(message); }).ConfigureAwait(false);
            return res;
        }

        public string MigoAction(Message message)
        {
            try
            {
                if (timestampTwitch.AddMinutes(minutedelay) < DateTime.UtcNow || message.IsModerator)
                {
                    if (!message.IsModerator)
                        timestampTwitch = DateTime.UtcNow;
                    if (message.Arguments.Count == 0)
                    {
                        Quote qu = Migo();
                        if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                            qu.SubmittedBy = "Unknown";
                        return $"{qu.Quotestring} submitted by {qu.SubmittedBy} #{qu.Id}";
                    }
                    else
                    {
                        string msg = String.Join(" ", message.Arguments);
                        if (Int32.TryParse(msg, out int x))
                        {
                            Quote qu = SearchMigo(x);
                            if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                                qu.SubmittedBy = "Unknown";
                            return $"{qu.Quotestring} QuoteID:{qu.Id}";
                        }
                        else
                        {
                            Quote qu = SearchMigo(msg);
                            if (qu.SubmittedBy == null || qu.SubmittedBy == "")
                                qu.SubmittedBy = "Unknown";
                            return $"{qu.Quotestring} QuoteID:{qu.Id}";
                        }

                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }
        

        public Quote SearchMigo(int search)
        {
            using (MySqlConnection con = new MySqlConnection(Bot.SQLConnectionString))
            {
                con.Open();
                MySqlCommand _cmd = con.CreateCommand();
                _cmd.CommandText = @"SELECT * FROM `Quotes` WHERE `ID` = @input";
                _cmd.Parameters.AddWithValue("@input", search);
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
                    if (quotes.Count > 0)
                        return quotes[rng.Next(quotes.Count)];
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
                    if (quotes.Count > 0)
                        return quotes[rng.Next(quotes.Count)];
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
