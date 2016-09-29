using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;

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
            string command = args.Command.Command.ToLower();
            if (command == "uptime" || command == "u" || command == "up")
            {
                TimeSpan uptime = await TwitchApi.GetUptime(args.Command.ChatMessage.Channel);
                if (uptime != new TimeSpan())
                {
                   client.SendMessage(new TwitchLib.TwitchClientClasses.JoinedChannel(args.Command.ChatMessage.Channel), $"Time: {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s");
                }
                else
                {
                    client.SendMessage(new TwitchLib.TwitchClientClasses.JoinedChannel(args.Command.ChatMessage.Channel), "He's offline I think? :)");
                }
            }
        }

        public void OnConnectedArgs(TwitchClient.OnConnectedArgs args, TwitchClient client)
        {
        }

        public async void OnMessageReceivedArgs(TwitchClient.OnMessageReceivedArgs args, TwitchClient client)
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
