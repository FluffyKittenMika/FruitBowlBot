using System;
using System.Collections.Generic;
using TwitchLib;
using Discord;
using Discord.Commands;
using TwitchLib.Models.Client;
using NLua;
using System.Linq;

namespace JefBot.Commands
{
    internal class LuaPluginCommand : IPluginCommand
    {
        public string PluginName => "Lua";
        public string Command => "lua";
        public string Help => "!lua {code}";
        public IEnumerable<string> Aliases => new string[0];
        public bool Loaded { get; set; } = true;

        

        public void Execute(ChatCommand command, TwitchClient client)
        {
            //  ¯\_(ツ)_/¯
        }

        public void Discord(MessageEventArgs arg, DiscordClient client)
        {
             try
            {
                var args = arg.Message.Text.Split(' ').ToList().Skip(1).ToList();
                string script = string.Join(" ", args);
                Lua state = new Lua();
                state.DoString(@"import = function () end");
                //var res = state.DoString("return 10 + 3*(5 + 2)")[0] as double?;
                var res = state.DoString(script)[0];
                arg.Channel.SendMessage("```"+Convert.ToString(res)+"```");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                arg.Channel.SendMessage(e.Message);
            }
        }


    }
}
