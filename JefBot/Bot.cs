using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TwitchLib.TwitchClientClasses;
using TwitchLib.Services;
using TwitchLib;
using TwitchLib.TwitchAPIClasses;
using TwitchLib.Exceptions;
using System.Threading;
using System.Net.Mail;

namespace JefBot
{

    /// <summary>
    /// Welcome to jefbot2000000000
    /// I'm your guide and butter on your toast.
    /// Please get the required refrences.
    /// Newtonsoft.json
    /// StarkSoftProxy //it's used for the Twitchapi or email. one of those
    /// TwitchLib
    /// log4net //probably not that needed, but it's in my list here so it might be needed
    /// Meebey.Smartirc4net
    /// 
    /// All of theese can be grabbed from the nugget package manager
    /// </summary>
    class Bot
    {
        ConnectionCredentials Credentials;
        TwitchClient ChatClient;
        Dictionary<string, string> settings = new Dictionary<string, string>();

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

                StreamReader file = new StreamReader(settingsFile); //Read the file, duh :)
                string line; //keep line in memory outside the while loop, like the queen of Englan is remembered outside of Canada
                while ((line = file.ReadLine()) != null)
                {
                    if (line[0] != '#')//skip comments
                    {
                        string[] split = line.Split('='); //Split the non comment lines at the equal signs
                        settings.Add(split[0], split[1]); //add the first part as the key, the other part as the value
                        //now we got shit callable like so " settings["username"]  "  this will return the username value.
                    }
                }
                file.Close();

            }else
            {
                Console.Write("nope, no config file found, please craft one");
                Thread.Sleep(5000);
                Environment.Exit(0); // Closes the program if there's no setting, should just make it generate one, but as of now, don't delete the settings.
            }
            foreach (var item in settings)
            {
                Console.WriteLine(item);
            }
            Credentials = new ConnectionCredentials(settings["username"], settings["oauth"]);
            ChatClient = new TwitchClient(Credentials, channel: settings["channel"], chatCommandIdentifier: '!', logging: Convert.ToBoolean(settings["debug"]));
           
            ChatClient.OnMessageReceived += new EventHandler<TwitchClient.OnMessageReceivedArgs>(RecivedMessage);
            ChatClient.OnChatCommandReceived += new EventHandler<TwitchClient.OnChatCommandReceivedArgs>(RecivedCommand);
            ChatClient.OnNewSubscriber += new EventHandler<TwitchClient.OnNewSubscriberArgs>(RecivedNewSub);
            ChatClient.OnReSubscriber += new EventHandler<TwitchClient.OnReSubscriberArgs>(RecivedResub);
            ChatClient.OnConnected += new EventHandler<TwitchClient.OnConnectedArgs>(Connected);

            ChatClient.Connect();
            Console.WriteLine("Bot init Complete");
        }

        private void Connected(object sender, TwitchClient.OnConnectedArgs e)
        {
            Console.WriteLine($@"Connected to {e.AutoJoinChannel} with name {e.Username}");
        }

        private void RecivedResub(object sender, TwitchClient.OnReSubscriberArgs e)
        {
            Console.WriteLine($@"{e.ReSubscriber.DisplayName} subbed for {e.ReSubscriber.Months} with the message '{e.ReSubscriber.ResubMessage}' :)");
            //ChatClient.SendMessage($"panicBasket {e.ReSubscriber.DisplayName} panicBasket ");
        }

        private void RecivedNewSub(object sender, TwitchClient.OnNewSubscriberArgs e)
        {
            Console.WriteLine($@"{e.Subscriber.Name} Just subbed! What a bro!' :)");
            //ChatClient.SendMessage("panicBasket " + e.Subscriber.Name + " panicBasket");
        }

        /// <summary>
        /// The simplest command handler ever, please feel free to make something way better, or even make everything modulare
        /// It's a lot of extra work for little reward if you make it modular though, but would be pretty memevalue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecivedCommand(object sender, TwitchClient.OnChatCommandReceivedArgs e)
        {
            Console.WriteLine($@"Just got a {e.Command} Command. Doing it if it's real");

            string command = e.Command.ToLower();

            if (command == "quote" || command == "q")
            {
                Quote(e);
            }

            if (command == "help" || command == "h")
            {
                ChatClient.SendMessage(new JoinedChannel(e.Channel),"Just do !quote or !q and some text after it to send a quote in for review");
            }

            if (command == "uptime" || command == "up" || command == "u")
            {
                Uptime(new JoinedChannel(e.Channel));
            }

            if (command == "modlist")
            {
                ChatClient.SendMessage(new JoinedChannel(e.Channel), "Rimworld Modlist: http://i.imgur.com/z6Mh76F.png SteamModList: http://tinyurl.com/hch8zob");
            }
        }

        /// <summary>
        /// Gets the uptime with a wonky twitch lib thing, it gets it in jef timezone and converts it somethign weird, so i have to substract 9 hours out of it.
        /// Gotta update this lib soon and see if they fixed it.
        /// </summary>
        private async void Uptime(JoinedChannel channel)
        {
            try
            {
                Console.WriteLine("uptime check");
                TimeSpan uptime = await TwitchApi.GetUptime(channel.Channel);
                ChatClient.SendMessage(channel, $"Time: {uptime.Hours - 9}h {uptime.Minutes}m {uptime.Seconds}s");

            }
            catch (Exception e)
            {
                ChatClient.SendMessage(channel, "he's offline i think? :)");
                Console.WriteLine(e.Message);
            }
                      //ChatClient.SendMessage(string.Format("uptime: {1} hours, {2} minutes, {3} seconds", uptime.Hours, uptime.Minutes, uptime.Seconds));
        }
        
        private void Quote(TwitchClient.OnChatCommandReceivedArgs e)
        {
            bool quoted = false;
            try
            {
                try
                {
                    if (e.ArgumentsAsString[0] == '"')
                    {
                        quoted = true;
                    }
                }
                catch (Exception meh)
                {
                    //fuck it, quote is just empty
                }
                
                MailMessage mail = new MailMessage();
                SmtpClient smtpserver = new SmtpClient(settings["serveraddress"]);
                mail.From = new MailAddress(settings["mailfrom"]);
                mail.To.Add(settings["mailto"]);
                mail.Body = $"\"{e.ArgumentsAsString}\"| {System.DateTime.Now} submitted by {e.ChatMessage.DisplayName}";
                mail.Subject = "Quote for review " + e.Channel;
                smtpserver.Port = 25;
              
                smtpserver.Credentials = new System.Net.NetworkCredential(settings["mailusername"], settings["mailpassword"]);
                smtpserver.EnableSsl = Convert.ToBoolean(settings["useSSL"]); 
                smtpserver.Send(mail);
                Console.WriteLine("Just sent this mail: " + mail.Body);
                
            }
            catch (Exception er)
            {
                ChatClient.SendMessage(new JoinedChannel(e.Channel), "Shit, tell mikaelssen: " + er.Message);
                Console.WriteLine(er.Message);
                Console.WriteLine(er.InnerException);
            }
            if (!quoted)
            {
                ChatClient.SendMessage(new JoinedChannel(e.Channel),"👌 Thanks!");
            }else
            {
                ChatClient.SendMessage(new JoinedChannel(e.Channel), "👌 please don't add \" to the quotes yourself :)");
            }
            
        }

        //Console.WriteLine($@"");

        private void RecivedMessage(object sender, TwitchClient.OnMessageReceivedArgs e)
        {
            Console.WriteLine($"{e.ChatMessage.DisplayName} : {e.ChatMessage.Message}");
           // Uptime();
            //string msg = $"{e.ChatMessage.DisplayName}:  \"{e.ChatMessage.Message}\"| {System.DateTime.Now} ";
            //email("mikael@rubixy.com", msg,e.ChatMessage.Channel);
        }

        public void run()
        {
            while (true)
            {
                ChatClient.SendMessage(ChatClient.JoinedChannels.First(), Console.ReadLine());
            }
        }
    }
}
