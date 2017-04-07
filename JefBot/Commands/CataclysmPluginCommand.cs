using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib;
using TwitchLib.Models.Client;
using System.Net;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace JefBot.Commands
{
    internal class CataclysmPluginCommand : IPluginCommand
    {
        public string PluginName => "Cataclysm Wiki Lookup";
        public string Command => "lookup";
        public string Help => "!l topic/item";
        public IEnumerable<string> Aliases => new[] { "l", "look" };
        public bool Loaded { get; set; } = false;

        public enum chattype { Twitch, Discord };
        private readonly Random _rand = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
                client.SendMessage(command.ChatMessage.Channel, lookup(command.ArgumentsAsList,chattype.Twitch));
        }


        public string lookup(List<string> args, chattype type)
        {
            var message = string.Empty;
            //get time
            var search = "http://cddawiki.chezzo.com/cdda_wiki/api.php?action=query&format=xml&prop=revisions&rvprop=content&titles=" + string.Join(",", args);

            string ResponseText;
            HttpWebRequest myRequest =(HttpWebRequest)WebRequest.Create(search);
            using (HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    ResponseText = reader.ReadToEnd();
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ResponseText);

            var node = doc.GetElementsByTagName("rev")[0];

            try
            {
                string ss = node.InnerText;
                Regex regex = new Regex("<[^>]*>");
                message = regex.Replace(ss, string.Empty);
                if (message.Length > 100)
                {
                    message = message.Substring(0, 100);
                }
            }
            catch (Exception e)
            {
                message = "No valid results";
                Console.WriteLine(e.Message);
            }
            


            return message;
        }

        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {
            var args = arg.Content.Split(' ').ToList().Skip(1).ToList(); //this is probably so wrong
            arg.Channel.SendMessageAsync(lookup(args, chattype.Discord));

        }

    }
}
