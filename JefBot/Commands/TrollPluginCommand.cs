using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class TrollPluginCommand : IPluginCommand
    {
        public string PluginName => "Troll";
        public string Command => "troll";
        public string Help => "!t ";
        public IEnumerable<string> Aliases => new[] { "t" };
        public bool Loaded { get; set; } = true;

        List<string> memes = new List<string>() { "6±2", "∑n⁻²", "451°", "≈10", "420","3.14pie", "tails", "head", "0","NaN","a cookie", "13","32","69", "9001", "∞", "½", "x̄","p̂","7‰","⨌", "∰", "√-1"};
        Random rng = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            try
            {
                if (command.ArgumentsAsList.Count > 0)
                {
                    string dice = command.ArgumentsAsString.Trim(new Char[] { ' ' });

                    string[] split = dice.ToLower().Split(new Char[] { 'd' });
                    string result = memes[rng.Next(memes.Count)];
                    
                    client.SendMessage(command.ChatMessage.Channel, $"{command.ChatMessage.Username} rolled a {dice[0]}D{dice[2]} and got {result}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Discord(Message arg)
        {
            arg.Channel.SendMessage("Not implemented yet");
        }
    }
}
