using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;

namespace JefBot
{
    public interface Plugin
    {
        string PluginName { get; }
        bool Loaded { get; }

        void OnMessageReceivedArgs(TwitchClient.OnMessageReceivedArgs args, TwitchClient client);
        void OnConnectedArgs(TwitchClient.OnConnectedArgs args, TwitchClient client);
        void RecivedResub(TwitchClient.OnReSubscriberArgs args, TwitchClient client);
        void OnNewSubscriberArgs(TwitchClient.OnNewSubscriberArgs args, TwitchClient client);
        void OnReSubscriberArgs(TwitchClient.OnReSubscriberArgs args, TwitchClient client);
        void OnChatCommandReceivedArgs(TwitchClient.OnChatCommandReceivedArgs args, TwitchClient client);
        void Shutdown();
    }
}
