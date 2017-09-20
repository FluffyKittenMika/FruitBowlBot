using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace JefBot.Commands
{
    internal class SpongebobPluginCommand : IPluginCommand
    {
        public string PluginName => "Spongebob";
        public string Command => "spongebob";
		public IEnumerable<string> Help => new[] { "!spongebob {string}" };
        public IEnumerable<string> Aliases => new[] { "bob","sponge","mock", "666bob" };
        public bool Loaded { get; set; } = true;

        readonly Random rng = new Random();

        public async Task<string> Action(Message message)
        {
            string res = null;
            await Task.Run(() => { res =  Bob(message); }).ConfigureAwait(false);
            return res;
        }

        private char GetZalgo()
        {
            char[] source;
            switch (rng.Next(0, 3))
            {
                case 0:
                    source = ZalgoUp;
                    break;
                case 1:
                    source = ZalgoDown;
                    break;
                default:
                    source = ZalgoMid;
                    break;
            }

            return source[rng.Next() % source.Length];
        }

        private readonly char[] ZalgoUp =
        {
            '\u030d',   '\u030e',   '\u0304',   '\u0305',
            '\u033f',   '\u0311',   '\u0306',   '\u0310',
            '\u0352',   '\u0357',   '\u0351',   '\u0307',
            '\u0308',   '\u030a',   '\u0342',   '\u0343',
            '\u0344',   '\u034a',   '\u034b',   '\u034c',
            '\u0303',   '\u0302',   '\u030c',   '\u0350',
            '\u0300',   '\u0301',   '\u030b',   '\u030f',
            '\u0312',   '\u0313',   '\u0314',   '\u033d',
            '\u0309',   '\u0363',   '\u0364',   '\u0365',
            '\u0366',   '\u0367',   '\u0368',   '\u0369',
            '\u036a',   '\u036b',   '\u036c',   '\u036d',
            '\u036e',   '\u036f',   '\u033e',   '\u035b',
        };
        private readonly char[] ZalgoDown =
        {
            '\u0316',   '\u0317',   '\u0318',   '\u0319',
            '\u031c',   '\u031d',   '\u031e',   '\u031f',
            '\u0320',   '\u0324',   '\u0325',   '\u0326',
            '\u0329',   '\u032a',   '\u032b',   '\u032c',
            '\u032d',   '\u032e',   '\u032f',   '\u0330',
            '\u0331',   '\u0332',   '\u0333',   '\u0339',
            '\u033a',   '\u033b',   '\u033c',   '\u0345',
            '\u0347',   '\u0348',   '\u0349',   '\u034d',
            '\u034e',   '\u0353',   '\u0354',   '\u0355',
            '\u0356',   '\u0359',   '\u035a',   '\u0323'
        };
        private readonly char[] ZalgoMid =
        {
            '\u0315',   '\u031b',   '\u0340',   '\u0341',
            '\u0358',   '\u0321',   '\u0322',   '\u0327',
            '\u0328',   '\u0334',   '\u0335',   '\u0336',
            '\u034f',   '\u035c',   '\u035d',   '\u035e',
            '\u035f',   '\u0360',   '\u0362',   '\u0338',
            '\u0337',   '\u0361',   '\u0489'
        };

        public string Bob(Message message)
        {
            string input = String.Join(" ", message.Arguments);
            StringBuilder temp = new StringBuilder();

            if (message.Command != "666bob")
                temp.Append("http://u.rubixy.com/u/1288584f.png " + Environment.NewLine);
            else
                temp.Append("http://u.rubixy.com/u/3c34b394.jpg " + Environment.NewLine);

            for (int i = 0; i < input.Length; i++)
            {
                int r = rng.Next(1, 3);
                if (i != 0 && !Char.IsNumber(input[i]))
                {
                    if (input[i] == char.ToUpper(input[i]))
                        temp.Append(char.ToLower(input[i]));
                    if (input[i] == char.ToLower(input[i]))
                        temp.Append(char.ToUpper(input[i]));
                }
                else
                    temp.Append(char.ToLower(input[i]));
                if (message.Command == "666bob") //add zalgo
                {
                    int u = rng.Next(3, 10);
                    for (int z = 0; z < u; z++)
                        temp.Append(GetZalgo());
                }
                for (int n = 1; n <= r; n++)
                    if (i + n < input.Length)
                        temp.Append(input[i + n]);
                i += r;
            }
            
            return temp.ToString();
        }
    }
}
