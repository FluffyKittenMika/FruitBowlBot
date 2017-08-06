using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JefBot
{
    static class Program
    {

        static void Main(string[] args)
        {
            while (true)
            {
                try //This just makes it a bit harder to permamently crash
                {
                    Bot bot = new Bot();
                    bot.Run();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

        }
    }
}
