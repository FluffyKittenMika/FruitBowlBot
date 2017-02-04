using System;
using Nancy.Hosting.Self;


public class WebInterface
{
    private NancyHost _host;
    private string _url = "http://localhost";
    private int _port = 80;

    public void Start()
	{
        var uri = new Uri($"{_url}:{_port}/");
        var configuration = new HostConfiguration() { UrlReservations = new UrlReservations() { CreateAutomatically = true } };

        _host = new NancyHost(configuration, uri);
        _host.Start();
	}

    public void Stop()
    {
        _host.Stop();
        Console.WriteLine("If this message is ever read, then the web server is down for the bot, and there's no interface up.");
    }
}
