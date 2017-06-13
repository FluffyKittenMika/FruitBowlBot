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
        public string Help => "!galaxy {stars} {dimension} {frames}";
        public IEnumerable<string> Aliases => new[] { "g" };
        public bool Loaded { get; set; } = true;
        public static HttpClient client = new HttpClient();

        List<IPluginCommand> plug = new List<IPluginCommand>();
        public static Random rng = new Random();

        public void Twitch(ChatCommand command, TwitchClient client)
        {
            client.SendMessage(command.ChatMessage.Channel,GetLink(command.ArgumentsAsList));
        }
        
        public string GetLink(List<string> args)
        {
            try
            {
                int stars = 2500;
                int dimension = 500;
                int frames = 1;

                if (args.ElementAtOrDefault(0) != null) //check if position 0 of array is set for stars
                    Int32.TryParse(args[0], out stars);
                if (args.ElementAtOrDefault(1) != null) //dimension
                    Int32.TryParse(args[1], out dimension);

                if (dimension > 2000)
                    dimension = 2000;
                if (dimension < 250)
                    dimension = 250;

                if (args.ElementAtOrDefault(2) != null)
                    Int32.TryParse(args[2], out frames);
                if (frames > 60)
                    frames = 60;
                if (frames < 1)
                    frames = 1;

                return MakeGif(Galaxy(stars, dimension, frames));

            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                Console.WriteLine(err.StackTrace);
                return err.Message;
            }
        }

        public void Discord(SocketMessage arg, DiscordSocketClient discordClient)
        {
            var args = arg.Content.Split(' ').ToList().Skip(1).ToList(); //skips the !g
            arg.Channel.SendMessageAsync(GetLink(args));
        }

        /// <summary>
        /// Compiles a list of bitmaps into a gif, and uploads it automatically
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <returns></returns>
        public string MakeGif(List<Bitmap> bitmaps)
        {
            Stream stream = new MemoryStream();
            System.Drawing.Image image;
            var gif = File.OpenWrite("gif.gif");
            var encoder = new BumpKit.GifEncoder(gif) { FrameDelay = new TimeSpan(1) };

            for (int i = 0; i < bitmaps.Count; i++)
            {
                //move bitmap to memory stream
                stream.Position = 0;
                bitmaps[i].Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                image = System.Drawing.Image.FromStream(stream);
                encoder.AddFrame(image);
            }
            stream.Position = 0;
            gif.Close();
            stream.Close();

            Stream strm = new FileStream("gif.gif", FileMode.Open);
            return UploadImage.Upload(strm, "gif");

        }

        /// <summary>
        /// Returns a list of galaxy images
        /// </summary>
        /// <param name="stars">Number of stars</param>
        /// <param name="Dimension">Size of the image</param>
        /// <param name="TotalFrames">Number of frames</param>
        /// <returns>List of images</returns>
        public static List<Bitmap> Galaxy(int stars = 1000, int Dimension = 250, int TotalFrames = 1)
        {
            List<Bitmap> OutList = new List<Bitmap>();
            //define the map
            Bitmap Base = new Bitmap(Dimension, Dimension);//hardcoded size, don't want it to use too much power 62500 pixels is plenty
            //define the colour white
            System.Drawing.Color c = System.Drawing.Color.FromArgb(255, 255, 255);

            //make and fill image
            using (Graphics gfx = Graphics.FromImage(Base))
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)))
            {
                gfx.FillRectangle(brush, 0, 0, Dimension, Dimension);//hardcoded size
            }


            //initialise the list of points
            List<PolarStar> StarList = new List<PolarStar>();

            if (stars > 10000)
            {
                stars = 10000;
            }
            if (stars < 100)
            {
                stars = 100;
            }

            //static things

            //amount of galaxy branches
            int arms = rng.Next(1, 10);
            //offset (width of branches)
            double armoffsetmax = 0.8d;
            //prefered distance between the branches
            double armdistance = 2 * Math.PI / arms;
            //define the bend
            double armrotation = rng.Next(3,9);
            //a bit of a random sprinkles to remove the lines
            //double randomoffset = 0.02d;

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

                //add stars to the mass exodus list

                StarList.Add(new PolarStar(distance * Dimension * 0.5f, angle));

                //maybe not add to list if a star is already there?
            }

            //probably end up merging the 2 loops
            for (Int32 Index = 0; Index < TotalFrames; Index++)
            {
                Bitmap Frame = new Bitmap(Base);
                foreach (var item in StarList)
                {
                    Int32 RNG = rng.Next(10) - 5;
                    item.RotateDegrees(RNG - (360 / TotalFrames));
                    CartPoint Point = new CartPoint(item);
                    Point.Translate((int)(0.5f * (Dimension-1)), (int)(0.5f * (Dimension-1)));
                    try
                    {
                        Frame.SetPixel((int)Point.X, (int)Point.Y, c);
                    }
                    catch (Exception e)
                    {
                    }
                    item.RotateDegrees(0 - RNG);
                }
                OutList.Add(Frame);
            }
            return OutList;
        }



        public class PolarStar
        {

            public PolarStar(double Distance, double AngleR)
            {
                this.Distance = Distance;
                this.AngleD = AngleR / Math.PI * 180;
            }

            public PolarStar()
            {

            }
            public void RotateDegrees(double Degrees)
            {
                AngleD += Degrees;
            }
            public double Distance { get; set; }
            public double AngleD { get; set; }
        }

        public class CartPoint
        {
            public CartPoint(PolarStar Star)
            {
                X = Math.Cos(Star.AngleD / 180 * Math.PI) * Star.Distance;
                Y = Math.Sin(Star.AngleD / 180 * Math.PI) * Star.Distance;
            }
            public void Translate(int NewCenterX, int NewCenterY)
            {
                X += NewCenterX;
                Y += NewCenterY;
            }
            public double X { get; set; }
            public double Y { get; set; }
        }



        
        public static class UploadImage
        {
            static Random rng = new Random();
            /// <summary>
            /// Upload a stream to U.Rubixy.com
            /// </summary>
            /// <param name="stream">Bitmap / Image stream</param>
            /// <param name="type">jpg/png/gif/ect</param>
            /// <returns>String URL</returns>
            public static string Upload(Stream stream, string type = "png")
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                HttpContent co = new StringContent("fiskebolle");
                form.Add(co, "k");
                stream.Position = 0;
                co = new StreamContent(stream);
                co.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "d",
                    FileName = $"{rng.Next()}.{type}",
                    Size = stream.Length
                };
                form.Add(co);
                var res = client.PostAsync("http://u.rubixy.com/", form);
                return res.Result.Content.ReadAsStringAsync().Result;
                
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
