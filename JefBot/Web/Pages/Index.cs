using Nancy;

namespace JefBot.Web.Pages
{
    public class Index : NancyModule
    {
        public Index()
        {
            Get[""] = _ =>
            {
                return Response.AsJson(Bot.settings);
            };
        }
    }
}
