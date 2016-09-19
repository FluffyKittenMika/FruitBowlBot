using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TwitchLib.TwitchClientClasses;
using TwitchLib;
using System.Threading;

namespace JefBot
{
    
    class Bot
    {
        ConnectionCredentials Credentials;
        TwitchClient ChatClient;
        Dictionary<string, string> settings = new Dictionary<string, string>();
        List<Plugin> plugins = new List<Plugin>();
        

        //constructor
        public Bot()
        {
            init();
        }

        //Start shit up m8
        private void init()
        {
            #region config loading
            var settingsFile = @"./Settings/Settings.txt";
            if (File.Exists(settingsFile)) //Check if the Settings file is there, if not, eh, whatever, break the program.
            {
                using (StreamReader r = new StreamReader(settingsFile))
                {
                    string line; //keep line in memory outside the while loop, like the queen of Englan is remembered outside of Canada
                    while ((line = r.ReadLine()) != null)
                    {
                        if (line[0] != '#')//skip comments
                        {
                            string[] split = line.Split('='); //Split the non comment lines at the equal signs
                            settings.Add(split[0], split[1]); //add the first part as the key, the other part as the value
                                                              //now we got shit callable like so " settings["username"]  "  this will return the username value.
                        }
                    }
                }

            }else
            {
                Console.Write("nope, no config file found, please craft one");
                Thread.Sleep(5000);
                Environment.Exit(0); // Closes the program if there's no setting, should just make it generate one, but as of now, don't delete the settings.
            }
            #endregion

            #region ChatClient init
            Credentials = new ConnectionCredentials(settings["username"], settings["oauth"]);
            ChatClient = new TwitchClient(Credentials, channel: settings["channel"], chatCommandIdentifier: '!', logging: Convert.ToBoolean(settings["debug"]));
           
            ChatClient.OnMessageReceived += new EventHandler<TwitchClient.OnMessageReceivedArgs>(RecivedMessage);
            ChatClient.OnChatCommandReceived += new EventHandler<TwitchClient.OnChatCommandReceivedArgs>(RecivedCommand);
            ChatClient.OnNewSubscriber += new EventHandler<TwitchClient.OnNewSubscriberArgs>(RecivedNewSub);
            ChatClient.OnReSubscriber += new EventHandler<TwitchClient.OnReSubscriberArgs>(RecivedResub);
            ChatClient.OnConnected += new EventHandler<TwitchClient.OnConnectedArgs>(Connected);
            
            ChatClient.Connect();
            #endregion

            #region plugins
            Console.WriteLine("Loading Plugins");

            //automate this shit somehow later
            //input greatly appriciated
            plugins.Add(new Plugins.Quote.Main());
            plugins.Add(new Plugins.Uptime.Main());
            plugins.Add(new Plugins.Modlist.Main());
            plugins.Add(new Plugins.CustomCommands.Main());

            foreach (var plug in plugins)
            {
                if (plug.Loaded)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Loaded: {plug.PluginName}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"NOT Loaded: {plug.PluginName}");
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            #endregion
            Console.WriteLine("Bot init Complete");
        }

        private void Connected(object sender, TwitchClient.OnConnectedArgs e)
        {
            foreach (var plug in plugins)
            {
                plug.OnConnectedArgs(e, ChatClient);
            }
        }

        private void RecivedResub(object sender, TwitchClient.OnReSubscriberArgs e)
        {
            foreach (var plug in plugins)
            {
                plug.RecivedResub(e, ChatClient);
            }
            Console.WriteLine($@"{e.ReSubscriber.DisplayName} subbed for {e.ReSubscriber.Months} with the message '{e.ReSubscriber.ResubMessage}' :)");
        }

        private void RecivedNewSub(object sender, TwitchClient.OnNewSubscriberArgs e)
        {
            foreach (var plug in plugins)
            {
                plug.OnNewSubscriberArgs(e, ChatClient);
            }
            Console.WriteLine($@"{e.Subscriber.Name} Just subbed! What a bro!' :)");
        }

      
        /// <summary>
        /// quite~
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecivedCommand(object sender, TwitchClient.OnChatCommandReceivedArgs e)
        {
            foreach (var plug in plugins)
            {
                plug.OnChatCommandReceivedArgs(e, ChatClient);
            }

            string command = e.Command.ToLower();
           
            if (command == "help" || command == "h")
            {
                ChatClient.SendWhisper(e.ChatMessage.Username, "!q {quote} witout brackets, !help for this message, !uptime for uptime, !modlist for a modlist when relevant");
                if (e.ChatMessage.IsModerator || e.ChatMessage.Username == "mikaelssen")
                {
                    ChatClient.SendWhisper(e.ChatMessage.Username, "hey mod!, you can also do !set modlist {text} without brackets, to change that, or !command add/remove {command} {result} for custom commands (don't do !command add uptime, it's untested help)");
                }
               //ChatClient.SendMessage(new JoinedChannel(e.Channel), "Just do !quote or !q and some text after it to send a quote in for review");
            }
        }

        private void RecivedMessage(object sender, TwitchClient.OnMessageReceivedArgs e)
        {
            foreach (var plug in plugins)
            {
                plug.OnMessageReceivedArgs(e, ChatClient);
            }
            Console.WriteLine($"{e.ChatMessage.DisplayName} : {e.ChatMessage.Message}");
        }

        public void run()
        {
            while (true)
            {
                //anything we type into the console is broadcasted to every channel we're inn. so don't be chatty :^)
                string msg = Console.ReadLine();
                if (msg == "quit" || msg == "stop")
                {
                    foreach (var plug in plugins)
                    {
                        plug.Shutdown();
                    }
                    Environment.Exit(0);
                }else
                {
                    foreach (var channel in ChatClient.JoinedChannels)
                    {
                        ChatClient.SendMessage(channel, msg);
                    }
                }
              
            }
        }
    }
}
