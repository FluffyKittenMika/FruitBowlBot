using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using Discord;
using TwitchLib.Models.Client;

namespace JefBot.Commands
{
    internal class RollPluginCommand : IPluginCommand
    {
        public string PluginName => "Roll";
        public string Command => "roll";
        public string Help => "!r {1d6} to roll a 1d6";
        public IEnumerable<string> Aliases => new[] { "r" };
        public bool Loaded { get; set; } = true;

        //Non default definitions
        Random rng = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            int DiceCount = 1;
            int SideCount = 6;
            int result = 0;
            int maxcount = 9001;

            bool minmax = false;
            int minRoll = Int32.MaxValue;
            int maxRoll = 0;

            if (command.ArgumentsAsList.Count > 0)
            {
                string[] args = command.ArgumentsAsString.Split(new Char[] { ' ' });
                foreach (string arg in args)
                {
                    if (arg.Equals("minmax", StringComparison.OrdinalIgnoreCase))
                    {
                        minmax = true;
                        break;
                    }
                }

                string dice = command.ArgumentsAsString.Trim(new Char[] { ' ' });

                string[] split = dice.ToLower().Split(new Char[] { 'd' });
                if (split.Length == 2)
                {
                    try
                    {
                        DiceCount = Convert.ToInt32(split[0]);
                    }
                    catch (Exception e) { Console.WriteLine(e.Message); }
                    try
                    {
                        SideCount = Convert.ToInt32(split[1]);
                    }
                    catch (Exception e) { Console.WriteLine(e.Message); }

                    DiceCount = Math.Max(Math.Min(DiceCount, maxcount), 1);
                    SideCount = Math.Max(Math.Min(SideCount, maxcount), 1);

                    for (int i = 0; i < DiceCount; i++)
                    {
                        try
                        {
                            int rollValue = rng.Next(SideCount) + 1;

                            minRoll = Math.Min(minRoll, rollValue);
                            maxRoll = Math.Max(maxRoll, rollValue);

                            result += rollValue;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            break;
                        }
                    }

                    string minmaxInfo = "";
                    if (minmax)
                    {
                        minmaxInfo = $" (lowest: {minRoll}, highest: {maxRoll})";
                    }

                    client.SendMessage(command.ChatMessage.Channel, $"{command.ChatMessage.Username} rolled a {DiceCount}D{SideCount} and got {result}{minmaxInfo}");
                }
            }
        }
        public void Discord(MessageEventArgs arg)
        {
            arg.Channel.SendMessage("Not implemented yet");
        }
    }
}
