using System;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class AirhornPluginCommand : IPluginCommand
    {
        public string PluginName => "Airhorn";
        public string Command => "airhorn";
        public IEnumerable<string> Aliases => new[] { "a", "air" };
        public bool Loaded { get; set; } = true;
        public bool OffWhileLive { get; set; } = false;
        
        private readonly Random _rand = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            if (!command.ChatMessage.Subscriber) return;

            var message = string.Empty;
            var count = _rand.Next(2, 5);

            if (command.ArgumentsAsList.Count == 1)
            {
                int parsed;
                if (int.TryParse(command.ArgumentsAsList[0], out parsed))
                {
                    count = Math.Max(1, Math.Min(5, parsed));
                }
            }

            for (var i = 0; i < count; i++)
            {
                message += "jspAirhorn ";
            }

            client.SendMessage(command.ChatMessage.Channel, message);

        }
    }
}
