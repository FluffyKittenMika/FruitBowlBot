using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;
using Discord.WebSocket;
using System.Linq;

namespace JefBot.Commands
{
    internal class SpongebobPluginCommand : IPluginCommand
    {
        public string PluginName => "Spongebob";
        public string Command => "spongebob";
        public string Help => "!spongebob {string}";
        public IEnumerable<string> Aliases => new[] { "bob","sponge","mock" };
        public bool Loaded { get; set; } = true;

        Random rng = new Random();

        public string Action(Message message)
        {
            return Bob(message);
        }

        public string Bob(Message message)
        {
            if (message.MessageIsFromDiscord)
            {
                string temp = "";
                string input = String.Join(" ", message.Arguments);

                for (int i = 0; i < input.Length; i++)
                {
                    int r = rng.Next(1, 3);

                    if (i != 0)
                    {
                        if (input[i] == char.ToUpper(input[i]))
                            temp += char.ToLower(input[i]);
                        if (input[i] == char.ToLower(input[i]))
                            temp += char.ToUpper(input[i]);
                    }
                    else
                        temp += char.ToLower(input[i]);
                    
                    for (int n = 1; n <= r; n++)
                        if (i + n < input.Length)
                            temp += input[i + n];
                    i += r;
                }
                return temp;
            }
            return null;
        }
    }
}
