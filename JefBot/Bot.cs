using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        ConnectionCredentials credentials;
        TwitchChatClient ChatClient;
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

            }else
            {
                Console.Write("nope, no config file found, please craft one");
                Thread.Sleep(5000);
                Environment.Exit(0); // Closes the program if there's no setting, should just make it generate one, but as of now, don't delete the settings.
            }

            credentials = new ConnectionCredentials(ConnectionCredentials.ClientType.Chat, new TwitchIpAndPort(settings["channel"], true), settings["username"], settings["oauth"]);
            ChatClient = new TwitchChatClient(settings["channel"], credentials, '!');

            ChatClient.OnMessageReceived += new EventHandler<TwitchChatClient.OnMessageReceivedArgs>(RecivedMessage);
            ChatClient.OnCommandReceived += new EventHandler<TwitchChatClient.OnCommandReceivedArgs>(RecivedCommand);
            ChatClient.OnNewSubscriber += new EventHandler<TwitchChatClient.OnNewSubscriberArgs>(RecivedNewSub);
            ChatClient.OnReSubscriber += new EventHandler<TwitchChatClient.OnReSubscriberArgs>(RecivedResub);
            ChatClient.OnConnected += new EventHandler<TwitchChatClient.OnConnectedArgs>(Connected);

            ChatClient.Connect();
            Console.WriteLine("Bot init Complete");
        }

        private void Connected(object sender, TwitchChatClient.OnConnectedArgs e)
        {
            Console.WriteLine($@"Connected to {e.Channel} with name {e.Username}");
        }

        private void RecivedResub(object sender, TwitchChatClient.OnReSubscriberArgs e)
        {
            Console.WriteLine($@"{e.ReSubscriber.DisplayName} subbed for {e.ReSubscriber.Months} with the message '{e.ReSubscriber.ResubMessage}' :)");
            //ChatClient.SendMessage($"panicBasket {e.ReSubscriber.DisplayName} panicBasket ");
        }

        private void RecivedNewSub(object sender, TwitchChatClient.OnNewSubscriberArgs e)
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
        private void RecivedCommand(object sender, TwitchChatClient.OnCommandReceivedArgs e)
        {
            Console.WriteLine($@"Just got a {e.Command} Command. Doing it if it's real");

            string command = e.Command.ToLower();

            if (command == "quote" || command == "q")
            {
                Quote(e);
            }

            if (command == "help" || command == "h")
            {
                ChatClient.SendMessage("Just do !quote or !q and some text after it to send a quote in for review");
            }

            if (command == "uptime" || command == "up" || command == "u")
            {
                Uptime();
            }

            if (command == "modlist")
            {
                ChatClient.SendMessage("Rimworld Modlist: http://i.imgur.com/z6Mh76F.png");
            }
        }

        /// <summary>
        /// Gets the uptime with a wonky twitch lib thing, it gets it in jef timezone and converts it somethign weird, so i have to substract 9 hours out of it.
        /// Gotta update this lib soon and see if they fixed it.
        /// </summary>
        private async void Uptime()
        {
            try
            {
                Console.WriteLine("uptime check");
                TimeSpan uptime = await TwitchApi.GetUptime(settings["channel"]);
                ChatClient.SendMessage($"Time: {uptime.Hours - 9}h {uptime.Minutes}m {uptime.Seconds}s");

            }
            catch (Exception e)
            {
                ChatClient.SendMessage("he's offline i think? :)");
                Console.WriteLine(e.Message);
            }
                      //ChatClient.SendMessage(string.Format("uptime: {1} hours, {2} minutes, {3} seconds", uptime.Hours, uptime.Minutes, uptime.Seconds));
        }
        
        private void Quote(TwitchChatClient.OnCommandReceivedArgs e)
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
                ChatClient.SendMessage("Shit, tell mikaelssen: " + er.Message);
                Console.WriteLine(er.Message);
                Console.WriteLine(er.InnerException);
            }
            if (!quoted)
            {
                ChatClient.SendMessage("👌 Thanks!");
            }else
            {
                ChatClient.SendMessage("👌 please don't add \" to the quotes yourself :)");
            }
            
        }

        //Console.WriteLine($@"");

        private void RecivedMessage(object sender, TwitchChatClient.OnMessageReceivedArgs e)
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
                ChatClient.SendMessage(Console.ReadLine());
            }
        }
    }
}
