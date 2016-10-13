using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class QuotePluginCommand : IPluginCommand
    {
        public string PluginName => "Quote";
        public string Command => "quote";
        public IEnumerable<string> Aliases => new[] { "q" };
        public bool Loaded { get; set; } = true;

        public void Execute(ChatCommand command, TwitchClient client)
        {
            //passive agressie anti double quote checker
            var quoted = command.ArgumentsAsString[0] == '"';

            using (var w = File.AppendText("quotes.txt"))
                w.Write($"\"{command.ArgumentsAsString}\"| {DateTime.Now} submitted by {command.ChatMessage.Username}" + Environment.NewLine);

            if (!quoted)
                client.SendMessage(command.ChatMessage.Channel, "👌 Thanks!");
            else
                client.SendMessage(command.ChatMessage.Channel, "👌 please don't add \" to the quotes yourself :)");

        }
    }
}
