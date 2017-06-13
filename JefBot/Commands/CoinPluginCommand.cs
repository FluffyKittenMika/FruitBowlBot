using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;
using Discord.WebSocket;

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
        
        public string Action(Message message)
        {
            if (rng.Next(1000) > 1)
            {
                var result = rng.Next(0, 2) == 1 ? "heads" : "tails";
                return $"{message.Username} flipped a coin, it was {result}";
            }
            else
                return $"{message.Username} flipped a coin, it landed on it's side...";
        }
    }
}
