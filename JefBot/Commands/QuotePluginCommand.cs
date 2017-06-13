using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using System.Net;
using Discord;
using System.Linq;
using TwitchLib.Models.Client;
using MySql.Data.MySqlClient;
using Discord.WebSocket;

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
        Random rnd = new Random();

        public string Action(Message message)
        {
            if (message.Arguments.Count > 0)
            {
                if (message.Channel == "236951447634182145")
                    message.Channel = "jefmajor";
                return Quote(message.Arguments, message.Channel, message.Username);
            }
            return null;
        }

        public string Quote(List<string> args, string channel, string username)
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
                    _cmd.Parameters.AddWithValue("@QUOTE", '"'+quote+'"');
                    _cmd.Parameters.AddWithValue("@SUBMITTER", username);
                    _cmd.Parameters.AddWithValue("@CHANNEL", channel);
                    _cmd.ExecuteNonQuery();
                }

                //keeping this for safekeeping quotes
                using (var w = File.AppendText(channel + "_quotes.txt"))
                    w.Write($"\"{quote.Replace('|', ' ')}\"| {DateTime.Now} submitted by {username}" + Environment.NewLine);

                if (!quoted)
                    switch (rnd.Next(4))
                    {
                        case 0:
                            return "Quote submitted! 👌";
                        case 1:
                            return "👌 Thanks!";
                        case 2:
                            return "Thanks for the quote!";
                        case 3:
                            return "Great! Thanks for the Quote!";
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
