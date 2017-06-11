using System;
using System.Collections.Generic;
using TwitchLib;
using System.Linq;
using Discord;
using TwitchLib.Models.Client;
using Discord.WebSocket;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Globalization;

namespace JefBot.Commands
{
    internal class GalaxyPluginCommand : IPluginCommand
    {
        public string PluginName => "Galaxy";
        public string Command => "galaxy";
        public string Help => "!galaxy {stars}  (discord only)";
        public IEnumerable<string> Aliases => new[] { "g" };
        public bool Loaded { get; set; } = true;
        public static HttpClient client = new HttpClient();

        List<IPluginCommand> plug = new List<IPluginCommand>();
        Random rng = new Random();

        public void Twitch(ChatCommand command, TwitchClient client)
        {
            //this is a discord only command. thanks
        }

        /// <summary>
        /// Returns a 250x250 Galaxy image
        /// </summary>
        /// <param name="stars">how many stars you want, max of 10k</param>
        /// <returns>Bitmap</returns>
        public Bitmap Galaxy(int stars = 1000, int width = 250, int height = 250)
        {
            //define the map
            Bitmap bmp = new Bitmap(width, height);//hardcoded size, don't want it to use too much power 62500 pixels is plenty
            //define the colour white
            System.Drawing.Color c = System.Drawing.Color.FromArgb(255, 255, 255);

            //make and fill image
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)))
            {
                gfx.FillRectangle(brush, 0, 0, 250, 250);//hardcoded size
            }


            //initialise the list of points
            List<Star> StarList = new List<Star>();

            //limit the amount to 10k
            if (stars > 10000)
            {
                stars = 10000;
            }

            //static things

            //amount of galaxy branches
            int arms = rng.Next(2,9);
            //offset (width of branches)
            double armoffsetmax = 0.5d;
            //prefered distance between the branches
            double armdistance = 2 * Math.PI / arms;
            //define the bend
            double armrotation = 5;
            //a bit of a random sprinkles to remove the lines
            double randomoffset = 0.02d;

            //first loop, where we generate everything
            for (int i = 0; i < stars; i++)
            {
                //distance from center
                double distance = rng.NextDouble();
                distance = Math.Pow(distance, 2);

                //angle of a circle
                double angle = rng.NextDouble() * 2 * Math.PI;

                //set the offset
                double armoffset = rng.NextDouble() * armoffsetmax;
                armoffset = armoffset - armoffsetmax / 2; //limits it to the lines
                armoffset = armoffset * (1 / distance); //spread em out a bit

                //sqare em up
                double sqaredarmoffset = Math.Pow(armoffset, 2);
                if (armoffset < 0)
                    sqaredarmoffset = sqaredarmoffset * -1;
                armoffset = sqaredarmoffset;

                //calculate rotation point
                double rotation = distance * armrotation;

                //manipulate it a bit
                angle = (int)(angle / armdistance) * armdistance + armoffset + rotation;

                //define x and y
                double starX = Math.Cos(angle) * (100 * distance);
                double starY = Math.Sin(angle) * (100 * distance);

                //add offset and center
                starX += (rng.NextDouble() * randomoffset) + (0.5 * 250);
                starY += (rng.NextDouble() * randomoffset) + (0.5 * 250);
                

                //add stars to the mass exodus list
                StarList.Add(new Star(starX,starY));
            }

            //probably end up merging the 2 loops
            foreach (var item in StarList)
            {
                bmp.SetPixel((int)item.X, (int)item.Y, c); //set the pixel cords to the correct colour
            }
            return bmp;
        }
        
        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {
            try
            {
                Bitmap galaxy;
                var args = arg.Content.Split(' ').ToList().Skip(1).ToList(); //this is probably so wrong (pro tip, it's verry bad)
                if (args.Count > 0) //if arguments is greater than nothing at all, then we go on to round 1
                {
                    Int32.TryParse(args[0], out int x);
                    galaxy = Galaxy(x);
                }
                else
                {
                    galaxy = Galaxy();//no arg then no barge
                }

                using (Stream stream = new MemoryStream())
                { //transform bmp to png in memory
                    galaxy.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    //galaxy.Save("a.png", System.Drawing.Imaging.ImageFormat.Png);
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    HttpContent co = new StringContent("fiskebolle");
                    form.Add(co, "k");
                    stream.Position = 0;
                    co = new StreamContent(stream);
                    co.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "d",
                        FileName = $"{rng.Next()}.png",
                        Size = stream.Length
                    };
                    form.Add(co);
                    var res = client.PostAsync("http://u.rubixy.com/", form);
                    arg.Channel.SendMessageAsync("<" + res.Result.Content.ReadAsStringAsync().Result + ">");
                }

                arg.DeleteAsync();

            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                Console.WriteLine(err.StackTrace);
            }

        }

        public class Star
        {

            public Star(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }

            public Star()
            {

            }
            public double X { get; set; }
            public double Y { get; set; }
        }

    }
}
