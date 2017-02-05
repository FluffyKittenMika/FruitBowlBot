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
                        width: 300px;
                        border: 25px solid black;
                        padding: 25px;
                        margin: 25px;
                    }
                </style>
                ";
                int counter = 0;
                string response = css + "<div>Crypto likes dat ass</div>";

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
                response += $"<div>{counter}</div>";
                return response;
            };
        }
    }
}
