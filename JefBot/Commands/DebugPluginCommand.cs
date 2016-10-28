using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class DebugPluginCommand : IPluginCommand
    {
        public string PluginName => "Debug";
        public string Command => "debug";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;


        public async void Execute(ChatCommand command, TwitchClient client)
        {
            //totally ok to add yourself to debug :^)
            if (command.ChatMessage.Username == "mikaelssen" || command.ChatMessage.IsBroadcaster || command.ChatMessage.IsModerator)
            {
                TwitchLib.TwitchAPIClasses.Stream stream = await TwitchApi.GetTwitchStream(command.ChatMessage.Channel);
                client.SendMessage(command.ChatMessage.Channel, $"AvFPS:{stream.AverageFps} Delay:{stream.Delay} Game:{stream.Game} Viewers:{stream.Viewers} videoHeight:{stream.VideoHeight}");
            }


        }

    }
}
