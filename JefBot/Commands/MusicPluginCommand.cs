using Discord;
using System;
using System.Linq;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.Models.Client;
using Discord.Audio;

namespace JefBot.Commands
{
    internal class MusicPluginCommand : IPluginCommand
    {
        public string PluginName => "Music";
        public string Command => "meowtest";
        public string Help => "!p ";
        public IEnumerable<string> Aliases => new[] { "musicplay" };
        public bool Loaded { get; set; } = true;
        IAudioClient _vClient;

        public void Execute(ChatCommand command, TwitchClient client)
        {
        }

        public async void Discord(MessageEventArgs arg)
        {
            var args = arg.Message.Text.Split(' ').ToList().Skip(1).ToList();
            if (args.Count > 0)
            {
                if (args[0] == "summon")
                {
                    _vClient = await arg.Server.Client.GetService<AudioService>().Join(arg.User.VoiceChannel);
                }
            }
        }
    }
}
