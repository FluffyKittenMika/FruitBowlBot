using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib;
using Discord;
using TwitchLib.Models.Client;
using RogueSharp.DiceNotation;
using System.Linq;
using Discord.WebSocket;

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


        //TODO make this into a better more recursive thing, so one can roll multiple dice in one message, like use a foreach arg loop and check every word i guess.
        //just make it a whole lot better
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
        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {
            try
            {
                var args = arg.Content.Split(' ').ToList().Skip(1).ToList();
                var result = Dice.Roll(string.Join("", args.ToArray()));
                arg.Channel.SendMessageAsync($"{arg.Author.Username} got {result}");

                //var sent = arg.Channel.SendMessage($"{arg.User.Name} got {result}").Result; //comment away the line above if this one is active
                //deletes the message and result after sending
                //Message[] msg = { arg.Message, sent };
                //System.Threading.Thread.Sleep(5000); //do not use, pauses the whole bot, as this is not async :D
                //arg.Channel.DeleteMessages(msg); //bye bye msgs!
            }
            catch (Exception e)
            {
                arg.Channel.SendMessageAsync($"{e.Message}");
            }

        }





    }

    
}
