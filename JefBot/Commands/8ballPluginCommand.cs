using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;
using Discord.WebSocket;

namespace JefBot.Commands
{
    internal class EightBallPluginCommand : IPluginCommand
    {
        public string PluginName => "8ball";
        public string Command => "8ball";
        public string Help => "!8ball to get wise";
        public IEnumerable<string> Aliases => new[] { "8" };
        public bool Loaded { get; set; } = true;

        List<string> responses = new List<string>
        {
            "As i see it, yes",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don't count on it",
            "It is certain",
            "It is decidedly so",
            "Most likely",
            "My reply is no",
            "My sources say no",
            "Outlook good",
            "Outlook not so good",
            "Reply hazy, try again",
            "Signs point to yes",
            "Very doubtful",
            "Without a doubt",
            "Yes",
            "Yes, definitely",
            "You may rely on it",
            "Only Greb knows",
            "Ask again, and Greb might know the answer you seek",
            "Donate $1 to Arkentosh, and he will reveal your answer",
            "Feel the wind, and you will know the truth",
            "Definitely",
            "Ask Selenaut about his game and he will tell you the truth about yourself",
            "Only you, my friend, knows this, within your heart",
            ":blep:",
            "bepis"

        };

        Random rng = new Random();
        public string Action(Message message)
        {
            return EightBall(message.RawMessage);
        }

        public string EightBall(string args)
        {
            string msg = "Please type a question!";
            if (args != "")
                msg = responses[rng.Next(responses.Count)];
            return msg;
        }
        
    }
}
