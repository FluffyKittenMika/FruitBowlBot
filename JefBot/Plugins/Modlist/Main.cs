﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Plugins.Modlist
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
                return "Modlist";
            }
        }

        public void OnChatCommandReceivedArgs(TwitchClient.OnChatCommandReceivedArgs args, TwitchClient client)
        {
            string command = args.Command.Command.ToLower();
            if (command == "modlist")
            {
                string output = "";
                using (StreamReader r = new StreamReader("./Plugins/Modlist/Memory.txt")) //don't worry about it ok?
                     output = r.ReadToEnd();
                client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel), output);
            }
            if (command == "set")
            {
                if (args.Command.ArgumentsAsList[0] == "modlist" && args.Command.ChatMessage.IsModerator)
                {
                    string newlist = "";
                    for (int i = 1; i < args.Command.ArgumentsAsList.Count; i++)
                    {
                        newlist += args.Command.ArgumentsAsList[i] + " ";
                    }
                    using (StreamWriter w = new StreamWriter("./Plugins/Modlist/Memory.txt"))
                        w.Write(newlist);
                    client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel), "Modlist updated");
                }
                else
                {
                    client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel), "You're not a moderator");
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