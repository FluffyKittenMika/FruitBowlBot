using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class NeuralPluginCommand : IPluginCommand
    {
        public string PluginName => "Neural Network Grid-LSTM";
        public string Command => "neural";
        public string Help => "!neural to do neural stuff";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;

        Random rng = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            //  ¯\_(ツ)_/¯
        }

        public void Discord(MessageEventArgs arg, DiscordClient client)
        {
            arg.Channel.SendMessage("Not implemented yet");
        }
    }
}
