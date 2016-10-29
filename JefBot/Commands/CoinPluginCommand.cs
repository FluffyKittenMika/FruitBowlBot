using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using TwitchLib.TwitchClientClasses;
using System.Net;

namespace JefBot.Commands
{
    internal class CoinPluginCommand : IPluginCommand
    {
        public string PluginName => "Coin";
        public string Command => "coin";
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
    }
}
