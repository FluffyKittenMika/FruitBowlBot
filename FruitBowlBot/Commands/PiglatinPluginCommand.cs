using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace JefBot.Commands
{
    internal class PiglatinPluginCommand : IPluginCommand
    {
        public string PluginName => "Piglatin";
        public string Command => "piglatin";
        public string Help => "!piglatin {string}";
        public IEnumerable<string> Aliases => new[] { "pig" };
        public bool Loaded { get; set; } = true;

        const string vowels = "AEIOUaeiou";

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res = Pig(message); });
            return res;
        }

        public string Pig(Message message)
        {
            return Piglatin(message.Arguments);
        }

        public static string Piglatin(List<string> pigify)
        {
            List<string> temp = new List<string>();
            foreach (var word in pigify)
            {
                char c = word[0];
                string restLet = word.Substring(1, word.Length - 1);
                temp.Add(vowels.Contains(c) ? word + "way" : restLet + c + "ay");
            }
            return string.Join(" ", temp);
        }
    }
}
