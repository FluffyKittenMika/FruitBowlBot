using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.TwitchClientClasses;

namespace JefBot
{
    public interface IPluginCommand
    {
        string PluginName { get; }
        string Command { get; }
        IEnumerable<string> Aliases { get; }
        bool Loaded { get; }
        bool OffWhileLive { get; }

        void Execute(ChatCommand command, TwitchClient client);
    }
}
