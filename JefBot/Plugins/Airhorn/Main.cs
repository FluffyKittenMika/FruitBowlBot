using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Plugins.Airhorn
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
                return "Airhorn";
            }
        }

        public void OnChatCommandReceivedArgs(TwitchClient.OnChatCommandReceivedArgs args, TwitchClient client)
        {
            string command = args.Command.Command.ToLower();

            if (command == "air" || command == "airhorn" || command == "a")
            {
                if (args.Command.ChatMessage.Subscriber)
                {
                    client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel), "jspAirhorn jspAirhorn jspAirhorn");
                }
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

        public void Shutdown()
        {
        }
    }
}
