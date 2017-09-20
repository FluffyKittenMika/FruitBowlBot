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
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace JefBot.Commands
{
    internal class GalaxyPluginCommand : IPluginCommand
    {
        public string PluginName => "Galaxy";
        public string Command => "galaxy";
		public IEnumerable<string> Help => new[] { "!galaxy {stars} {dimension} {frames} {arms}" };
        public IEnumerable<string> Aliases => new[] { "g" };
        public bool Loaded { get; set; } = false; //enable this when we fix black squares
        public static HttpClient client = new HttpClient();

        readonly Random rng = new Random();
        
        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => {res = GetLink(message.Arguments); }).ConfigureAwait(false);
            return res;
        }

        public string GetLink(List<string> args)
        {
            try
            {
                int stars = 2500;
                int dimension = 500;
                int frames = 1;
                int arms = 100;

                if (args.ElementAtOrDefault(0) != null) //check if position 0 of array is set for stars
                    Int32.TryParse(args[0], out stars);
                if (args.ElementAtOrDefault(1) != null) //dimension
                    Int32.TryParse(args[1], out dimension);
                if (args.ElementAtOrDefault(2) != null)
                    Int32.TryParse(args[2], out frames);
                if (args.ElementAtOrDefault(3) != null)
                    Int32.TryParse(args[3], out arms);

                return MakeGif(Galaxy(stars, dimension, frames, arms));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                Console.WriteLine(err.StackTrace);
                return err.Message;
            }
        }

        public static int Clamp(int val, int min, int max)
        {
            return (val < min) ? min : ((val > max) ? max : val);
        }

        public static double Lerp(double min, double max, double val)
        {
            return min + val * (max - min);
        }

        /// <summary>
        /// Compiles a list of bitmaps into a gif, and uploads it automatically
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <returns></returns>
        public string MakeGif(Bitmap[] bitmaps)
        {
            Stream stream = new MemoryStream();
            System.Drawing.Image image;
            var gif = File.OpenWrite("gif.gif");
            var encoder = new BumpKit.GifEncoder(gif) { FrameDelay = new TimeSpan(1) };

			foreach (Bitmap b in bitmaps)
            {
                //move bitmap to memory stream
                stream.Position = 0;
                b.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
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
        /// Makes a black bitmap to the specific size
        /// </summary>
        /// <param name="width">Width of the image</param>
        /// <param name="height">height of the image</param>
        /// <returns>Bitmap</returns>
        public static Bitmap BaseFactory(int width, int height)
        {
            Bitmap bit = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(bit))
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)))
                gfx.FillRectangle(brush, 0, 0, width, height);
            return bit;
        }

        /// <summary>
        /// / Makes a black bitmap to the specific dimension 
        /// </summary>
        /// <param name="dimension">Sqare size</param>
        /// <returns>Bitmap</returns>
        public static Bitmap BaseFactory(int dimension)
        {
            return BaseFactory(dimension, dimension);
        }

        /// <summary>
        /// Returns a list of galaxy images
        /// </summary>
        /// <param name="stars">Number of stars</param>
        /// <param name="Dimension">Size of the image</param>
        /// <param name="TotalFrames">Number of frames</param>
        /// <returns>List of images</returns>
        public Bitmap[] Galaxy(int stars = 1000, int Dimension = 250, int TotalFrames = 1, int Arms = 100)
        {
            stars = Clamp(stars, 100, 10000);
            Dimension = Clamp(Dimension, 250, 2000);
            TotalFrames = Clamp(TotalFrames, 1, 60);

            //amount of galaxy branches
            int arms;

            if (Arms == 100)
                arms = rng.Next(2, 10);
            else
                arms = Clamp(Arms, 1, 10);

            //static things
            var po = new ParallelOptions
            {
                MaxDegreeOfParallelism = 100
            };

            int centerX = (int)(0.5f * Dimension);
            int centerY = (int)(0.5f * Dimension);

            //amount of galaxy branches
            double armoffsetmax = 0.8d;
            double armdistance = 2 * Math.PI / arms;
            double armrotation = rng.Next(3, 9);

            Tuple<double, double>[] starList = new Tuple<double, double>[stars];

            Parallel.For(0, stars, po, i => {
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
                
                starList[i] = new Tuple<double, double>(distance * Dimension * 0.5f, angle / Math.PI * 180);
            });

            //make and fill images
            Bitmap[] OutList = new Bitmap[TotalFrames];

            Parallel.For(0, TotalFrames, po, frame => {
                Bitmap b = BaseFactory(Dimension);

                BitmapData bData = b.LockBits(new Rectangle(0, 0, Dimension, Dimension), ImageLockMode.ReadWrite, b.PixelFormat);

                /* GetBitsPerPixel just does a switch on the PixelFormat and returns the number */

                int bitsPerPixel = System.Drawing.Image.GetPixelFormatSize(bData.PixelFormat);
                int bytesPerPixel = bitsPerPixel / 8;

                /*the size of the image in bytes */
                int size = bData.Stride * bData.Height;

                /*Allocate buffer for image*/
                byte[] data = new byte[size];

                /*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
                System.Runtime.InteropServices.Marshal.Copy(bData.Scan0, data, 0, size);

                Parallel.For(0, starList.Count(), po, i => {
                    Tuple<double, double> star = starList[i];

                    Int32 RNG = rng.Next(10) - 5;

                    double newAngle = star.Item2 + RNG - frame * (360 / TotalFrames);

                    int newX = centerX + (int)Math.Floor(Math.Cos(newAngle / 180 * Math.PI) * star.Item1);
                    int newY = centerY + (int)Math.Floor(Math.Sin(newAngle / 180 * Math.PI) * star.Item1);

                    if (newX > 0 && newX < Dimension && newY > 0 && newY < Dimension)
                    {
                        int xoff = bytesPerPixel * newX;
                        int yoff = bytesPerPixel * Dimension * newY;
                        for (int bite = 0; bite < bytesPerPixel; bite++)
                        {
                            data[xoff + yoff + bite] = 255;
                        }
                    }
                });

                System.Runtime.InteropServices.Marshal.Copy(data, 0, bData.Scan0, data.Length);

                b.UnlockBits(bData);

                OutList[frame] = b;
            });

            return OutList;
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
    }
}