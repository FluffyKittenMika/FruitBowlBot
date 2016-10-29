using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using TwitchLib.TwitchClientClasses;
using System.Net;

namespace JefBot.Commands
{
    internal class RollPluginCommand : IPluginCommand
    {
        public string PluginName => "Roll";
        public string Command => "roll";
        public IEnumerable<string> Aliases => new[] { "r" };
        public bool Loaded { get; set; } = true;
        public bool OffWhileLive { get; set; } = true;

        //Non default definitions
        Random rng = new Random();

        public void Execute(ChatCommand command, TwitchClient client)
        {
            int DiceCount = 1;
            int SideCount = 6;
            int result = 0;
            int maxcount = 5000;

            if (command.ArgumentsAsList.Count > 0)
            {
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
                            result += rng.Next(SideCount) + 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            break;
                        }
                    }
                    client.SendMessage(command.ChatMessage.Channel, $"{command.ChatMessage.Username} rolled a {DiceCount}D{SideCount} and got {result}");
                }
            }
        }
    }
}
