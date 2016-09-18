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

            Credentials = new ConnectionCredentials(settings["username"], settings["oauth"]);
            ChatClient = new TwitchClient(Credentials, channel: settings["channel"], chatCommandIdentifier: '!', logging: Convert.ToBoolean(settings["debug"]));
           
            ChatClient.OnMessageReceived += new EventHandler<TwitchClient.OnMessageReceivedArgs>(RecivedMessage);
            ChatClient.OnChatCommandReceived += new EventHandler<TwitchClient.OnChatCommandReceivedArgs>(RecivedCommand);
            ChatClient.OnNewSubscriber += new EventHandler<TwitchClient.OnNewSubscriberArgs>(RecivedNewSub);
            ChatClient.OnReSubscriber += new EventHandler<TwitchClient.OnReSubscriberArgs>(RecivedResub);
            ChatClient.OnConnected += new EventHandler<TwitchClient.OnConnectedArgs>(Connected);
            
            ChatClient.Connect();
            Console.WriteLine("Loading Plugins");

            //automate this shit somehow later
            //input greatly appriciated
            plugins.Add(new Plugins.Quote.Main());
            plugins.Add(new Plugins.Uptime.Main());

            foreach (var plug in plugins)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (plug.Loaded)
                {
                    Console.WriteLine($"Loaded: {plug.PluginName}");
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Bot init Complete");
        }

        private void Connected(object sender, TwitchClient.OnConnectedArgs e)
        {
            foreach (var plug in plugins)
            {
                plug.OnConnectedArgs(e, ChatClient);
            }
            Console.WriteLine($@"Connected to {e.AutoJoinChannel} with name {e.Username}");
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
        /// The simplest command handler ever, please feel free to make something way better, or even make everything modulare
        /// It's a lot of extra work for little reward if you make it modular though, but would be pretty memevalue
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
                ChatClient.SendMessage(new JoinedChannel(e.Channel),"Just do !quote or !q and some text after it to send a quote in for review");
            }
            
            if (command == "modlist")
            {
                ChatClient.SendMessage(new JoinedChannel(e.Channel), "Rimworld Modlist: http://i.imgur.com/z6Mh76F.png SteamModList: http://tinyurl.com/hch8zob");
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

        //meh, i din't want brackets in this function, so i removed them all :)
        private void Quoteold(TwitchClient.OnChatCommandReceivedArgs e)
        {
            //passive agressie anti double quote checker
            bool quoted = false;
            if (e.ArgumentsAsString[0] == '"')
                quoted = true;

            using (StreamWriter w = File.AppendText("quotes.txt"))
                w.Write($"\"{e.ArgumentsAsString}\"| {DateTime.Now} submitted by {e.ChatMessage.DisplayName}" + Environment.NewLine);

            if (!quoted)
                ChatClient.SendMessage(new JoinedChannel(e.Channel), "👌 Thanks!");
            else
                ChatClient.SendMessage(new JoinedChannel(e.Channel), "👌 please don't add \" to the quotes yourself :)");
        }

        public void run()
        {
            while (true)
            {
                //anything we type into the console is broadcasted to every channel we're inn. so don't be chatty :^)
                string msg = Console.ReadLine();
                foreach (var channel in ChatClient.JoinedChannels)
                {
                    ChatClient.SendMessage(channel, msg);
                }
            }
        }
    }
}
