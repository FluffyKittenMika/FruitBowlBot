using Nancy;
using System;

namespace JefBot.Web.Pages
{
    public class Index : NancyModule
    {
        public Index()
        {
            Get[""] = _ =>
            {
                var css = @"
                <style>
                    div {
                        width: 100%;
                        border: 1px solid black;
                        padding: 5px;
                        margin: 5px;
                    }
                </style>
                ";
                int counter = 0;
                string response = css + "<h1>Crypto likes dat ass</h1>";

                foreach (var item in Bot._plugins)
                {
                    counter++;
                    response += $"<div>";
                    response += $"<p>Name:              {item.PluginName}</p>";
                    response += $"<p>Loaded:            {item.Loaded}</p>";
                    response += $"<p>Command:           {item.Command}</p>";
                    response += $"<p>Command aliases:   {String.Join(", ",item.Aliases)}</p>";
                    response += $"<p>Help:              {item.Help}</p>";
                    response += $"</div>";
                }
                response += $"<div>Amount of plugins: {counter}</div>";
                return response;
            };
        }
    }
}
