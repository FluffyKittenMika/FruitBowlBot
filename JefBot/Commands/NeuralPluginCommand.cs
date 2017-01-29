using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Models.Client;
using SharpMind;

namespace JefBot.Commands
{
    internal class NeuralPLuginCommand : IPluginCommand
    {
        public string PluginName => "Neural Network test";
        public string Command => "neural";
        public string Help => "!neural {EXPERIMENTAL}";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;



        public void Execute(ChatCommand command, TwitchClient client)
        {

        }
        public void Discord(MessageEventArgs arg, DiscordClient client)
        {
            int[] top = new int[3];//inputs, hidden, output
            top[0] = 1;     //in
            top[1] = 100;   //hidden
            top[2] = 1;     //out

            Mind mind = new Mind(top);


        }
    }
}
