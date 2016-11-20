using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class CoinPluginCommand : IPluginCommand
    {
        public string PluginName => "Coin";
        public string Command => "coin";
        public string Help => "!c to flip a coin";
        public IEnumerable<string> Aliases => new[] { "c", "flip" };
        public bool Loaded { get; set; } = true;

        Random rng = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (rng.Next(1000) > 1)
            {
                var result = rng.Next(0, 2) == 1 ? "heads" : "tails";
                
                client.SendMessage(command.ChatMessage.Channel,
                    $"{command.ChatMessage.Username} flipped a coin, it was {result}");
            }
            else
            {
                client.SendMessage(command.ChatMessage.Channel,
                    $"{command.ChatMessage.Username} flipped a coin, it landed on it's side...");
            }

        }

        public void Discord(MessageEventArgs arg)
        {
            arg.Channel.SendMessage("Not implemented yet");
        }
    }
}
