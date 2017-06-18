using Discord;
using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.Models.API;
using TwitchLib.Models.API.Stream;
using TwitchLib.Models.Client;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace JefBot.Commands
{
    internal class DebugPluginCommand : IPluginCommand
    {
        public string PluginName => "Debug";
        public string Command => "debug";
        public string Help => "!debug to get a bunch of stream data";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;

        public string Debug(Message message)
        {
            if (Bot.IsStreaming(message.Channel))
            {
                try
                {
                    //totally ok to add yourself to debug :^)
                    if (message.Username == "mikaelssen" || message.IsModerator)
                    {
                        Stream stream = TwitchApi.Streams.GetStream(message.Channel);
                        return $"AvFPS:{stream.AverageFps} Delay:{stream.Delay} Game:{stream.Game} Viewers:{stream.Viewers} videoHeight:{stream.VideoHeight}";
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return e.Message;
                }

            }
            return null;
        }

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = Debug(message); });
            return res;
        }
    }
}
