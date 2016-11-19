using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Commands
{
    internal class ModListPluginCommand : IPluginCommand
    {
        public string PluginName => "Modlist";
        public string Command => "modlist";
        public string Help => "!mods {set/clear} {result}; Will print out modlist for the current playing stream game.";
        public IEnumerable<string> Aliases => new[] { "mods" };
        public Dictionary<string, string> ModLists = new Dictionary<string, string>();
        public bool Loaded { get; set; } = true;


        private string memoryPath = "./modList.txt";

        public ModListPluginCommand()
        {
            Load();
        }

        public async void Execute(ChatCommand command, TwitchClient client)
        {
            var channel = await TwitchApi.GetTwitchChannel(command.ChatMessage.Channel);
            var modKey = channel.Game.ToLower();
            if (command.ArgumentsAsList.Count >= 1)
            {
                if (
                    string.Equals(command.ArgumentsAsList[0], "set", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(command.ArgumentsAsList[0], "clear", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    if (command.ChatMessage.IsModerator || command.ChatMessage.IsBroadcaster)
                    {
                        if (
                            string.Equals(command.ArgumentsAsList[0], "set", StringComparison.OrdinalIgnoreCase) &&
                            command.ArgumentsAsList.Count >= 2
                            )
                        {
                            var message = string.Join(" ", command.ArgumentsAsList.Skip(1));

                            //stops a crash if there's already a key for the new list
                            if (ModLists.ContainsKey(modKey))
                                ModLists.Remove(modKey);

                            ModLists.Add(modKey, message);
                            Save();

                            client.SendMessage(command.ChatMessage.Channel, $"Modlist updated for {channel.Game}");
                        }

                        if (string.Equals(command.ArgumentsAsList[0], "clear", StringComparison.OrdinalIgnoreCase))
                        {
                            ModLists.Remove(modKey);
                            Save();
                            client.SendMessage(command.ChatMessage.Channel, $"Modlist cleared for {channel.Game}");
                        }
                    }
                    else
                    {
                        client.SendMessage(command.ChatMessage.Channel, "You're not a moderator");
                    }
                }
                else
                {
                    if (ModLists.ContainsKey(modKey))
                    {
                        client.SendMessage(command.ChatMessage.Channel, $"{modKey}: {ModLists[modKey]}");
                    }
                    else
                    {
                        client.SendMessage(command.ChatMessage.Channel, $"I don't have a modlist for {modKey}");
                    }
                }

            }
            else
            {
                if (ModLists.ContainsKey(modKey))
                {
                    client.SendMessage(command.ChatMessage.Channel, $"{channel.Game}: {ModLists[modKey]}");
                }
                else
                {
                    client.SendMessage(command.ChatMessage.Channel, $"I don't have a modlist for {channel.Game}");
                }
            }

        }

    

        private void Save()
        {
            File.Create(memoryPath).Close();

            using (StreamWriter w = new StreamWriter(memoryPath))
            {
                foreach (var cmd in ModLists)
                {
                    w.WriteLine($"{cmd.Key}!@!~!@!{cmd.Value}");
                }
            }
        }



        private void Load()
        {
            try
            {
                if (!File.Exists(memoryPath)) return;

                using (StreamReader r = new StreamReader(memoryPath))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        string[] ncmd = line.Split(new[] { "!@!~!@!" }, StringSplitOptions.None);

                        ModLists.Add(ncmd[0], ncmd[1]);

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"{ncmd[0]} --- {ncmd[1]}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void Discord(Message arg)
        {
            arg.Channel.SendMessage("Not implemented yet");
        }
    }
}
