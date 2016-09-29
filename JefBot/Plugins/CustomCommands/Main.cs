using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot.Plugins.CustomCommands
{
    class Main : Plugin
    {
        Dictionary<string, string> Commands = new Dictionary<string, string>();

        public bool Loaded
        {
            get
            {
                try
                {
                    using (StreamReader r = new StreamReader("./Plugins/CustomCommands/Memory.txt")) 
                    {
                        string line;
                        while ((line = r.ReadLine()) != null)
                        {
                            string[] ncmd = line.Split(new string[] { "!@!~!@!" }, StringSplitOptions.None);

                            Commands.Add(ncmd[0], ncmd[1]);

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{ncmd[0]} --- {ncmd[1]}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    return false;
                }
            }
        }

        public string PluginName
        {
            get
            {
                return "Custom Commands";
            }
        }

        public void OnChatCommandReceivedArgs(TwitchClient.OnChatCommandReceivedArgs args, TwitchClient client)
        {
            string command = args.Command.Command.ToLower();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(command);
            Console.ForegroundColor = ConsoleColor.White;
            if (command == "command")
            {
                if (args.Command.ArgumentsAsList[0] == "list")
                {
                    string commands = "";
                    foreach (var item in Commands)
                    {
                        commands += " " + item;
                    }
                    client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel), $"Commands: {commands}");
                } else
                if (args.Command.ArgumentsAsList[0] == "add" && args.Command.ChatMessage.IsModerator)
                {
                    Console.WriteLine("Adding");
                    string newcommand = args.Command.ArgumentsAsList[1];
                    string newcommandresult = "";
                    
                    for (int i = 2; i < args.Command.ArgumentsAsList.Count; i++)
                    {
                        newcommandresult += args.Command.ArgumentsAsList[i] + " ";
                    }

                    Console.WriteLine("Adding " + newcommand + newcommandresult);
                    Commands.Add(newcommand, newcommandresult);
                    client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel), $"Command {newcommand} has been added");
                }else 
                if (args.Command.ArgumentsAsList[0] == "remove" && args.Command.ChatMessage.IsModerator)
                {
                    Console.WriteLine("Removing");
                    if (Commands.ContainsKey(command) == true)
                    {
                        Console.WriteLine("Removing " + command);
                        Commands.Remove(command);
                    }
                }
                else
                {
                    client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel), $"You're not a moderator {args.Command.ChatMessage.Username} :)");
                }
            }

            if (Commands.ContainsKey(command) == true)
            {
                client.SendMessage(new JoinedChannel(args.Command.ChatMessage.Channel),Commands[command]);
            }
            
        }

        public void OnConnectedArgs(TwitchClient.OnConnectedArgs args, TwitchClient client)
        {
        }

        public void OnMessageReceivedArgs(TwitchClient.OnMessageReceivedArgs args, TwitchClient client)
        {
        }

        public void OnNewSubscriberArgs(TwitchClient.OnNewSubscriberArgs args, TwitchClient client)
        {
        }

        public void OnReSubscriberArgs(TwitchClient.OnReSubscriberArgs args, TwitchClient client)
        {
        }

        public void RecivedResub(TwitchClient.OnReSubscriberArgs args, TwitchClient client)
        {
        }

        public void Shutdown()
        {
            string path = "./Plugins/CustomCommands/Memory.txt";
            File.Create(path).Close();
            
            using (StreamWriter w = new StreamWriter(path))
            {
                foreach (var cmd in Commands)
                {
                    w.WriteLine($"{cmd.Key}!@!~!@!{cmd.Value}");
                }
            }
        }
    }
}
