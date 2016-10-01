using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JefBot
{
    class Program
    {
        //Yes
        static void Main(string[] args)
        {
            while (true)
            {
                Bot bot = new Bot();
                bot.run();
            }

        }
    }
}
