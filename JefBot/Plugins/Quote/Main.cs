using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Plugins.Quote
{
    class Main : Plugin
    {
      
        public bool Loaded
        {
            get
            {
                return true;
            }
        }

        public string PluginName
        {
            get
            {
                return "Quote";
            }
        }

        public void OnChatCommandReceivedArgs(TwitchClient.OnChatCommandReceivedArgs args, TwitchClient client)
        {
            string command = args.Command.ToLower();
            if (command == "q" || command == "quote")
            {
                //passive agressie anti double quote checker
                bool quoted = false;
                if (args.ArgumentsAsString[0] == '"')
                    quoted = true;

                using (StreamWriter w = File.AppendText("quotes.txt"))
                    w.Write($"\"{args.ArgumentsAsString}\"| {DateTime.Now} submitted by {args.ChatMessage.DisplayName}" + Environment.NewLine);

                if (!quoted)
                    client.SendMessage(new JoinedChannel(args.Channel), "👌 Thanks!");
                else
                    client.SendMessage(new JoinedChannel(args.Channel), "👌 please don't add \" to the quotes yourself :)");
            }
        }

        public void OnConnectedArgs(TwitchClient.OnConnectedArgs args, TwitchClient client)
        {
        }

        public void OnMessageReceivedArgs(TwitchClient.OnMessageReceivedArgs args, TwitchClient client)
        {
        }

        public void OnNewSubscriberArgs(TwitchClient.OnNewSubscriberArgs args, TwitchClient client)
        {
        }

        public void OnReSubscriberArgs(TwitchClient.OnReSubscriberArgs args, TwitchClient client)
        {
        }

        public void RecivedResub(TwitchClient.OnReSubscriberArgs args, TwitchClient client)
        {
        }
    }
}
