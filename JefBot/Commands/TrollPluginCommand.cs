using Discord;
using System;
using System.Linq;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class TrollPluginCommand : IPluginCommand
    {
        public string PluginName => "Troll";
        public string Command => "troll";
        public string Help => "!t ";
        public IEnumerable<string> Aliases => new[] { "t" };
        public bool Loaded { get; set; } = true;

        List<string> memes = new List<string>() { "6±2", "∑n⁻²", "451°", "≈10", "420", "3.14pie", "tails", "head", "0", "NaN", "a cookie", "13", "32", "69", "9001", "∞", "½", "x̄", "p̂", "7‰", "⨌", "∰", "√-1" };
        Random rng = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (!Bot.IsStreaming(command.ChatMessage.Channel))
            {
                client.SendMessage(command.ChatMessage.Channel, Trollit(command.ChatMessage.Username, command.ArgumentsAsList));
            }
          
        }

        public string Trollit(string username, List<string> args)
        {
            try
            {
                if (args.Count > 0)
                {
                    string dice = args.ToString().Trim(new Char[] { ' ' }).ToLower();

                    
                    string[] split = dice.ToLower().Split(new Char[] { 'd' });
                    string result = memes[rng.Next(memes.Count)];

                    return ($"{username} rolled a {dice[0]}D{dice[2]} and got {result}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "no";
        }


        public void Discord(MessageEventArgs arg)
        {
            arg.Channel.SendMessage(Trollit(arg.User.Name, arg.Message.Text.Split(' ').ToList()));
        }
    }
}
