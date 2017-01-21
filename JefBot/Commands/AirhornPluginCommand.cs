using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class AirhornPluginCommand : IPluginCommand
    {
        public string PluginName => "Airhorn";
        public string Command => "airhorn";
        public string Help => "!a [1-10] to airhorn [] is optional";
        public IEnumerable<string> Aliases => new[] { "a", "air" };
        public bool Loaded { get; set; } = true;

        public enum chattype { Twitch, Discord };
        private readonly Random _rand = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (!Bot.IsStreaming(command.ChatMessage.Channel))
                client.SendMessage(command.ChatMessage.Channel, air(command.ArgumentsAsList,chattype.Twitch));
        }


        public string air(List<string> args, chattype type)
        {
            var message = string.Empty;
            var count = _rand.Next(2, 5);

            if (args.Count == 1)
            {
                int parsed;
                if (int.TryParse(args[0], out parsed))
                {
                    count = Math.Max(1, Math.Min(10, parsed));
                }
            }

            for (var i = 0; i < count; i++)
            {
                if (type == chattype.Twitch)
                {
                    message += "jspAirhorn ";
                }
                if (type == chattype.Discord)
                {
                    message += ":duck: ";
                }
            }

            return message;
        }

        public void Discord(MessageEventArgs arg, DiscordClient client)
        {

            var args = arg.Message.Text.Split(' ').ToList().Skip(1).ToList(); //this is probably so wrong
            arg.Channel.SendMessage(air(args, chattype.Discord));

        }

    }
}
