using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Plugins.Uptime
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
                return "Uptime";
            }
        }

        public async void OnChatCommandReceivedArgs(TwitchClient.OnChatCommandReceivedArgs args, TwitchClient client)
        {
            string command = args.Command.ToLower();
            if (command == "uptime" || command == "u" || command == "up")
            {
                Console.WriteLine("uptime check");
                TimeSpan uptime = await TwitchApi.GetUptime(args.Channel);
                if (uptime != new TimeSpan())
                {
                    client.SendMessage(new JoinedChannel(args.Channel), $"Time: {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s");
                }
                else
                {
                    client.SendMessage(new JoinedChannel(args.Channel), "he's offline i think? :)");
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
    }
}
